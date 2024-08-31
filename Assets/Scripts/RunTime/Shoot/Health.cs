using RootMotion.FinalIK;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float maxHealth;
    private float currentHealth;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void OnReceiveDamage(HitData data, int multiplier)
    {
        currentHealth -= data.DamageAmount * multiplier;
        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        transform.GetComponent<VRIK>().enabled = false;
        animator.CrossFade("Falling Forward Death", 0.3f);
    }
}