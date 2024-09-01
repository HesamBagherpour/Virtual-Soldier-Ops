using System;
using ArioSoren.PlayerSystem.Combat.PlayerSystem.Combat.VR.Gun;
using ArioSoren.PlayerSystem.Combat.PlayerSystem.Combat.VR.Gun.ShootingMode;
using ArioSoren.PlayerSystem.Combat.PlayerSystem.Combat.VR.Magazine;
using FishNet.Object;
using UnityEngine;

namespace ArioSoren.PlayerSystem.Combat.PlayerSystem.Combat.Shoot
{
    public enum GunType
    {
        None, Pistol, Rifle
    }


    public abstract class Gun : NetworkBehaviour
    {
        public PlayerInputActions PlayerControls;
        public LayerMask ValidLayers;
        public GunType GunType;
        public Action<bool> onShoot;

        private BulletScriptableObject CurrentBullets;//bullet  in gun
        private bool isBulletInGun;
        private bool ReadyToPull;

        [SerializeField] private GameObject _shootStartPosition;
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

        protected void Start()
        {
            Initialize();
            ImpactHandler.Initialize();
            _shootingModeControl.OnShootingModeChange = (mode) => { _shootingMode = mode; };
            _boltControl.OnBoltPull = BoltPuller;
            _boltControl.OnReadyToPull = () => ReadyToPull = true;
        
            //todo just for Enhance XR controller 
            // CurrentBullet = _currentMagazine.GetBullet();
            //------------------------------------
            _magazineReceiver.OnMagazineSelectEnter += (t) =>
            {
                _currentMagazine = t.GetComponent<MagazineControl>();
            };
            _magazineReceiver.OnMagazineSelectExit += () =>
            {
                _currentMagazine = null;
            };
        }

        private void BoltPuller(bool pull)
        {
            if (!ReadyToPull)
                return;

            if (_currentMagazine != null && pull)
            {
                CurrentBullets = _currentMagazine.GetBullet();
                ReadyToPull = false;
            }
        }
    
    
        // @NetworkHint called form Triggered and update
        protected void Shoot()
        {
            if (!_gunController.IsGunReadyToShoot())
                return;

            if (CurrentBullets == null)
            {
                onShoot?.Invoke(false);
                return;
            }

            RaycastHit hit;
            if (Physics.Raycast(_shootStartPosition.transform.position, transform.forward, out hit, CurrentBullets.MaxDistance, ValidLayers/*, QueryTriggerInteraction.Ignore*/))
            {
                var hitData = new HitData()
                {
                    collide = hit.collider.gameObject,
                    DamageAmount = CurrentBullets.Damage,
                    HitForce = CurrentBullets.PhysicForceOnHit,
                    HitPoint = hit.point,
                    normal = hit.normal,
                    DistanceFactor = GetDistanceFactor(_shootStartPosition.transform.position, hit.collider.gameObject.transform.position)
                };
                OnRaycastHit(hitData);
            }
            onShoot?.Invoke(true);
            _gunController.Recoil();

            CurrentBullets = _currentMagazine? _currentMagazine?.GetBullet() : null;
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

        protected virtual void OnRaycastHit(HitData data)
        {
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
            public Vector3 HitPoint;
            public float HitForce;
            public float DistanceFactor;
            public GameObject collide;
        }
    }
}