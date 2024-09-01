using UnityEngine;

namespace ArioSoren.VirtualCarOps.General_Utility
{
    public class LookAtPlayerUI : MonoBehaviour
    {
        public Transform player;

        public void Init(Transform target)
        {
            player = target;
        }
    
        public void LateUpdate()
        {
            if(!player)
                return;
            transform.LookAt(player);
        }
    }
}
