using UnityEngine;

namespace ArioSoren.PlayerSystem.Combat.PlayerSystem.Combat.Shoot
{
    public class Bullet : MonoBehaviour
    {
        // todo this script must change to shoot package 
        public void OnDrop()
        {
            transform.GetChild(0).gameObject.AddComponent<Rigidbody>();
        }
    }
}