using FishNet.Object;
using FishNet.Transporting;
using UnityEngine;

public class Rifle : Gun
{
    [SerializeField] private float _durationBetweenShoot = .2f;
    [SerializeField] private bool _readyToShoot;
    private float _lastShootTime;
    [SerializeField] private bool GunTriggered;

    // @NetworkHint called form Triggered and update
    public override void DoAction()
    {
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
            _readyToShoot = false;
            _lastShootTime = Time.time;
        }
    }

    protected override void Initialize()
    {
        GunType = GunType.Rifle;
        _gunController.AddGunReactionsToTrigger(TriggerStarted, TriggerEnded);
    }

    private void Update()
    {
        if (GunTriggered) 
            DoAction();

        if (_shootingMode == ShootingMode.fullAuto)
        {
            if (Time.time > _lastShootTime + _durationBetweenShoot)
                _readyToShoot = true;
        }
        
    }

    protected override void TriggerStarted()
    {
        GunTriggered = true;
        if (_shootingMode == ShootingMode.semi)
        {
            _readyToShoot = true;
            DoAction();
        }
        RpcSrv_Triggered(true);
    }
    
    protected override void TriggerEnded()
    {
        GunTriggered = false;
        RpcSrv_Triggered(false);
    }
    
    [ServerRpc]
    private void RpcSrv_Triggered(bool state, Channel channel = Channel.Reliable)
    {
        RpcObs_Triggered(state);
    }
    [ObserversRpc(RunLocally = true, BufferLast = true,ExcludeOwner = true)]
    private void RpcObs_Triggered(bool state, Channel channel = Channel.Reliable)
    {
        GunTriggered = state;
        if (state && _shootingMode == ShootingMode.semi)
        {
            _readyToShoot = true;
            DoAction();
        }
    }

    
}

