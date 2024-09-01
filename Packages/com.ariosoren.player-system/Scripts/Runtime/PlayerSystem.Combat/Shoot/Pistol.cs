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
    }

    protected override void TriggerEnded()
    {

    }

    protected override void TriggerStarted()
    {
        DoAction();
    }
}