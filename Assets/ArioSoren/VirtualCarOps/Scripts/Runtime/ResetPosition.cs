using UnityEngine;

namespace ArioSoren.VirtualCarOps
{
    public class ResetPosition : MonoBehaviour
    {
        public Transform targetObject;
        public bool kinematicOnReset;
        public Rigidbody targetRigid;

        public void ResetPos()
        {
            targetObject.SetPositionAndRotation(transform.position, transform.rotation);
            if (kinematicOnReset)
                targetRigid.isKinematic = true;
        }
    }
}
