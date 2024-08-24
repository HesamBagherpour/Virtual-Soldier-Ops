using UnityEngine.XR.Interaction.Toolkit.Transformers;

public class OneHandGrab : IGunState
{
    GunController gunController;
    HandsOnGunControl handOnGun;

    public void init(GunController _gunController, HandsOnGunControl _handOnGun)
    {
        gunController = _gunController;
        handOnGun = _handOnGun;
    }

    public void Enter()
    {
        gunController.FirstAttachCollidersSetActive(false);
        gunController.SecondAttachColliderSetActive(true);
        gunController.BoltColliderSetActive(true);
        gunController.SetDefaultSecondaryAttachTransform();
        //TODO Network sync related to VR
        gunController.SetTwoHandRotationMode(XRGeneralGrabTransformer.TwoHandedRotationMode.FirstHandDirectedTowardsSecondHand);
        handOnGun.SetSecondHandToNormal();
        handOnGun.SetToSingleGrab(gunController.GetFirstSelectedHand());
    }

    // @NetworkHint called from player input
    public void TriggerStay(float value, TriggerControl triggerHandControl)
    {
        triggerHandControl.OnActionStay(value);
    }

    // @NetworkHint called from player input
    public void TriggerCancel(TriggerControl triggerHandControl)
    {
        triggerHandControl.OnActionCancle();
    }

    public void ChangeTriggerMode(TriggerControl triggerHandControl)
    {
        
    }
    
    public string GetNameId()
    {
        return "OneHandGrab";
    }
}