using System;
using ArioSoren.PlayerSystem.Combat.PlayerSystem.Combat.VR.Player;
using UnityEngine;

namespace ArioSoren.PlayerSystem.Combat.PlayerSystem.Combat.Shoot
{
    public class HandsAnimation : MonoBehaviour
    {
        Animator animator;
        string grabParameter;
        string pinchParameter;

        void Start()
        {
            animator = transform.GetComponent<Animator>();
        }

        public void Grab(PlayerHand hand, float value)
        {
            grabParameter = hand == PlayerHand.Left? "LHand_Grab" : "RHand_Grab";
            ChangeValue(grabParameter, value);
        }

        public void Pinch(PlayerHand hand, float value)
        {
            pinchParameter = hand == PlayerHand.Left? "LHand_Pinch" : "RHand_Pinch";
            ChangeValue(pinchParameter, value);
        }

        void ChangeValue(String name, float value)
        {
            animator.SetFloat(name, value);
        }
    }
}