using UnityEngine;

namespace ArioSoren.PlayerSystem.Combat.PlayerSystem.Combat.Shoot
{
    [CreateAssetMenu(fileName = "Impact", menuName = "ScriptableObjects/ImpactDatabase", order = 1)]
    public class ImpactList : ScriptableObject
    {
        public GameObject bulletImpactPrefab;
        public GameObject HitVfxPrefab;
    }
}
