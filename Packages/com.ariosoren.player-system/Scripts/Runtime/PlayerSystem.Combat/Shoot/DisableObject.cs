using System;
using System.Collections;
using UnityEngine;

public class DisableObject : MonoBehaviour
{
    public void Initialize(float LifeTime, Action onfinish)
    {
        StartCoroutine(ReleaseImpact(gameObject, LifeTime, onfinish));
    }
    private IEnumerator ReleaseImpact(GameObject impact, float delay, Action Onfinish)
    {
        yield return new WaitForSeconds(delay);
        Onfinish.Invoke();
    }
}
