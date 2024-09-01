using System;
using System.Threading.Tasks;
using ArioSoren.GeneralUtility;
using ArioSoren.InputControllerUtility;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Transporting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerHandController : NetworkBehaviour
{
    [SerializeField] private PlayerHand hand;
    public PlayerHand Hand => hand;
    [SerializeField] private GameObject handGameObject;
    [SerializeField] private Transform controller;

    [Header("Inputs")] [SerializeField, Range(0, 1)]
    private float pressureSensitivity = 0.5f;

    private PlayerHandAnimation _handAnimation;

    private XRDirectInteractor _interactor;
    private Vector3 _oldHandPosition;
    private float _handPositionFloat;

    //PlayerHandAnimation handAnimation;
    private GunController _gunController;
    private BoltControl _boltControl;

    public event Action OnSelectChange;
    private InputControllerComp _inputControl;

    private void Awake()
    {
        _interactor = GetComponent<XRDirectInteractor>();
        _handAnimation = gameObject.GetComponent<PlayerHandAnimation>();
        _inputControl = gameObject.GetComponent<InputControllerComp>();
    }


    public override void OnStartClient()
    {
        base.OnStartClient();
        if (IsOwner)
        {
            ChangePlayerInputSubscription(true);
        }
        else
        {
            // All Remote Clients
        }

        // All Client
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        if (IsOwner)
        {
            ChangePlayerInputSubscription(false);
        }
        else
        {
            // All Remote Clients
        }

        // All Client
    }

    private void ChangePlayerInputSubscription(bool state)
    {
        if (state)
        {
            _interactor.selectEntered.AddListener(OnSelectEntered);
            _interactor.selectExited.AddListener(OnSelectExited);

            _inputControl.GetInputActionByName(hand == PlayerHand.Right
                ? InputActionName.XRI_RightHand_Interaction_Select_Value.GetName()
                : InputActionName.XRI_LeftHand_Interaction_Select_Value.GetName()).started += TakeAction;
            _inputControl.GetInputActionByName(hand == PlayerHand.Right
                ? InputActionName.XRI_RightHand_Interaction_Select_Value.GetName()
                : InputActionName.XRI_LeftHand_Interaction_Select_Value.GetName()).canceled += ReleaseAction;
            _inputControl.GetInputActionByName(hand == PlayerHand.Right
                ? InputActionName.XRI_RightHand_Interaction_Activate_Value.GetName()
                : InputActionName.XRI_LeftHand_Interaction_Activate_Value.GetName()).performed += TriggerStay;
            _inputControl.GetInputActionByName(hand == PlayerHand.Right
                ? InputActionName.XRI_RightHand_Interaction_Activate_Value.GetName()
                : InputActionName.XRI_LeftHand_Interaction_Activate_Value.GetName()).canceled += TriggerCancel;
            _inputControl.GetInputActionByName(hand == PlayerHand.Right
                ? InputActionName.XRI_RightHand_Interaction_Switch_Down.GetName()
                : InputActionName.XRI_LeftHand_Interaction_Switch_Down.GetName()).started += PrimaryButtonPressed;
            _inputControl.GetInputActionByName(hand == PlayerHand.Right
                ? InputActionName.XRI_RightHand_Interaction_Switch_Up.GetName()
                : InputActionName.XRI_LeftHand_Interaction_Switch_Up.GetName()).started += SecondaryButtonPressed;
            _inputControl.GetInputActionByName(hand == PlayerHand.Right
                ? InputActionName.XRI_RightHand_Position.GetName()
                : InputActionName.XRI_LeftHand_Position.GetName()).performed += HandPositionInput;
        }
        else
        {
            _interactor.selectEntered.RemoveListener(OnSelectEntered);
            _interactor.selectExited.RemoveListener(OnSelectExited);

            _inputControl.GetInputActionByName(hand == PlayerHand.Right
                ? InputActionName.XRI_RightHand_Interaction_Select_Value.GetName()
                : InputActionName.XRI_LeftHand_Interaction_Select_Value.GetName()).started -= TakeAction;
            _inputControl.GetInputActionByName(hand == PlayerHand.Right
                ? InputActionName.XRI_RightHand_Interaction_Select_Value.GetName()
                : InputActionName.XRI_LeftHand_Interaction_Select_Value.GetName()).canceled -= ReleaseAction;
            _inputControl.GetInputActionByName(hand == PlayerHand.Right
                ? InputActionName.XRI_RightHand_Interaction_Activate_Value.GetName()
                : InputActionName.XRI_LeftHand_Interaction_Activate_Value.GetName()).performed -= TriggerStay;
            _inputControl.GetInputActionByName(hand == PlayerHand.Right
                ? InputActionName.XRI_RightHand_Interaction_Activate_Value.GetName()
                : InputActionName.XRI_LeftHand_Interaction_Activate_Value.GetName()).canceled -= TriggerCancel;
            _inputControl.GetInputActionByName(hand == PlayerHand.Right
                ? InputActionName.XRI_RightHand_Interaction_Switch_Down.GetName()
                : InputActionName.XRI_LeftHand_Interaction_Switch_Down.GetName()).started -= PrimaryButtonPressed;
            _inputControl.GetInputActionByName(hand == PlayerHand.Right
                ? InputActionName.XRI_RightHand_Interaction_Switch_Up.GetName()
                : InputActionName.XRI_LeftHand_Interaction_Switch_Up.GetName()).started -= SecondaryButtonPressed;
            _inputControl.GetInputActionByName(hand == PlayerHand.Right
                ? InputActionName.XRI_RightHand_Position.GetName()
                : InputActionName.XRI_LeftHand_Position.GetName()).performed -= HandPositionInput;

            //TODO if 'this gameObject' possible will destroy -> should unsubscribe input in OnDestroy method too 
        }
    }


    private void OnTriggerStay(Collider other)
    {
        if (!HasSelection() && other.transform.CompareTag("Bolt"))
            SetBoltScript(other.GetComponent<BoltControl>());
    }

    private void OnTriggerExit(Collider other)
    {
        if (!HasSelection() && other.transform.CompareTag("Bolt"))
            SetBoltScript(null);
    }

    //TODO @Network sync HideDefaultHand and OnSelectChange
    private void OnSelectEntered(SelectEnterEventArgs eventArgs)
    {
        string interactableTag = SelectedInteractable().tag;
        if (interactableTag == "Gun" || interactableTag == "ak47mag" || interactableTag == "mp5mag" ||
            interactableTag == "pistolmag")
        {
            SetGunController(SelectedInteractable().GetComponent<GunController>());
            HideDefaultHand();
            RpcSrv_OnSelect(SelectedInteractable().GetComponent<NetworkObject>());
        }

        OnSelectChange?.Invoke();
    }


    [ServerRpc]
    private void RpcSrv_OnSelect(NetworkObject gunNetworkObject, Channel channel = Channel.Reliable)
    {
        RpcObs_OnSelect(gunNetworkObject);
    }

    [ObserversRpc(RunLocally = true, BufferLast = true, ExcludeOwner = true)]
    private void RpcObs_OnSelect(NetworkObject gunNetworkObject, Channel channel = Channel.Reliable)
    {
        if (gunNetworkObject == null)
        {
            SetHandActive(true);
            SetActiveHandAnimation();
            SetGunController(null);
        }
        else
        {
            SetGunController(gunNetworkObject.gameObject.GetComponent<GunController>());
            HideDefaultHand();
        }
        
        OnSelectChange?.Invoke();
    }


    private void HideDefaultHand()
    {
        SetDeActiveHandAnimation();
        SetHandActive(false);
    }

    private async void SetHandActive(bool value)
    {
        await Task.Delay(10);
        handGameObject.SetActive(value);
    }

    private void OnSelectExited(SelectExitEventArgs eventArgs)
    {
        SetHandActive(true);
        SetActiveHandAnimation();
        SetGunController(null);

        OnSelectChange?.Invoke();
        RpcSrv_OnSelect(null);
    }

    private void SetActiveHandAnimation()
    {
        _handAnimation.Active();
    }

    private void SetDeActiveHandAnimation()
    {
        _handAnimation.Deactivate();
    }

    private void SetGunController(GunController gunControllerParam)
    {
        if (gunControllerParam == _gunController) return;

        if (_gunController != null)
        {
            Debug.unityLogger.Log(
                $"PlayerHandController | SetGunController | call RpcSrv_GunChangeOwnership with null");
            RpcSrv_GunChangeOwnership(null, _gunController.gameObject.GetComponent<NetworkObject>());
        }

        if (gunControllerParam != null)
        {
            Debug.unityLogger.Log(
                $"PlayerHandController | SetGunController | call RpcSrv_GunChangeOwnership with ownerId: {base.OwnerId}");
            RpcSrv_GunChangeOwnership(base.Owner, gunControllerParam.gameObject.GetComponent<NetworkObject>());
        }

        _gunController = gunControllerParam;
    }

    [ServerRpc]
    private void RpcSrv_GunChangeOwnership(NetworkConnection owner, NetworkObject networkObject,
        Channel channel = Channel.Reliable)
    {
        if (owner == null)
        {
            Debug.unityLogger.Log($"PlayerHandController | RpcSrv_GunChangeOwnership | ownerId is null {base.OwnerId}");
            networkObject.RemoveOwnership();
        }
        else
        {
            Debug.unityLogger.Log(
                $"PlayerHandController | RpcSrv_GunChangeOwnership | before ownerId is {networkObject.OwnerId}");
            networkObject.GiveOwnership(owner);
            Debug.unityLogger.Log(
                $"PlayerHandController | RpcSrv_GunChangeOwnership | after ownerId is {networkObject.OwnerId}");
        }
    }

    private GunController GetGunController()
    {
        return _gunController;
    }

    public void HandRecoil(PlayerHand playerHand, int numberOfHands)
    {
        _handAnimation.PlayRecoil();
    }

    public bool HasSelection()
    {
        return _interactor != null && _interactor.hasSelection;
    }

    private Transform SelectedInteractable()
    {
        return _interactor.interactablesSelected[0].transform;
    }

    private void SetBoltScript(BoltControl boltControlParam)
    {
        _boltControl = boltControlParam;
    }

    private void TakeAction(InputAction.CallbackContext callback)
    {
        _oldHandPosition = controller.localPosition;
    }

    private void ReleaseAction(InputAction.CallbackContext callback)
    {
        if (_boltControl != null && callback.ReadValue<float>() < pressureSensitivity)
        {
            _boltControl.LeaveBolt();
            SetBoltScript(null);
        }
    }

    void TriggerStay(InputAction.CallbackContext context)
    {
        if (GetGunController() != null)
        {
            var pressure = context.ReadValue<float>();
            GetGunController().TriggerStay(pressure, hand);
        }
    }

    void TriggerCancel(InputAction.CallbackContext context)
    {
        if (GetGunController() != null)
        {
            GetGunController().TriggerCancel(hand);
        }
    }

    void PrimaryButtonPressed(InputAction.CallbackContext context)
    {
        if (GetGunController() != null)
        {
            GetGunController().PrimaryButtonPressed(hand, ChangeModeDirection.down);
        }
    }

    void SecondaryButtonPressed(InputAction.CallbackContext context)
    {
        if (GetGunController() != null)
        {
            GetGunController().SecondaryButtonPressed(hand, ChangeModeDirection.up);
        }
    }

    void HandPositionInput(InputAction.CallbackContext context)
    {
        var distance = controller.localPosition - _oldHandPosition;
        if (_boltControl != null && HasSelection())
        {
            int direction = 0;
            var angle = Quaternion.Angle(Quaternion.LookRotation(distance), SelectedInteractable().rotation);
            if (angle > 90)
                direction = 1;
            else if (angle <= 90)
                direction = -1;
            _handPositionFloat = distance.magnitude * direction * 12;
            _oldHandPosition = controller.localPosition;
            _boltControl.MoveBolt(_handPositionFloat, transform.position);
        }
    }


////// Generated Code [Start] --- InputController inspector -- Don't change this block /////

    #region InputActionName

    public enum InputActionName
    {
        [EnumName("XRI LeftHand Interaction/Switch Down")]
        XRI_LeftHand_Interaction_Switch_Down,
        [EnumName("XRI LeftHand/Position")] XRI_LeftHand_Position,

        [EnumName("XRI LeftHand Interaction/Select Value")]
        XRI_LeftHand_Interaction_Select_Value,

        [EnumName("XRI LeftHand Interaction/Activate Value")]
        XRI_LeftHand_Interaction_Activate_Value,
        [EnumName("XRI RightHand/Position")] XRI_RightHand_Position,

        [EnumName("XRI RightHand Interaction/Select Value")]
        XRI_RightHand_Interaction_Select_Value,

        [EnumName("XRI RightHand Interaction/Activate Value")]
        XRI_RightHand_Interaction_Activate_Value,

        [EnumName("XRI RightHand Interaction/Switch Up")]
        XRI_RightHand_Interaction_Switch_Up,

        [EnumName("XRI RightHand Interaction/Switch Down")]
        XRI_RightHand_Interaction_Switch_Down,

        [EnumName("XRI LeftHand Interaction/Switch Up")]
        XRI_LeftHand_Interaction_Switch_Up,
    }

    #endregion InputActionName

////// Generated Code [End] --- InputController inspector -- Don't change this block /////
}