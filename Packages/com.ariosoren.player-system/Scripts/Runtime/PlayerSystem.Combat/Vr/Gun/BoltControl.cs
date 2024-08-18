using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Transformers;

public class BoltControl : MonoBehaviour
{
    [SerializeField] HandsOnGunControl handOnGun;
    [SerializeField] GunController gunController;
    [SerializeField] Transform boltAttachPoint;
    [SerializeField] Animator animator;
    [SerializeField, Range(0, 1)] float returnSpeed = 0.1f;
    [Header("Sound")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip pullSound;
    [SerializeField] AudioClip releaseSound;

    public Action<bool> OnBoltPull;
    public Action OnReadyToPull;

    private bool isReturnCoroutineEnded = true;
    private bool _playedPullSound = false;

    void OnTriggerStay(Collider other)
    {
        if (other.transform.tag == "interactor")
        {
            var interactor = other.GetComponent<XRDirectInteractor>();
            if(interactor.hasSelection == false)
            {
                gunController.AllowTakeMagazine(false);
                gunController.SetTwoHandRotationMode(XRGeneralGrabTransformer.TwoHandedRotationMode.FirstHandOnly);
                gunController.SetSecondaryAttachTransform(boltAttachPoint);
                handOnGun.SetSecondHandToBolt();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        TriggerExit();
    }

    void TriggerExit()
    {
        if(! gunController.IsInTwoHandGrab())
        {
            gunController.AllowTakeMagazine(true);
            gunController.SetTwoHandRotationMode(XRGeneralGrabTransformer.TwoHandedRotationMode.FirstHandDirectedTowardsSecondHand);
            gunController.SetDefaultSecondaryAttachTransform();
            handOnGun.SetSecondHandToNormal();
        }
    }

    public void OnGunStateChnged()
    {
        if(! gunController.IsGrabbed())
        {
            TriggerExit();
            StartCoroutine(ReturnToDefault());
        }
    }

    public void MoveBolt(float _value)
    {
        if(! gunController.IsInTwoHandGrab())
        {
            return;
        }

        var value = GetAnimatorValue();

        if(value + _value >= -0.1f && value + _value <= 1.05)
        {
            value += _value;
            SetAnimatorValue(value);
        }

        if(value >= 0.9)
        {
            Pull(true);
        }

        if(value <= 0.3f)
        {
            OnReadyToPull?.Invoke();
        }

        if(value > 0.4 && value < 0.6 && !_playedPullSound)
        {
            audioSource.PlayOneShot(pullSound);
            _playedPullSound = true;
        }
    }

    public void Pull(bool value)
    {
        OnBoltPull?.Invoke(value);
    }

    public void LeaveBolt()
    {
        gunController.AllowTakeMagazine(true);
        StartCoroutine(ReturnToDefault());
    }

    IEnumerator ReturnToDefault()
    {
        if(! isReturnCoroutineEnded)
        {
            yield break;
        }

        isReturnCoroutineEnded = false;

        var value = GetAnimatorValue();
        value = value > 1 ? 1 : value;
        while (value > 0)
        {
            value -= returnSpeed;
            SetAnimatorValue(value);
            yield return new WaitForEndOfFrame();
        }
        SetAnimatorValue(0);

        if (_playedPullSound)
        {
            audioSource.PlayOneShot(releaseSound);
            _playedPullSound = false;
        }

        isReturnCoroutineEnded = true;
    }

    float GetAnimatorValue()
    {
        return animator.GetFloat("Blend");
    }
    void SetAnimatorValue(float value)
    {
        animator.SetFloat("Blend", value);
    }
}