using UnityEngine;

[CreateAssetMenu(fileName = "Impact", menuName = "ScriptableObjects/ImpactDatabase", order = 1)]
public class ImpactList : ScriptableObject
{
    public GameObject bulletImpactPrefab;
    public GameObject HitVfxPrefab;
}
