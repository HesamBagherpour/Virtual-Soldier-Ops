using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Transformers;

public class BoltControl : MonoBehaviour
{
    [SerializeField] HandsOnGunControl handOnGun;
    [SerializeField] GunController gunController;
    [SerializeField] Transform boltAttachPoint;
    [SerializeField] Transform points;
    [SerializeField] Animator boltAnimator;
    [SerializeField] Animator bulletAnimator;
    [SerializeField, Range(0, 1)] float moveSpeed = 0.2f;
    [SerializeField, Range(0, 1)] float returnSpeed = 0.1f;
    [Header("Sound")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip pullSound;
    [SerializeField] AudioClip releaseSound;

    Transform front;
    Transform back;
    private bool isFirstSelect = false;
    private bool isReturnCoroutineEnded = true;
    private bool _playedPullSound = false;

    public Action<bool> OnBoltPull;
    public Action OnReadyToPull;

    void Awake()
    {
        front = points.Find("Front");
        back = points.Find("Back");
    }

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

    void CalibratePoints(Vector3 handPos)
    {
        points.position = handPos;
    }

    public void MoveBolt(float _value , Vector3 handPos)
    {
        if(! gunController.IsInTwoHandGrab())
            return;

    //Position Base Calculation 
    
        if(isFirstSelect == false)
        {
            CalibratePoints(handPos);
            isFirstSelect = true;
        }
        var distance = ((handPos - front.position).magnitude - (handPos - back.position).magnitude) * moveSpeed * 25;
        var value = GetAnimatorValue();

        if(value + distance < -0.05f)
            value = 0;
        else if(value + distance > 1.05 )
            value = 1;
        else if(value + distance >= -0.05f && value + distance <= 1.05)
            value += distance;

        SetAnimatorValue(value);
        CalibratePoints(handPos);

        if(value >= 0.9)
        {
            Pull(true);
        }
        else if(value <= 0.2f)
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
        isFirstSelect = false;
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
        return boltAnimator.GetFloat("Blend");
    }
    void SetAnimatorValue(float value)
    {
        boltAnimator.SetFloat("Blend", value);

        if(bulletAnimator != null)
        {
            if (value < 0.9f)
                bulletAnimator.SetFloat("Position", value);
            else if(value > 0.9f)
                bulletAnimator.Play("DropBullet");
        }
    }
}