using System;
using System.Collections.Generic;
using System.Linq;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Transporting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Transformers;

public class GunController : NetworkBehaviour
{
    [Header("Gun Components")] [SerializeField]
    private HandsOnGunControl handOnGun;

    [SerializeField] private ShootingModeControl shootingMode;
    [SerializeField] private TriggerControl triggerControlRight;
    [SerializeField] private TriggerControl triggerControlLeft;
    [SerializeField] private MagazineReceiver magazineReceiver;
    [SerializeField] private BoltControl boltControl;

    [Header("Xr Components")] [SerializeField]
    private XRGrabInteractable xRGrabIntractable;

    [SerializeField] XRGeneralGrabTransformer grabTransformer;

    private List<GameObject> _firstAttachColliders;
    [Header("Colliders")] [SerializeField] private Collider secondAttachCollider;
    [SerializeField] private Collider boltCollider;

    [Header("AttachPoints")] [SerializeField]
    private Transform secondAttachPoint;

    [Header("Animator")] [SerializeField] private Animator recoil;

    [Header("GunType")] [SerializeField] private GunType gunType;

    private IGunState _gunState;
    private Idle _idle;
    private OneHandGrab _oneHandGrab;
    private TwoHandGrab _twoHandGrab;

    private PlayerHandController _firstSelectingHand;

    private void Awake()
    {
        _firstAttachColliders = new List<GameObject>();
        _idle = new Idle();
        _oneHandGrab = new OneHandGrab();
        _twoHandGrab = new TwoHandGrab();
    }

    #region Client

