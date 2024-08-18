using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using static Gun;

public class ImpactHandler
{
    [SerializeField] private ImpactList impactlist;
    private static ImpactHandler _instance;
    private GameObject _root;
    //[SerializeField] private GameObject bulletImpactPrefab;
    //[SerializeField] private GameObject HitVfxPrefab;
    private ObjectPool<GameObject> _BulletImpactpool;

    public List<GameObject> Poolmanager;
    public int poolManagerMaxCapasity = 10;
    public int poolManagerLastIndex = -1;

    private void VFXPoolInitiazlize()
    {
        _BulletImpactpool = new ObjectPool<GameObject>(
            () => { return GameObject.Instantiate(impactlist.HitVfxPrefab); },//create
            (impact) => { impact.SetActive(true); },//onGet
            (impact) => { impact.SetActive(false); },//Onrelease
            (impact) => { GameObject.Destroy(impact); },//onDestroy
                false, 5, 10);
    }

    public static void Initialize()
    {
        if (_instance != null)
            return;
        _instance = new ImpactHandler();
        _instance._root = new GameObject();
        _instance._root.name = "ImpactHandler";

        _instance.VFXPoolInitiazlize();

        _instance.Poolmanager = new List<GameObject>();
        _instance.impactlist = Resources.Load<ImpactList>("Impact");
    }

   
    public static void GenerateHitImpact(HitData data)
    {
        if (_instance == null)
            _instance = new ImpactHandler();

        if (_instance.impactlist != null)
        {
            _instance.MakeBulletHole(data);
            _instance.MakeImpactVFX(data);
        }
    }



    private GameObject GetBulletFromPool()
    {
        Poolmanager.RemoveAll(x => !x);//remove destroyed bullet holes
        if (Poolmanager.Count < poolManagerMaxCapasity)
        {
            var CurrentBulletImpact = GameObject.Instantiate(impactlist.bulletImpactPrefab);
            Poolmanager.Add(CurrentBulletImpact);
            poolManagerLastIndex = Poolmanager.Count - 1;
        }
        else
        {
            poolManagerLastIndex++;
            if (poolManagerLastIndex == poolManagerMaxCapasity)
            {
                poolManagerLastIndex = 0;
            }
        }
        return Poolmanager[poolManagerLastIndex];
    }

    private void MakeBulletHole(HitData data)
    {
        GameObject CurrentBulletImpact = GetBulletFromPool();// Poolmanager[poolManagerLastIndex];
        CurrentBulletImpact.transform.SetParent(data.collide.transform);
        CurrentBulletImpact.transform.position = data.HitPoint;// +Vector3.forward*0.1f;
        CurrentBulletImpact.transform.rotation = Quaternion.LookRotation(data.normal);
    }

    private void MakeImpactVFX(HitData data)
    {
        var bulletimpact = _BulletImpactpool.Get();
        bulletimpact.transform.SetParent(_root.transform);
        bulletimpact.transform.position = data.HitPoint;
        bulletimpact.transform.rotation = Quaternion.LookRotation(data.normal);

        bulletimpact.GetComponent<DisableObject>().Initialize(0.5f,
            () =>
            {
                _BulletImpactpool.Release(bulletimpact);
            });
    }
}
