using TMPro;
using UnityEngine;

public class TargetBoard : MonoBehaviour, Idamageable
{

    [SerializeField] protected float _maxHealth;
    [SerializeField] protected float _currentHealth;
    [SerializeField] protected TMP_Text _UIhealth;


    public virtual void ReceiveDamage(Gun.HitData data)
    {
        //ImpactHandler.GenerateHitImpact(data);
        _currentHealth -= data.DamageAmount;
        if (_UIhealth != null)
            _UIhealth.text = $"{_currentHealth}/{_maxHealth}";
        if (_currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    public virtual void Start()
    {
        _currentHealth = _maxHealth;
        if (_UIhealth != null)
            _UIhealth.text = $"{_currentHealth}/{_maxHealth}";
    }

}
