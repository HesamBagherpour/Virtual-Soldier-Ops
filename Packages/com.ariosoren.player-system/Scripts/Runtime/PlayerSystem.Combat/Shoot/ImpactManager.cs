using ArioSoren.PlayerSystem.Combat.PlayerSystem.Combat.VR.Gun;
using UnityEngine;
using static ArioSoren.PlayerSystem.Combat.PlayerSystem.Combat.Shoot.MaterialType;

namespace ArioSoren.PlayerSystem.Combat.PlayerSystem.Combat.Shoot
{
    public class ImpactManager : MonoBehaviour
    {
        public static ImpactManager Instance { private set; get; }

        [System.Serializable]
        public class ImpactInfo
        {
            public MaterialType.MaterialTypeEnum MaterialType;
            public ObjectPool Pool;
        }

        public ImpactInfo[] ImpactElemets = new ImpactInfo[0];

        private void Awake()
        {
            if(Instance == null)
                Instance = this;
            else
                Destroy(Instance);
        }


        public GameObject GetImpactEffect(MaterialTypeEnum TypeOfMaterial)
        {
            foreach (var impactInfo in ImpactElemets)
            {
                if (impactInfo.MaterialType == TypeOfMaterial) {
                    GameObject effect = impactInfo.Pool.GiveObject();
                    impactInfo.Pool.TakeObject(effect, 20);
                    return effect;
                }
            }
            return null;
        }
    }
}
