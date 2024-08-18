using UnityEngine;
using System.Collections;
using static Gun;

public class MaterialType : MonoBehaviour {

    public MaterialTypeEnum TypeOfMaterial = MaterialTypeEnum.Plaster;

    [System.Serializable]
	public enum MaterialTypeEnum
	{
        Plaster,
	    Metall,
        Folliage,
        Rock,
        Wood,
        Brick,
        Concrete,
        Dirt,
        Glass,
        Water
	}

    public void ShowImpact(HitData data)
    {
        var effect = ImpactManager.Instance.GetImpactEffect(TypeOfMaterial);
        if (effect == null)
            return;
        //var effectIstance = Instantiate(effect, hitPoint.point, new Quaternion()) as GameObject;
        effect.transform.position = data.HitPoint;
        effect.transform.LookAt(data.HitPoint + data.normal);
        //Destroy(effectIstance, 20);

    }
}
