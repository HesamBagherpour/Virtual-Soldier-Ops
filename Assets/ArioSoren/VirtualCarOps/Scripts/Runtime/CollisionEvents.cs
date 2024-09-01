using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ArioSoren.VirtualCarOps
{
    public class CollisionEvents : MonoBehaviour
    {
        public bool useNames;
        public List<string> targetNames;
        public bool useTags;
        public List<string> targetTags;
        public UnityEvent<Collision> onColEnter, onColStay, onColExit;
        public UnityEvent<Collider> onTrigEnter, onTrigStay, onTrigExit;


        private void OnCollisionEnter(Collision collision)
        {
            if(IsTargetObject(collision.gameObject))
                onColEnter?.Invoke(collision);
        }

        private void OnCollisionStay(Collision collision)
        {
            if (IsTargetObject(collision.gameObject))
                onColStay?.Invoke(collision);
        }

        private void OnCollisionExit(Collision collision)
        {
            if (IsTargetObject(collision.gameObject))
                onColExit?.Invoke(collision);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (IsTargetObject(other.gameObject))
                onTrigEnter?.Invoke(other);
        }

        private void OnTriggerStay(Collider other)
        {
            if (IsTargetObject(other.gameObject))
                onTrigStay?.Invoke(other);
        }

        private void OnTriggerExit(Collider other)
        {
            if (IsTargetObject(other.gameObject))
                onTrigExit?.Invoke(other);
        }

        private bool IsTargetObject(GameObject obj)
        {
        
            if (!useNames && !useTags)
                return true;
            else if(useNames && useTags)
            {
                if (targetNames.Contains(obj.name) && targetTags.Contains(obj.tag))
                    return true;
                else
                    return false;
            }
            else if(useNames)
            {
                if (targetNames.Contains(obj.name))
                    return true;
                else
                    return false;
            }
            else
            {
                if (targetTags.Contains(obj.tag))
                    return true;
                else
                    return false;
            }
        }
    }
}
