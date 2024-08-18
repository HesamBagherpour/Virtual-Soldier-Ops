using UnityEngine;

public class Magazine : MonoBehaviour
{
    //[Serializable]
    //public class BulletData
    //{
    //    public float Range;
    //    public float Damage;
    //}

    [SerializeField] private float _bulletAmount;
    [SerializeField] private BulletScriptableObject bulletRefrence;


    private void Start()
    {
#if UnlimitedAmmo
        _bulletAmount = 9999999;
#endif
    }

    public BulletScriptableObject GetBullet()
    {
        if (_bulletAmount > 0)
        {
            _bulletAmount--;
            return bulletRefrence;
        }
        return null;
    }

    public bool HasBullet()
    {
        return _bulletAmount > 0;
    }

    public int GetBulletAmount()
    {
        return (int)_bulletAmount;
    }
}
