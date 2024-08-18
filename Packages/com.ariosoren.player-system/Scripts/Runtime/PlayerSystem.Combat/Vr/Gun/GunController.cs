using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Transformers;

public class GunController : MonoBehaviour
{
    [Header("Gun Components")]
    [SerializeField] HandsOnGunControl handOnGun;
    [SerializeField] ShootingModeControl shootingMode;
    [SerializeField] TriggerControl triggerControlRight;
    [SerializeField] TriggerControl triggerControlLeft;
    [SerializeField] MagazineReceiver magazineReceiver;
    [SerializeField] BoltControl boltControl;

    [Header("Xr Components")]
    [SerializeField] XRGrabInteractable xRGrabInteractable;
    [SerializeField] XRGeneralGrabTransformer grabTransformer;

    List<GameObject> firstAttachColliders = new List<GameObject>();
    [Header("Coliders")]
    [SerializeField] Collider secondAttachColider;
    [SerializeField] Collider boltColider;

    [Header("AttachPoints")]
    [SerializeField] Transform secondAttachPoint;

    [Header("Animator")]
    [SerializeField] Animator recoil;

    [Header("GunType")]
    [SerializeField] GunType gunType;

    //[SerializeField] XRDirectInteractor xRDirectInteractor;

    IGunState gunState;
    Idle idle = new Idle();
    OneHandGrab oneHandGrab = new OneHandGrab();
    TwoHandGrab twoHandGrab = new TwoHandGrab();

    PlayerHandController firstSelectingHand;

    void Start()
    {
        xRGrabInteractable.firstSelectEntered.AddListener(OnFirstSelectEntered);
        xRGrabInteractable.selectEntered.AddListener(OnSelectEntered);
        xRGrabInteractable.selectExited.AddListener(OnSelectExited);

        //StartCoroutine(SelectEnterCoroutine());
        GetFirstAttachColiders();
        MoveToState(idle);
    }

    /*IEnumerator SelectEnterCoroutine()
    {
        yield return new WaitForSeconds(5);
        GetInteractionManager().SelectEnter(xRDirectInteractor, xRGrabInteractable);
    }*/

    public void AddGunReactionsToTrigger(Action startTrigger,Action endTrigger)
    {
        triggerControlLeft.OnTriggerStart = startTrigger;
        triggerControlRight.OnTriggerStart = startTrigger;
        triggerControlLeft.OnTriggerEnd = endTrigger;
        triggerControlRight.OnTriggerEnd = endTrigger;
    }

    public void CancelShootOnGunReleased()
    {
        if(GetSelectingInteractors().Count <= 0)
        {
            GetFirstActiveHand().OnActionCancle();
        }
    }

    void OnDestroy()
    {
        xRGrabInteractable.firstSelectEntered.RemoveListener(OnFirstSelectEntered);
        xRGrabInteractable.selectEntered.RemoveListener(OnSelectEntered);
        xRGrabInteractable.selectExited.RemoveListener(OnSelectExited);
    }

    void GetFirstAttachColiders()
    {
        foreach (var colider in xRGrabInteractable.colliders)
            if(colider != secondAttachColider && colider != boltColider)
                firstAttachColliders.Add(colider.gameObject);
    }

    void MoveToState(IGunState state)
    {
        if(gunState != null)
            gunState.Exit();

        gunState = state;
        gunState.init(this, handOnGun);
        gunState.Enter();
    }

    public bool IsGunReadyToShoot()
    {
        return gunState!= idle;
    }

    List<IXRSelectInteractor> GetSelectingInteractors() { return xRGrabInteractable.interactorsSelecting; }
    IXRSelectInteractor GetFirstSelectingInteractor() { return xRGrabInteractable.firstInteractorSelecting; }
    XRInteractionManager GetInteractionManager() { return xRGrabInteractable.interactionManager; }

