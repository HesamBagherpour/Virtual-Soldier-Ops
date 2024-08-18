using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ThrowableObject : MonoBehaviour
{
    private XRGrabInteractable grabInteractable;
    private Rigidbody rb;
    public float veclocity;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();
        grabInteractable.throwOnDetach = false;
        grabInteractable.movementType = XRBaseInteractable.MovementType.VelocityTracking;
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    private void OnDestroy()
    {
        grabInteractable.selectExited.RemoveListener(OnRelease);
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        rb.velocity = veclocity * gameObject.transform.forward;
    }
}
