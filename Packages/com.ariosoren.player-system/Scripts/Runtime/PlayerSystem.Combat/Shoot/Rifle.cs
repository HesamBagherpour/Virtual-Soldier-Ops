using UnityEngine;

public class Rifle : Gun
{
    [SerializeField] private float _durationBetweenShoot = .2f;
    [SerializeField] private bool _readyToShoot;
    private float _lastShootTime;
    [SerializeField] private bool GunTriggered;
    //private int brustshotingCount = 0;

    public override void DoAction()
    {
        //Debug.Log("DoAction");
        if (!_readyToShoot || _shootingMode == ShootingMode.safety)
            return;

        if(_currentMagazine != null)
        {
            if (!_currentMagazine.HasBullet())
            {
                Debug.Log("Rifle magazine is empty");
                return;
            }
            Shoot();
        //brustshotingCount++;
            _readyToShoot = false;
            _lastShootTime = Time.time;
        }
    }

    protected override void Initialize()
    {
        GunType = GunType.Rifle;
        _gunController.AddGunReactionsToTrigger(TriggerStarted, TriggerEnded);
        //Fire.performed += Fire_performed;
    }

    private void Update()
    {
        //if (Fire.IsPressed())
        //{
        //    DoAction();
        //}

        if (GunTriggered) 
            DoAction();

        if (_shootingMode == ShootingMode.fullAuto)
        {
            //Debug.Log("fullAuto");
            if (Time.time > _lastShootTime + _durationBetweenShoot)
                _readyToShoot = true;
        }

        //if (_shootingMode == ShootingMode.burst)
        //{
        //    Debug.Log("burst");
        //    if (Time.time > _lastShootTime + _durationBetweenShoot && brustshotingCount < 3)
        //    {
        //        _readyToShoot = true;
        //    }
        //    //if (!Fire.IsPressed())
        //    //    brustshotingCount = 0;
        //}
    }

    protected override void TriggerStarted()
    {
        GunTriggered = true;
        //Debug.Log("TriggerStarted");
        if (_shootingMode == ShootingMode.semi)
        {
            _readyToShoot = true;
            DoAction();
        }
    }

    protected override void TriggerEnded()
    {
        GunTriggered = false;
        //brustshotingCount = 0;
    }
}

