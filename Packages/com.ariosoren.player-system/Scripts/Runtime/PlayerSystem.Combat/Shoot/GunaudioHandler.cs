using UnityEngine;

namespace ArioSoren.PlayerSystem.Combat.PlayerSystem.Combat.Shoot
{
    public class GunaudioHandler : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip _emptyGunSound;
        [SerializeField] private AudioClip _ShootSound;

        private Gun _gun;

        private void Awake()
        {
            _gun = GetComponent<Gun>();
        }

        private void OnEnable()
        {
            _gun.onShoot += onShootHandle;
        }

        private void OnDisable()
        {
            _gun.onShoot -= onShootHandle;

        }

        private void onShootHandle(bool onShoot)
        {
            if (!onShoot)
            {
                audioSource.PlayOneShot(_emptyGunSound);
                return;
            }
            audioSource.PlayOneShot(_ShootSound);
        }
    }
}