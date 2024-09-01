using UnityEngine;

namespace ArioSoren.PlayerSystem.Combat.PlayerSystem.Combat.Shoot
{
    public class PhysicsTarget : MonoBehaviour
    {
        private Rigidbody _rigidbody;

        public void ReceiveDamage(Gun.HitData data)
        {
            if (_rigidbody == null)
            {
                _rigidbody = GetComponent<Rigidbody>();
            }
            _rigidbody.AddForceAtPosition(data.HitForce * data.normal*-1, data.collide.transform.position);
        }
    
        public void Start()
        {
            if (_rigidbody == null)
            {
                _rigidbody = GetComponent<Rigidbody>();
            }
        }
    }
}