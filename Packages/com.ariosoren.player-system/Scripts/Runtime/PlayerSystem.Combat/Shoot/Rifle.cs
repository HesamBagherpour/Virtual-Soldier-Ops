using System.Collections;
using UnityEngine;

public class Rifle : Gun
{
    [SerializeField] private float _durationBetweenShoot = .2f;
    [SerializeField] private bool _readyToShoot;
    [SerializeField] private bool GunTriggered;

    public override void DoAction()
    {
        if (!_readyToShoot)
            return;

        if(_currentMagazine != null)
        {
            Shoot();
            _readyToShoot = false;
        }
    }

    protected override void Initialize()
    {
        GunType = GunType.Rifle;
        _gunController.AddGunReactionsToTrigger(TriggerStarted, TriggerEnded);
    }

    protected override void TriggerStarted()
    {
        GunTriggered = true;
        if (_shootingMode == ShootingMode.semi)
        {
            _readyToShoot = true;
            DoAction();
        }
        else if (_shootingMode == ShootingMode.fullAuto)
            StartCoroutine(ShootCoroutine());
    }

    IEnumerator ShootCoroutine()
    {
        yield return new WaitForSeconds(_durationBetweenShoot);

        if (_shootingMode == ShootingMode.fullAuto)
        {
            _readyToShoot = true;

            DoAction();
            if(GunTriggered)
                StartCoroutine(ShootCoroutine());
        }
    }

    protected override void TriggerEnded()
    {
        GunTriggered = false;
    }
}