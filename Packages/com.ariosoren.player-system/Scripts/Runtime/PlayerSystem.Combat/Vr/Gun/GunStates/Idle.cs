using UnityEngine.XR.Interaction.Toolkit.Transformers;

public class Idle : IGunState
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
        gunController.FirstAttachColidersSetActive(true);
        gunController.SecondAttachColiderSetActive(true);
        gunController.BoltColiderSetActive(false);
        gunController.SetDefaultSecondaryAttachTransform();
        gunController.SetTwoHandRotationMode(XRGeneralGrabTransformer.TwoHandedRotationMode.FirstHandDirectedTowardsSecondHand);
        gunController.AllowTakeMagazine(true);
        handOnGun.SetSecondHandToNormal();
        handOnGun.SetToNoGrab();
    }
}