    public override void OnStartClient()
    {
        base.OnStartClient();

        ChangePlayerInputSubscription(true);
        GetFirstAttachColliders();
        MoveToState(_idle);

        if (base.IsOwner)
        {
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

        ChangePlayerInputSubscription(false);

        if (base.IsOwner)
        {
        }
        else
        {
            // All Remote Clients
        }

        // All Client
    }

    #endregion

    public override void OnOwnershipClient(NetworkConnection prevOwner)
    {
        base.OnOwnershipClient(prevOwner);
        Debug.unityLogger.Log($"GunController | OnOwnershipClient | started. prevOwner: {prevOwner.ClientId} - owner: {OwnerId}");

    }

    public override void OnOwnershipServer(NetworkConnection prevOwner)
    {
        base.OnOwnershipServer(prevOwner);
        Debug.unityLogger.Log($"GunController | OnOwnershipServer | started. prevOwner: {prevOwner.ClientId} - owner: {OwnerId}");
    }


    #region Logics

    private void ChangePlayerInputSubscription(bool state)
    {
        if (state)
        {
            xRGrabIntractable.firstSelectEntered.AddListener(OnFirstSelectEntered);
            xRGrabIntractable.selectEntered.AddListener(OnSelectEntered);
            xRGrabIntractable.selectExited.AddListener(OnSelectExited);
        }
        else
        {
            xRGrabIntractable.firstSelectEntered.RemoveListener(OnFirstSelectEntered);
            xRGrabIntractable.selectEntered.RemoveListener(OnSelectEntered);
            xRGrabIntractable.selectExited.RemoveListener(OnSelectExited);

            //TODO if 'this gameObject' possible will destroy -> should unsubscribe input in OnDestroy method too 
        }
    }

    #endregion


    // @NetworkHint called on rifle's init <- start method
    public void AddGunReactionsToTrigger(Action startTrigger, Action endTrigger)
    {
        triggerControlLeft.OnTriggerStart = startTrigger;
        triggerControlRight.OnTriggerStart = startTrigger;
        triggerControlLeft.OnTriggerEnd = endTrigger;
        triggerControlRight.OnTriggerEnd = endTrigger;
    }

    // @NetworkHint called from player input
    private void CancelShootOnGunReleased()
    {
        if (GetSelectingInteractors().Count <= 0)
        {
            GetFirstActiveHand().OnActionCancle();
        }
    }

    private void GetFirstAttachColliders()
    {
        foreach (var colliderObj in xRGrabIntractable.colliders
                     .Where(colliderObj => colliderObj != secondAttachCollider
                                           && colliderObj != boltCollider))
            _firstAttachColliders.Add(colliderObj.gameObject);
    }

    #region MoveToState

    private void MoveToState(IGunState state)
    {
        _gunState?.Exit();
        _gunState = state;
        _gunState.init(this, handOnGun);
        _gunState.Enter();
        RpcSrv_MoveToState(state.GetNameId());
    }

    [ServerRpc(RequireOwnership = false)]
    private void RpcSrv_MoveToState(string stateNameId, Channel channel = Channel.Reliable)
    {
        RpcObs_MoveToState(stateNameId);
    }

    [ObserversRpc(RunLocally = true, BufferLast = true, ExcludeOwner = true)]
    private void RpcObs_MoveToState(string stateNameId, Channel channel = Channel.Reliable)
    {
        Debug.unityLogger.Log($"GunController | RpcObs_MoveToState | Started. stateNameId: {stateNameId}");
        _gunState?.Exit();
        _gunState = GetStateByNameId(stateNameId);
        _gunState?.init(this, handOnGun);
        _gunState?.Enter();
    }

    private IGunState GetStateByNameId(string nameId)
    {
        switch (nameId)
        {
            case "Idle":
                return _idle;
            case "OneHandGrab":
                return _oneHandGrab;
            case "TwoHandGrab":
                return _twoHandGrab;
            default: return null;
        }
    }

    #endregion


    public bool IsGunReadyToShoot()
    {
        return _gunState != _idle;
    }

    private List<IXRSelectInteractor> GetSelectingInteractors()
    {
        return xRGrabIntractable.interactorsSelecting;
    }

    private IXRSelectInteractor GetFirstSelectingInteractor()
    {
        return xRGrabIntractable.firstInteractorSelecting;
    }

    private XRInteractionManager GetInteractionManager()
    {
        return xRGrabIntractable.interactionManager;
    }

    #region ChangeSelection

    // @NetworkHint Called from player input
    private void ChangeSelection()
    {
        switch (GetSelectingInteractors().Count)
        {
            case 0:
                MoveToState(_idle);
                break;
            case 1:
                MoveToState(_oneHandGrab);
                break;
            case 2:
                MoveToState(_twoHandGrab);
                break;
        }

        magazineReceiver.AllowSocketSelect(_gunState != _idle);
        boltControl.OnGunStateChnged();
        RpcSrv_ChangeSelection(_gunState != _idle);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RpcSrv_ChangeSelection(bool isAllowSocketSelect, Channel channel = Channel.Reliable)
    {
        RpcObs_ChangeSelection(isAllowSocketSelect);
    }

    [ObserversRpc(RunLocally = true, ExcludeOwner = true)]
    private void RpcObs_ChangeSelection(bool isAllowSocketSelect, Channel channel = Channel.Reliable)
    {
        // magazineReceiver.AllowSocketSelect(isAllowSocketSelect);
        boltControl.OnGunStateChnged();
    }

    #endregion

    public bool IsGrabbed()
    {
        return _gunState != _idle;
    }

    public bool IsInTwoHandGrab()
    {
        return _gunState == _twoHandGrab;
    }

    public void AllowTakeMagazine(bool value)
    {
        magazineReceiver.AllowSelectMagazine(value);
    }

    // TODO @Network VR -> should just run on owner
    public void SetTwoHandRotationMode(XRGeneralGrabTransformer.TwoHandedRotationMode rotationMode)
    {
        if (!IsOwner) return;
        if (gunType != GunType.Pistol)
            grabTransformer.allowTwoHandedRotation = rotationMode;
    }

    // TODO @Network VR -> should just run on owner
    public void SetSecondaryAttachTransform(Transform transformParam)
    {
        if (!IsOwner) return;
        xRGrabIntractable.secondaryAttachTransform = transformParam;
    }

    // TODO @Network check VR -> should just run on owner
    public void SetDefaultSecondaryAttachTransform()
    {
        if (!IsOwner) return;
        xRGrabIntractable.secondaryAttachTransform = secondAttachPoint;
    }

    // @NetworkHint called from player input
    // TODO Network sync -> values
    public void TriggerStay(float value, PlayerHand hand)
    {
        if (_firstSelectingHand.Hand == hand)
            _gunState.TriggerStay(value, GetFirstActiveHand());
    }

    // @NetworkHint called from player input
    // TODO Network sync -> values
    public void TriggerCancel(PlayerHand hand)
    {
        if (_firstSelectingHand.Hand == hand)
            _gunState.TriggerCancel(GetFirstActiveHand());
    }

    // @NetworkHint called from player input
    // TODO Network sync -> values
    public void PrimaryButtonPressed(PlayerHand hand, ChangeModeDirection direction)
    {
        if (_firstSelectingHand.Hand == hand)
            shootingMode.ChangeMode(direction);
    }

    // @NetworkHint called from player input
    // TODO Network sync -> values
    public void SecondaryButtonPressed(PlayerHand hand, ChangeModeDirection direction)
    {
        switch (gunType)
        {
            case GunType.Rifle:
            {
                if (_firstSelectingHand.Hand == hand)
                    shootingMode.ChangeMode(direction);
                break;
            }
            case GunType.Pistol:
                magazineReceiver.ForceRelease();
                break;
        }
    }

    // @NetworkHint called form Shoot -> this just set animator -> not need sync
    public void Recoil()
    {
        if (_gunState == _idle) return;
        var animationVar = _gunState == _oneHandGrab ? "onehand" : "twohand";
        recoil.CrossFade(animationVar, 0.15f);
    }

    private TriggerControl GetFirstActiveHand()
    {
        if (triggerControlRight.gameObject.activeSelf)
            return triggerControlRight;

        return triggerControlLeft.gameObject.activeSelf ? triggerControlLeft : null;
    }

    internal void FirstAttachCollidersSetActive(bool value)
    {
        foreach (var colliderItem in _firstAttachColliders.Where(colliderItem => colliderItem.activeSelf != value))
            colliderItem.SetActive(value);
    }

    internal void SecondAttachColliderSetActive(bool value)
    {
        if (secondAttachCollider.gameObject.activeSelf != value)
            secondAttachCollider.gameObject.SetActive(value);
    }

    internal void BoltColliderSetActive(bool value)
    {
        if (boltCollider.enabled != value)
            boltCollider.enabled = value;
    }

    internal PlayerHand GetFirstSelectedHand()
    {
        return _firstSelectingHand.Hand;
    }

    //TODO @Network sync 
    private void OnFirstSelectEntered(SelectEnterEventArgs eventArgs)
    {
        Debug.unityLogger.Log("GunController | OnFirstSelectEntered | started.");
        var playerHandController = GetFirstSelectingInteractor().transform.GetComponent<PlayerHandController>();
        _firstSelectingHand = playerHandController;
        RpcSrv_OnFirstSelectEntered(playerHandController.NetworkObject);
    }
    
    
    [ServerRpc(RequireOwnership = false)]
    private void RpcSrv_OnFirstSelectEntered(NetworkObject networkObject, Channel channel = Channel.Reliable)
    {
        RpcObs_OnFirstSelectEntered(networkObject);
    }

    [ObserversRpc(RunLocally = true, BufferLast = true, ExcludeOwner = true)]
    private void RpcObs_OnFirstSelectEntered(NetworkObject networkObject, Channel channel = Channel.Reliable)
    {
        Debug.unityLogger.Log("GunController | RpcObs_OnFirstSelectEntered | started.");
        _firstSelectingHand = networkObject.GetComponent<PlayerHandController>();
        Debug.unityLogger.Log($"GunController | RpcObs_OnFirstSelectEntered | _firstSelectingHand == null {_firstSelectingHand == null}");
    }

    private void OnSelectEntered(SelectEnterEventArgs eventArgs)
    {
        Debug.unityLogger.Log("GunController | OnSelectEntered | started.");
        ChangeSelection();
        RpcObs_OnSelect(true);
    }
        
    //TODO @Network sync expect ChangeSelection()
    private void OnSelectExited(SelectExitEventArgs eventArgs)
    {
        Debug.unityLogger.Log("GunController | OnSelectExited | started.");
        CancelShootOnGunReleased();
        CancelSelectionsOnFirstSelectExit(eventArgs);
        ChangeSelection();
        RpcObs_OnSelect(false);
    }


    [ServerRpc]
    private void RpcSrv_OnSelect(bool state, Channel channel = Channel.Reliable)
    {
        RpcObs_OnSelect(state);
    }

    [ObserversRpc(RunLocally = true, BufferLast = true, ExcludeOwner = true)]
    private void RpcObs_OnSelect(bool state, Channel channel = Channel.Reliable)
    {
        Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();
        if (state && rigidbody)
        {
            rigidbody.angularDrag = 0;
            rigidbody.isKinematic = true;
        }
        else if (rigidbody)
        {
            rigidbody.angularDrag = 0.05f;
            rigidbody.isKinematic = false;
        }
        else
        {
            //TODO @Network sync
            //CancelShootOnGunReleased(); // implement in inner method
            //CancelSelectionsOnFirstSelectExit(eventArgs);
        }
    }

    private void CancelSelectionsOnFirstSelectExit(SelectExitEventArgs args)
    {
        if (!GetFirstSelectingInteractor().hasSelection)
            GetInteractionManager().CancelInteractableSelection(args.interactableObject);
    }
}