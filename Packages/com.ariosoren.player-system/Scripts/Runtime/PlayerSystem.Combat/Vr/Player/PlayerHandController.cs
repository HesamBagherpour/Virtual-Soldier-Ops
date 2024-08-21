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
    [SerializeField] PlayerHand hand;
    public PlayerHand Hand { get { return hand; } }
    [SerializeField] GameObject handGameObject;
    [SerializeField] Transform controller;

    [Header("Inputs")]
    [SerializeField, Range(0, 1)] float pressureSensitivity = 0.5f;

    PlayerHandAnimation handAnimation;

    XRDirectInteractor interactor;
    Vector3 OldHandPosition;
    float handPositionFloat;

    //PlayerHandAnimation handAnimation;
    GunController gunController;
    BoltControl boltControl;

    public event Action OnSelectChange;
    private InputControllerComp inputControl;

    void Awake(){
        interactor = GetComponent<XRDirectInteractor>();
        handAnimation = gameObject.GetComponent<PlayerHandAnimation>();
        inputControl = gameObject.GetComponent<InputControllerComp>();
    }

    void Start()
    {
        // interactor = GetComponent<XRDirectInteractor>();
        // handAnimation = gameObject.GetComponent<PlayerHandAnimation>();

        interactor.selectEntered.AddListener(OnSelectEntered);
        interactor.selectExited.AddListener(OnSelectExited);

        inputControl.GetInputActionByName(hand == PlayerHand.Right ? InputActionName.XRI_RightHand_Interaction_Select_Value.GetName() : InputActionName.XRI_LeftHand_Interaction_Select_Value.GetName()).started += TakeAction;
        inputControl.GetInputActionByName(hand == PlayerHand.Right ? InputActionName.XRI_RightHand_Interaction_Select_Value.GetName() : InputActionName.XRI_LeftHand_Interaction_Select_Value.GetName()).canceled  += ReleaseAction;
        inputControl.GetInputActionByName(hand == PlayerHand.Right ? InputActionName.XRI_RightHand_Interaction_Activate_Value.GetName() : InputActionName.XRI_LeftHand_Interaction_Activate_Value.GetName()).performed += TriggerStay;
        inputControl.GetInputActionByName(hand == PlayerHand.Right ? InputActionName.XRI_RightHand_Interaction_Activate_Value.GetName() : InputActionName.XRI_LeftHand_Interaction_Activate_Value.GetName()).canceled += TriggerCancel;
        inputControl.GetInputActionByName(hand == PlayerHand.Right ? InputActionName.XRI_RightHand_Interaction_Switch_Down.GetName() : InputActionName.XRI_LeftHand_Interaction_Switch_Down.GetName()).started += PrimaryButtonPressed;
        inputControl.GetInputActionByName(hand == PlayerHand.Right ? InputActionName.XRI_RightHand_Interaction_Switch_Up.GetName() : InputActionName.XRI_LeftHand_Interaction_Switch_Up.GetName()).started += SecondaryButtonPressed;
        inputControl.GetInputActionByName(hand == PlayerHand.Right ? InputActionName.XRI_RightHand_Position.GetName() : InputActionName.XRI_LeftHand_Position.GetName()).performed += HandPositionInput;
    
    }
    void OnDestroy()
    {
        interactor.selectEntered.RemoveListener(OnSelectEntered);
        interactor.selectExited.RemoveListener(OnSelectExited);

        inputControl.GetInputActionByName(hand == PlayerHand.Right ? InputActionName.XRI_RightHand_Interaction_Select_Value.GetName() : InputActionName.XRI_LeftHand_Interaction_Select_Value.GetName()).started -= TakeAction;
        inputControl.GetInputActionByName(hand == PlayerHand.Right ? InputActionName.XRI_RightHand_Interaction_Select_Value.GetName() : InputActionName.XRI_LeftHand_Interaction_Select_Value.GetName()).canceled  -= ReleaseAction;
        inputControl.GetInputActionByName(hand == PlayerHand.Right ? InputActionName.XRI_RightHand_Interaction_Activate_Value.GetName() : InputActionName.XRI_LeftHand_Interaction_Activate_Value.GetName()).performed -= TriggerStay;
        inputControl.GetInputActionByName(hand == PlayerHand.Right ? InputActionName.XRI_RightHand_Interaction_Activate_Value.GetName() : InputActionName.XRI_LeftHand_Interaction_Activate_Value.GetName()).canceled -= TriggerCancel;
        inputControl.GetInputActionByName(hand == PlayerHand.Right ? InputActionName.XRI_RightHand_Interaction_Switch_Down.GetName() : InputActionName.XRI_LeftHand_Interaction_Switch_Down.GetName()).started -= PrimaryButtonPressed;
        inputControl.GetInputActionByName(hand == PlayerHand.Right ? InputActionName.XRI_RightHand_Interaction_Switch_Up.GetName() : InputActionName.XRI_LeftHand_Interaction_Switch_Up.GetName()).started -= SecondaryButtonPressed;
        inputControl.GetInputActionByName(hand == PlayerHand.Right ? InputActionName.XRI_RightHand_Position.GetName() : InputActionName.XRI_LeftHand_Position.GetName()).performed -= HandPositionInput;
  
    }

    void OnTriggerStay(Collider other)
    {
        if( ! HasSelection() && other.transform.tag == "Bolt")
            SetBoltScript(other.GetComponent<BoltControl>());
    }
    void OnTriggerExit(Collider other)
    {
        if( ! HasSelection() && other.transform.tag == "Bolt")
            SetBoltScript(null);
    }

    void OnSelectEntered(SelectEnterEventArgs eventArgs)
    {
        string interactableTag = SelectedInteractable().tag;
        if (interactableTag == "Gun" || interactableTag == "ak47mag" || interactableTag == "mp5mag" || interactableTag == "pistolmag")
        {
            SetGunController(SelectedInteractable().GetComponent<GunController>());
            HideDefaultHand();
        }
        
        OnSelectChange?.Invoke();
    }

    public void HideDefaultHand()
    {
        SetDeActiveHandAnimation();
        SetHandActive(false);
    }

    async void SetHandActive(bool value)
    {
        await Task.Delay(10);
        handGameObject.SetActive(value);
    }

    void OnSelectExited(SelectExitEventArgs eventArgs)
    {
        SetHandActive(true);
        SetActiveHandAnimation();
        SetGunController(null);

        OnSelectChange?.Invoke();
    }

    void SetActiveHandAnimation()
    {
        handAnimation.Active();
    }
    void SetDeActiveHandAnimation()
    {
        handAnimation.Deactivate();
    }

    void SetGunController(GunController _gunController)
    {
        if(_gunController == gunController) return;

        if (gunController != null)
        {
            Debug.unityLogger.Log($"PlayerHandController | SetGunController | call RpcSrv_GunChangeOwnership with null");
            RpcSrv_GunChangeOwnership(null,gunController.gameObject.GetComponent<NetworkObject>());
        }
        
        if (_gunController != null)
        {
            Debug.unityLogger.Log($"PlayerHandController | SetGunController | call RpcSrv_GunChangeOwnership with ownerId: {base.OwnerId}");
            RpcSrv_GunChangeOwnership(base.Owner,_gunController.gameObject.GetComponent<NetworkObject>());
        }

        gunController = _gunController;
    }

    [ServerRpc]
    private void RpcSrv_GunChangeOwnership(NetworkConnection owner,NetworkObject networkObject ,Channel channel = Channel.Reliable)
    {
        if (owner == null)
        {
            Debug.unityLogger.Log($"PlayerHandController | RpcSrv_GunChangeOwnership | ownerId is null {base.OwnerId}");
            networkObject.RemoveOwnership();
        }
        else
        {
            Debug.unityLogger.Log($"PlayerHandController | RpcSrv_GunChangeOwnership | before ownerId is {networkObject.OwnerId}");
            networkObject.GiveOwnership(owner);
            Debug.unityLogger.Log($"PlayerHandController | RpcSrv_GunChangeOwnership | after ownerId is {networkObject.OwnerId}");
        }
    }
    
    GunController GetGunController()
    {
        return gunController;
    }

    public void HandRecoil(PlayerHand _hand, int numberOfHands)
    {
        handAnimation.PlayRecoil();
    }

    public bool HasSelection()
    {
        return interactor != null && interactor.hasSelection;
    }

    Transform SelectedInteractable()
    {
        return interactor.interactablesSelected[0].transform;
    }

    void SetBoltScript(BoltControl _boltControl)
    {
        boltControl = _boltControl;
    }

    void TakeAction(InputAction.CallbackContext callback)
    {
        OldHandPosition = controller.localPosition;
    }

    void ReleaseAction(InputAction.CallbackContext callback)
    {
        if(boltControl != null && callback.ReadValue<float>() < pressureSensitivity)
        {
            boltControl.LeaveBolt();
            SetBoltScript(null);
        }
    }

    void TriggerStay(InputAction.CallbackContext context)
    {
        if(GetGunController() != null)
        {
            var pressure = context.ReadValue<float>();
            GetGunController().TriggerStay(pressure, hand);
        }
    }
    void TriggerCancel(InputAction.CallbackContext context)
    {
        if(GetGunController() != null)
        {
            GetGunController().TriggerCancel(hand);
        }
    }

    void PrimaryButtonPressed(InputAction.CallbackContext context)
    {
        if(GetGunController() != null)
        {
            //GetGunController().ChangeShootingMode(hand, ChangeModeDirection.down);
            GetGunController().PrimaryButtonPressed(hand, ChangeModeDirection.down);
        }
    }

    void SecondaryButtonPressed(InputAction.CallbackContext context)
    {
        if(GetGunController() != null)
        {
            //GetGunController().ChangeShootingMode(hand, ChangeModeDirection.up);
            GetGunController().SecondaryButtonPressed(hand, ChangeModeDirection.up);
        }
    }

    void HandPositionInput(InputAction.CallbackContext context)
    {
        if(boltControl != null && HasSelection())
        {
            var distance = controller.localPosition - OldHandPosition;

            int direction = 0;
            var angle = Quaternion.Angle(Quaternion.LookRotation(distance), SelectedInteractable().rotation);

            if (angle > 120)
                direction = 1;
            else if (angle < 60)
                direction = -1;

            handPositionFloat = distance.magnitude * direction * 10;
            OldHandPosition = controller.localPosition;
            boltControl.MoveBolt(handPositionFloat);
        }
    }


////// Generated Code [Start] --- InputController inspector -- Don't change this block /////
#region InputActionName
public enum InputActionName
{
    [EnumName("XRI LeftHand Interaction/Switch Down")]
    XRI_LeftHand_Interaction_Switch_Down,
    [EnumName("XRI LeftHand/Position")]
    XRI_LeftHand_Position,
    [EnumName("XRI LeftHand Interaction/Select Value")]
    XRI_LeftHand_Interaction_Select_Value,
    [EnumName("XRI LeftHand Interaction/Activate Value")]
    XRI_LeftHand_Interaction_Activate_Value,
    [EnumName("XRI RightHand/Position")]
    XRI_RightHand_Position,
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