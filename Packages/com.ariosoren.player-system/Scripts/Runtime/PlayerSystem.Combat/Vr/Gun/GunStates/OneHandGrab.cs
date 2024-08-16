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
        gunController.FirstAttachColidersSetActive(false);
        gunController.SecondAttachColiderSetActive(true);
        gunController.BoltColiderSetActive(true);
        gunController.SetDefaultSecondaryAttachTransform();
        gunController.SetTwoHandRotationMode(XRGeneralGrabTransformer.TwoHandedRotationMode.FirstHandDirectedTowardsSecondHand);
        handOnGun.SetSecondHandToNormal();
        handOnGun.SetToSingleGrab(gunController.GetFirstSelectedHand());
    }

    public void TriggerStay(float value, TriggerControl triggerHandControl)
    {
        triggerHandControl.OnActionStay(value);
    }

    public void TriggerCancel(TriggerControl triggerHandControl)
    {
        triggerHandControl.OnActionCancle();
    }

    public void ChangeTriggerMode(TriggerControl triggerHandControl)
    {
        
    }
}