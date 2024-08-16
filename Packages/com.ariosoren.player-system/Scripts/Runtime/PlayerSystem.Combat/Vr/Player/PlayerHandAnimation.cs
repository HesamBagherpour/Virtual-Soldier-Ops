using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHandAnimation : MonoBehaviour
{
    [SerializeField] InputActionReference selectAction;
    [SerializeField] InputActionReference activateAction;
    [SerializeField] Transform controller;

    Animator handDeformAnimator;
    Animator directInteractorAnimator;

    GameObject handGameObject;
    bool isActive = true;
    bool Isis = false;

    void Awake()
    {
        var handIndex = Enumerable.Range(0, controller.childCount).Where(x => controller.GetChild(x).tag == "Hand").First();
        handGameObject = controller.GetChild(handIndex).gameObject;
        handDeformAnimator = handGameObject.GetComponent<Animator>();
        directInteractorAnimator = GetComponent<Animator>();

        //handGameObject = transform.GetChild(0).gameObject;
        //animator= handGameObject.GetComponent<Animator>();

        selectAction.action.performed += OnGripping;
        selectAction.action.canceled += OnGripRelease;

        activateAction.action.performed += OnPinching;
        activateAction.action.canceled += OnPinchRelease;
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
        if(isActive == true)
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
    public void Deactive ()
    {
        isActive = false;
        GripRelease();
        PinchRelease();
    }

    public void PlayRecoil()
    {
        if(Isis)
        {
            directInteractorAnimator.CrossFade("Recoil2",0.1f);
            Isis = false;
        }
        else
        {
            directInteractorAnimator.CrossFade("Recoil1",0.1f);
            Isis = true;
        }
    }
}