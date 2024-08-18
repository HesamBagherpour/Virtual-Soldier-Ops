using System;
using UnityEngine;
using UnityEngine.InputSystem;

public enum GunType
{
    None, Pistol, Rifle
}

public abstract class Gun : MonoBehaviour
{
    public PlayerInputActions PlayerControls;
    public LayerMask ValidLayers;
    public GunType GunType;
    public Action<bool> onShoot;

    private BulletScriptableObject CurrentBullet;//bullet  in gun
    private bool clipReady;
    private bool ReadyToPull;

    protected InputAction Fire;

    [SerializeField] private GameObject _shooptStartPosition;
    //[SerializeField] protected Magazine _currentMagazine;
    [SerializeField] protected MagazineControl _currentMagazine;
    [SerializeField] protected GunController _gunController;
    [SerializeField] protected ShootingMode _shootingMode;
    [SerializeField] protected BoltControl _boltControl;
    [SerializeField] protected MagazineReceiver _magazineReceiver;
    [SerializeField] private ShootingModeControl _shootingModeControl;

    protected abstract void Initialize();
    protected abstract void TriggerStarted();
    protected abstract void TriggerEnded();
    public abstract void DoAction();

    private void Awake()
    {
        PlayerControls = new PlayerInputActions();
    }
    protected void Start()
    {
        Initialize();
        ImpactHandler.Initialize();
        _shootingModeControl.OnShootingModeChange = (mode) => { _shootingMode = mode; };
        _boltControl.OnBoltPull = BoltPuller;
        _boltControl.OnReadyToPull = () => ReadyToPull = true;
        _magazineReceiver.OnMagazineSelectEnter += (t) =>
        {
            _currentMagazine = t.GetComponent<MagazineControl>();
            if (_currentMagazine != null)
            {
                clipReady = true;
            }
        };
        _magazineReceiver.OnMagazineSelectExit += () =>
        {
            clipReady = false;
            _currentMagazine = null;
        };
    }

    private void BoltPuller(bool pull)
    {
        if (_currentMagazine == null)
            Debug.LogWarning("Use bolt");

        if (!ReadyToPull)
            return;

        if (_currentMagazine != null && pull)
        {
            CurrentBullet = _currentMagazine.GetBullet();
            ReadyToPull = false;
        }
    }

    private void OnEnable()
    {
        Fire = PlayerControls.Player.Fire;
        Fire.Enable();

    }

    private void OnDisable()
    {
        Fire.Disable();
    }

    protected void Shoot()
    {
        //if (!_gunController.IsGunReadyToShoot())
            //return;

        if (CurrentBullet == null)
        {
            Debug.LogWarning("CurrentBullet == null");
            onShoot?.Invoke(false);
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(_shooptStartPosition.transform.position, //+ UnityEngine.Random.onUnitSphere * 0.1f,
             transform.forward, out hit, CurrentBullet.MaxDistance,
             ValidLayers, QueryTriggerInteraction.Ignore))
        {//OnRaycastHit(hit, currentBullet.Damage,currentBullet.PhysicForceOnHit);

            var hitData = new HitData()
            {
                collide = hit.collider.gameObject,
                DamageAmount = CurrentBullet.Damage,
                HitForce = CurrentBullet.PhysicForceOnHit,
                HitPoint = hit.point,
                normal = hit.normal,
                DistanceFactor = GetDistanceFactor(_shooptStartPosition.transform.position, hit.collider.gameObject.transform.position)
            };
            OnRaycastHit(hitData);
        }
        onShoot?.Invoke(true);
        _gunController.Recoil();

        if (!clipReady)
            Debug.LogWarning("clip not ready");

        if (_currentMagazine == null)
            Debug.LogWarning("clip is null");

        CurrentBullet = clipReady ? _currentMagazine?.GetBullet() : null;
    }

    private float GetDistanceFactor(Vector3 startPoint, Vector3 endPoint)
    {
        Vector3.Distance(startPoint, endPoint);

        return 0;
    }

    public int GetBulletCountInMagazine()
    {
        return _currentMagazine.GetBulletAmount();
    }

    //protected virtual void OnRaycastHit(RaycastHit hit, float damage)
    //{
    //    Debug.Log(hit.collider.gameObject.name);
    //    var damageable = hit.collider.GetComponent<Idamageable>();
    //    if (damageable != null)
    //        damageable.ReceiveDamage(hit, damage);
    //} 
    protected virtual void OnRaycastHit(HitData data)
    {
        Debug.Log(data.collide.name);
        var damageable = data.collide.GetComponent<Idamageable>();
        if (damageable != null)
            damageable.ReceiveDamage(data);

        if (data.collide.TryGetComponent(out MaterialType matType))
            matType.ShowImpact(data);
    }


    public class HitData
    {
        public Vector3 normal;
        public float DamageAmount;
        //public Vector3 HitPosition;
        public Vector3 HitPoint;
        public float HitForce;
        public float DistanceFactor;
        public GameObject collide;
    }
}