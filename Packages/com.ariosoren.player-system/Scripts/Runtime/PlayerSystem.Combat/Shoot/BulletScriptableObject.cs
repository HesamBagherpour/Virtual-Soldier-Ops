using UnityEngine;

[CreateAssetMenu(fileName = "Bullet", menuName = "ScriptableObjects/Bullet", order = 1)]
public class BulletScriptableObject : ScriptableObject
{
    public GameObject BulletPrefab;
    public float Damage;
    public float MaxDistance;
    [Header("PhysicForceOnHit based on distance for 1 meter")] 
    //todo: [Header("it will multiply by disance")] 
    public float PhysicForceOnHit;
    public GunType GunType;
}