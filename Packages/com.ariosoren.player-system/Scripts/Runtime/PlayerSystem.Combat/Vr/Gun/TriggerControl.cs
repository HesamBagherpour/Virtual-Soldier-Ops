using System;
using System.Collections;
using UnityEngine;

public class TriggerControl : MonoBehaviour
{
    [SerializeField, Range(0f, 1)] float returnSpeed = 0.05f;
    [SerializeField] ShootingModeControl shootingMode;
    [SerializeField] Animator triggerAnimator;
    Animator fingerAnimator;

    public Action OnTriggerStart;
    public Action OnTriggerEnd;
    bool readyToShoot = true;

    void Start()
    {
        fingerAnimator = GetComponent<Animator>();
    }

    public void OnActionStay(float value)
    {
        fingerAnimator.SetFloat("TriggerValue", value);
        triggerAnimator.SetFloat("Trigger", value);

        if (value > 0.7)
        {
            if (readyToShoot)
            {
                OnTriggerStart?.Invoke();
                readyToShoot = false;
            }
        }
        else if(value < 0.4)
        {
            readyToShoot = true;
        }
    }

    public void OnActionCancle()
    {
        OnTriggerEnd?.Invoke();
        StartCoroutine(ReturnToDefault());
    }

    IEnumerator ReturnToDefault()
    {
        var animationValue = fingerAnimator.GetFloat("TriggerValue");
        while (animationValue > 0)
        {
            yield return new WaitForEndOfFrame();
            animationValue -= returnSpeed;
            fingerAnimator.SetFloat("TriggerValue", animationValue);
            triggerAnimator.SetFloat("Trigger", animationValue);
        }
        fingerAnimator.SetFloat("TriggerValue", 0);
        triggerAnimator.SetFloat("Trigger", 0);
    }

    public void ChangeShootMode(ChangeModeDirection direction)
    {
        shootingMode.ChangeMode(direction);
    }
}