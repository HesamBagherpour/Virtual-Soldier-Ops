public class Pistol : Gun
{
    public override void DoAction()
    {
        if ( _shootingMode == ShootingMode.safety)
            return;

        Shoot();
    }

    protected override void Initialize()
    {
        GunType = GunType.Pistol;
        //Fire.performed += Fire_performed;
    }

    protected override void TriggerEnded()
    {
        //throw new System.NotImplementedException();
    }

    protected override void TriggerStarted()
    {
        DoAction();
    }

    //private void Fire_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    //{
    //    DoAction();
    //}

}

