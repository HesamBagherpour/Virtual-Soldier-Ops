using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Grabbable : XRGrabInteractable
{
    public bool isHoverable = true;
    public bool isSelectable = false;

    public bool isBoltTriggred = false;
    bool bolt = false;

    public bool allowGrab = true;
    bool _allowGrab = true;

    public GameObject rightHandObject;
    public GameObject leftHandObject;

    protected override void Awake()
    {
        base.Awake();

        if(rightHandObject || leftHandObject)
        {
            selectEntered.AddListener(EnableFirstHand);
            selectExited.AddListener(DisableFirstHand);
        }
    }

    void EnableFirstHand(SelectEnterEventArgs eventArgs)
    {
        /*var playerHandController = GetFirstSelectingInteractor().transform.GetComponent<PlayerHandController>();
        firstSelectingHand = playerHandController;*/
        if (eventArgs.interactorObject.transform.tag != "interactor")
        {
            return; 
        }
        var playerHand = firstInteractorSelecting.transform.GetComponent<PlayerHandController>();
        if (playerHand.Hand == PlayerHand.Left)
        {
            if (leftHandObject)
                leftHandObject.SetActive(true);
        }
        else
        {
            if (rightHandObject)
                rightHandObject.SetActive(true);
        }
    }

    void DisableFirstHand(SelectExitEventArgs eventArgs)
    {
        leftHandObject.SetActive(false);
        rightHandObject.SetActive(false);
    }

    public override bool IsHoverableBy(IXRHoverInteractor interactor)
    {
        if(interactor.transform.tag == "interactor")
            isHoverable = true;

        return base.IsHoverableBy(interactor) && isSelectable && ! bolt && _allowGrab;
    }

    public override bool IsSelectableBy(IXRSelectInteractor interactor)
    {
        if(interactor.transform.tag == "interactor")
        {
            _allowGrab = allowGrab;
            bolt = isBoltTriggred;
            isSelectable = true;
        }

        if(interactor.transform.tag != "interactor")
        {
            bolt = false;
            _allowGrab = true;
        }

        return base.IsSelectableBy(interactor) && isSelectable && ! bolt && _allowGrab;
    }
}