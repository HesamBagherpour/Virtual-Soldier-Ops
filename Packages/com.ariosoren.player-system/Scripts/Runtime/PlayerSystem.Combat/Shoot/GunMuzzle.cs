using System.Collections;
using UnityEngine;

public class GunMuzzle : MonoBehaviour
{
    [SerializeField] private Transform _muzzleFlashesParent;
    [SerializeField] private float _disableTime;

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
            return;

        Transform nextFlash = _muzzleFlashesParent.GetChild(0);
        nextFlash.gameObject.SetActive(true);
        nextFlash.transform.SetAsLastSibling();
        StartCoroutine(DisableMuzzleFlash(nextFlash.gameObject));
    }


    private IEnumerator DisableMuzzleFlash(GameObject flash)
    {
        yield return new WaitForSeconds(_disableTime);
        flash.SetActive(false);
    }
}
