using ArioSoren.VirtualCarOps;
using UnityEngine;

namespace PG
{
    public class PlayerRole : MonoBehaviour
    {
        public SyncPlayerPosition syncPosition;

        public enum Role
        {
            Driver = 0,
            Bodyguard = 1
        }

        public Role CurrentRole;
        public Transform DriverPos, BodyguardPos;

        public void SetPlayerRole(Role role)
        {
            Transform xrParent = role == Role.Driver ? DriverPos : BodyguardPos;
            syncPosition.SetPlayerPos(xrParent);
            SteeringWheelInputController.Instance.isEnable = role == Role.Driver;
            CurrentRole = role;
        }
    }
}