    void ChangeSelection()
    {
        switch (GetSelectingInteractors().Count)
        {
            case 0:
                MoveToState(idle);
                break;
            case 1:
                MoveToState(oneHandGrab);
                break;
            case 2:
                MoveToState(twoHandGrab);
                break;
        }

        magazineReceiver.AllowSocketSelect(gunState != idle);
        boltControl.OnGunStateChnged();
    }

    public bool IsGrabbed()
    {
        return gunState != idle;
    }
    public bool IsInTwoHandGrab()
    {
        return gunState == twoHandGrab;
    }

    public void AllowTakeMagazine(bool value)
    {
        magazineReceiver.AllowSelectMagazine(value);
    }

    public void SetTwoHandRotationMode(XRGeneralGrabTransformer.TwoHandedRotationMode rotationMode)
    {
        if(gunType != GunType.Pistol)
            grabTransformer.allowTwoHandedRotation = rotationMode;
    }

    public void SetSecondaryAttachTransform(Transform transform)
    {
        xRGrabInteractable.secondaryAttachTransform = transform;
    }
    public void SetDefaultSecondaryAttachTransform()
    {
        xRGrabInteractable.secondaryAttachTransform = secondAttachPoint;
    }

    public void TriggerStay(float value, PlayerHand hand)
    {
        if(firstSelectingHand.Hand == hand)
            gunState.TriggerStay(value, GetFirstActiveHand());
    }
    public void TriggerCancel(PlayerHand hand)
    {
        if(firstSelectingHand.Hand == hand)
            gunState.TriggerCancel(GetFirstActiveHand());
    }

    public void PrimaryButtonPressed(PlayerHand hand, ChangeModeDirection direction)
    {
        if(firstSelectingHand.Hand == hand)
            shootingMode.ChangeMode(direction);
    }

    public void SecondaryButtonPressed(PlayerHand hand, ChangeModeDirection direction)
    {
        if(gunType == GunType.Rifle)
        {
            if(firstSelectingHand.Hand == hand)
                shootingMode.ChangeMode(direction);
        }
        else if(gunType == GunType.Pistol)
        {
            magazineReceiver.ForceRelease();
        }
    }

    public void Recoil()
    {
        if(gunState != idle)
        {
            string animation = gunState == oneHandGrab ? "onehand" : "twohand";
            recoil.CrossFade(animation,0.15f);
        }
    }

    internal TriggerControl GetFirstActiveHand()
    {
        if(triggerControlRight.gameObject.activeSelf)
            return triggerControlRight;
        else if(triggerControlLeft.gameObject.activeSelf)
            return triggerControlLeft;
        else return null;
    }

    internal void FirstAttachColidersSetActive(bool value)
    {    
        foreach (var colider in firstAttachColliders)
            if (colider.activeSelf != value)
                colider.SetActive(value);
    }
    internal void SecondAttachColiderSetActive(bool value)
    {
        if (secondAttachColider.gameObject.activeSelf != value)
            secondAttachColider.gameObject.SetActive(value);
    }
    internal void BoltColiderSetActive(bool value)
    {
        if (boltColider.enabled != value)
            boltColider.enabled = value;
    }

    internal PlayerHand GetFirstSelectedHand()
    {
        return firstSelectingHand.Hand;
    }

    void OnFirstSelectEntered(SelectEnterEventArgs eventArgs)
    {
        var playerHandController = GetFirstSelectingInteractor().transform.GetComponent<PlayerHandController>();
        firstSelectingHand = playerHandController;
    }

    void OnSelectEntered(SelectEnterEventArgs eventArgs)
    {
        ChangeSelection();
    }

    void OnSelectExited(SelectExitEventArgs eventArgs)
    {
        CancelShootOnGunReleased();
        CancelSelectionsOnFirstSelectExit(eventArgs);
        ChangeSelection();
    }

    void CancelSelectionsOnFirstSelectExit(SelectExitEventArgs args)
    {
        if(!GetFirstSelectingInteractor().hasSelection)
            GetInteractionManager().CancelInteractableSelection(args.interactableObject);
    }
}