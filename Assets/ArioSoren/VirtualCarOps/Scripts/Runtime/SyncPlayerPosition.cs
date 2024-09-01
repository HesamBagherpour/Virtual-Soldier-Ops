using UnityEngine;

namespace ArioSoren.VirtualCarOps
{
    public class SyncPlayerPosition : MonoBehaviour
    {
        public XROrigin xrOrigin;
        public float minimumHeight;

        private Vector3 xrLocalPostion;
        private Vector3 xrLocalRotation;

        private void Start()
        {
            Invoke(nameof(SyncXRPosition), 1);
        }

        //Initial XR position to match the prefered position
        private void SyncXRPosition()
        {
            //Reset the position
            xrOrigin.transform.localPosition = Vector3.zero;
            xrOrigin.transform.localEulerAngles = Vector3.zero;

            //Rotate XRorigin around Y according to camera rotation
            xrLocalRotation.y = -xrOrigin.Camera.transform.localEulerAngles.y;
            xrOrigin.transform.localEulerAngles = xrLocalRotation;

            //Move XRorigin by the inverse of camera distance from XRorigin
            var cameraPos = xrOrigin.Camera.transform.position;
            var xrPos = xrOrigin.transform.position;
            cameraPos.y = 0; //ignore height
            xrPos.y = 0; //ignore height
            var posOffset = xrPos - cameraPos;
            xrOrigin.transform.position += posOffset;

            //Save position 
            xrLocalPostion = xrOrigin.transform.localPosition;

            //Set minimum height to it
            xrLocalPostion.y = minimumHeight - xrOrigin.Camera.transform.localPosition.y;

            //assign again to set height
            xrOrigin.transform.localPosition = xrLocalPostion;
        }

        public void SetPlayerPos(Transform parent)
        {
            xrOrigin.transform.SetParent(parent);
            xrOrigin.transform.localPosition = xrLocalPostion;
            xrOrigin.transform.localEulerAngles = xrLocalRotation;
        }
    }
}
