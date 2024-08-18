public class TwoHandGrab : IGunState
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
        gunController.SecondAttachColiderSetActive(false);
        gunController.BoltColiderSetActive(true);
        handOnGun.SetToDoubleGrab(gunController.GetFirstSelectedHand());
    }

    public void Exit()
    {
        handOnGun.SetSecondHandToNormal();
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