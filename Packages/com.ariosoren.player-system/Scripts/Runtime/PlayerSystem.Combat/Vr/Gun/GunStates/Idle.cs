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
        gunController.FirstAttachCollidersSetActive(true);
        gunController.SecondAttachColliderSetActive(true);
        gunController.BoltColliderSetActive(false);
        gunController.SetDefaultSecondaryAttachTransform();
        //TODO related to VR
        gunController.SetTwoHandRotationMode(XRGeneralGrabTransformer.TwoHandedRotationMode.FirstHandDirectedTowardsSecondHand);
        gunController.AllowTakeMagazine(true);
        handOnGun.SetSecondHandToNormal();
        handOnGun.SetToNoGrab();
    }

    public string GetNameId()
    {
        return "Idle";
    }

}