using System.Collections;
using UnityEngine;

namespace ArioSoren.VirtualCarOps
{
    public class CanvasRootPosSetter : MonoBehaviour
    {
        public Transform parent;
        public Vector3 localPos;
        public Vector3 localRot;

        private const string canvasRoot = "CanvasRoot(Clone)";

        private void Start()
        {
            StartCoroutine(SetPos());
        }

        IEnumerator SetPos()
        {
            yield return new WaitForEndOfFrame();
            var canvas = GameObject.Find(canvasRoot);
            canvas.transform.parent = parent;
            canvas.transform.SetLocalPositionAndRotation(localPos,Quaternion.Euler(localRot));
            canvas.transform.localScale = Vector3.one;
        }
    }
}
