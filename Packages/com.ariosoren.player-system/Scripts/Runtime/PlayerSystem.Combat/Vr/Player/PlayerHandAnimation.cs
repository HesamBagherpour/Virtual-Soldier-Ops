using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHandAnimation : MonoBehaviour
{
    [SerializeField] PlayerHand hand;
    [SerializeField] Transform controller;

    Animator handDeformAnimator;
    Animator directInteractorAnimator;

    GameObject handGameObject;
    bool isActive = true;
    bool Isis = false;

    private InputControllerComp inputControl;


    void Awake()
    {
        var handIndex = Enumerable.Range(0, controller.childCount).Where(x => controller.GetChild(x).tag == "Hand").First();
        handGameObject = controller.GetChild(handIndex).gameObject;
        handDeformAnimator = handGameObject.GetComponent<Animator>();
        directInteractorAnimator = GetComponent<Animator>();
        inputControl = gameObject.GetComponent<InputControllerComp>();

        //handGameObject = transform.GetChild(0).gameObject;
        //animator= handGameObject.GetComponent<Animator>();

        inputControl.GetInputActionByName(hand == PlayerHand.Right ? InputActionName.XRI_RightHand_Interaction_Select_Value.GetName() : InputActionName.XRI_LeftHand_Interaction_Select_Value.GetName()).performed += OnGripping;
        inputControl.GetInputActionByName(hand == PlayerHand.Right ? InputActionName.XRI_RightHand_Interaction_Select_Value.GetName() : InputActionName.XRI_LeftHand_Interaction_Select_Value.GetName()).canceled += OnGripRelease;
        inputControl.GetInputActionByName(hand == PlayerHand.Right ? InputActionName.XRI_RightHand_Interaction_Activate_Value.GetName() : InputActionName.XRI_LeftHand_Interaction_Activate_Value.GetName()).performed += OnPinching;
        inputControl.GetInputActionByName(hand == PlayerHand.Right ? InputActionName.XRI_RightHand_Interaction_Activate_Value.GetName() : InputActionName.XRI_LeftHand_Interaction_Activate_Value.GetName()).canceled += OnPinchRelease;
    }
    void OnDestroy()
    {
        inputControl.GetInputActionByName(hand == PlayerHand.Right ? InputActionName.XRI_RightHand_Interaction_Select_Value.GetName() : InputActionName.XRI_LeftHand_Interaction_Select_Value.GetName()).started -= OnGripping;
        inputControl.GetInputActionByName(hand == PlayerHand.Right ? InputActionName.XRI_RightHand_Interaction_Select_Value.GetName() : InputActionName.XRI_LeftHand_Interaction_Select_Value.GetName()).canceled  -= OnGripRelease;
        inputControl.GetInputActionByName(hand == PlayerHand.Right ? InputActionName.XRI_RightHand_Interaction_Activate_Value.GetName() : InputActionName.XRI_LeftHand_Interaction_Activate_Value.GetName()).performed -= OnPinching;
        inputControl.GetInputActionByName(hand == PlayerHand.Right ? InputActionName.XRI_RightHand_Interaction_Activate_Value.GetName() : InputActionName.XRI_LeftHand_Interaction_Activate_Value.GetName()).canceled -= OnPinchRelease;
  
    }

    void OnGripping(InputAction.CallbackContext obj)
    {
        if (isActive == true)
            handDeformAnimator.SetFloat("Grip", obj.ReadValue<float>());
    }

    void OnGripRelease(InputAction.CallbackContext obj)
    {
        GripRelease();
    }

    void OnPinching(InputAction.CallbackContext obj)
    {
        if (isActive == true)
            handDeformAnimator.SetFloat("Pinch", obj.ReadValue<float>());
    }

    void OnPinchRelease(InputAction.CallbackContext obj)
    {
        PinchRelease();
    }

    void GripRelease()
    {
        handDeformAnimator.SetFloat("Grip", 0f);
    }
    void PinchRelease()
    {
        handDeformAnimator.SetFloat("Pinch", 0f);
    }

    public void Active()
    {
        isActive = true;
    }
    public void Deactive()
    {
        isActive = false;
        GripRelease();
        PinchRelease();
    }

    public void PlayRecoil()
    {
        if (Isis)
        {
            directInteractorAnimator.CrossFade("Recoil2", 0.1f);
            Isis = false;
        }
        else
        {
            directInteractorAnimator.CrossFade("Recoil1", 0.1f);
            Isis = true;
        }
    }




    



////// Generated Code [Start] --- InputController inspector -- Don't change this block /////
#region InputActionName
public enum InputActionName
{
    [EnumNameAttribute("XRI LeftHand Interaction/Switch Down")]
    XRI_LeftHand_Interaction_Switch_Down,
    [EnumNameAttribute("XRI LeftHand/Position")]
    XRI_LeftHand_Position,
    [EnumNameAttribute("XRI LeftHand Interaction/Select Value")]
    XRI_LeftHand_Interaction_Select_Value,
    [EnumNameAttribute("XRI LeftHand Interaction/Activate Value")]
    XRI_LeftHand_Interaction_Activate_Value,
    [EnumNameAttribute("XRI RightHand/Position")]
    XRI_RightHand_Position,
    [EnumNameAttribute("XRI RightHand Interaction/Select Value")]
    XRI_RightHand_Interaction_Select_Value,
    [EnumNameAttribute("XRI RightHand Interaction/Activate Value")]
    XRI_RightHand_Interaction_Activate_Value,
    [EnumNameAttribute("XRI RightHand Interaction/Switch Up")]
    XRI_RightHand_Interaction_Switch_Up,
    [EnumNameAttribute("XRI RightHand Interaction/Switch Down")]
    XRI_RightHand_Interaction_Switch_Down,
    [EnumNameAttribute("XRI LeftHand Interaction/Switch Up")]
    XRI_LeftHand_Interaction_Switch_Up,
}
#endregion InputActionName
////// Generated Code [End] --- InputController inspector -- Don't change this block /////

}