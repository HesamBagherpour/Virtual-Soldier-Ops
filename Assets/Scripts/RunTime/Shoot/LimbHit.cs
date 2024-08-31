using UnityEngine;

public class LimbHit : MonoBehaviour, Idamageable
{
    [SerializeField] private int multiplier;
    Health health;

    void Start()
    {
        health = GetComponentInParent<Health>();
    }

    public virtual void ReceiveDamage(HitData data)
    {
        health.OnReceiveDamage(data, multiplier);
        Debug.Log(transform.name + " : Damage Amount = " + multiplier);
    }
}