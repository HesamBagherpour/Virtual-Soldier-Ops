using UnityEngine;

public class LimbHit : MonoBehaviour
{
    [SerializeField] private int multiplier;
    Health health;

    void Start()
    {
        health = GetComponentInParent<Health>();
    }

    public virtual void ReceiveDamage(Gun.HitData data)
    {
        health.OnReceiveDamage(data, multiplier);
        Debug.Log(transform.name + " : Damage Amount = " + multiplier);
    }
}