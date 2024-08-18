using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerHandController : MonoBehaviour
{
    [SerializeField] PlayerHand hand;
    public PlayerHand Hand { get { return hand; } }
    [SerializeField] GameObject handGameObject;
    [SerializeField] Transform controller;

    [Header("Inputs")]
    [SerializeField, Range(0, 1)] float pressureSensitivity = 0.5f;
    [SerializeField] InputActionReference gripInput;
    [SerializeField] InputActionReference selectInput;
    [SerializeField] InputActionReference primaryButtonInput;
    [SerializeField] InputActionReference secondaryButtonInput;
    [SerializeField] InputActionReference handPosInput;

    PlayerHandAnimation handAnimation;

    XRDirectInteractor interactor;
    Vector3 OldHandPosition;
    float handPositionFloat;

    //PlayerHandAnimation handAnimation;
    GunController gunController;
    BoltControl boltControl;

    public event Action OnSelectChange;

    void Start()
    {
        interactor = GetComponent<XRDirectInteractor>();
        handAnimation = gameObject.GetComponent<PlayerHandAnimation>();

        interactor.selectEntered.AddListener(OnSelectEntered);
        interactor.selectExited.AddListener(OnSelectExited);

        selectInput.action.started += TakeAction;
        selectInput.action.canceled += ReleaseAction;
        gripInput.action.performed += TriggerStay;
        gripInput.action.canceled += TriggerCancel;
        primaryButtonInput.action.started += PrimaryButtonPressed;
        secondaryButtonInput.action.started += SecondaryButtonPressed;

        handPosInput.action.performed += HandPositionInput;
    }
    void OnDestroy()
    {
        interactor.selectEntered.RemoveListener(OnSelectEntered);
        interactor.selectExited.RemoveListener(OnSelectExited);

        selectInput.action.started -= TakeAction;
        selectInput.action.canceled -= ReleaseAction;
        gripInput.action.performed -= TriggerStay;
        gripInput.action.canceled -= TriggerCancel;
        primaryButtonInput.action.started -= PrimaryButtonPressed;
        secondaryButtonInput.action.started -= SecondaryButtonPressed;

        handPosInput.action.performed -= HandPositionInput;
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
        handAnimation.Deactive();
    }

    void SetGunController(GunController _gunController)
    {
        gunController = _gunController;
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
        return interactor.hasSelection;
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
}