using FishNet.Object;
using UnityEngine;

namespace UtilityComponents
{
    public class NetworkGameObjectChildActivator : NetworkBehaviour
    {
        [Header("Server Started")] public bool activeOnServerStarted;
        public bool deactiveOnServerStarted;
        public bool destroyOnServerStarted;

        [Header("Server Stop")] public bool activeOnServerStop;
        public bool deactiveOnServerStop;
        public bool destroyOnServerStop;

        [Space(20)] [Header("Client Started")] public bool activeOnClientStarted;
        public bool deactiveOnClientStarted;
        public bool destroyOnClientStarted;


        [Header("Client Stop")] public bool activeOnClientStop;
        public bool deactiveOnClientStop;
        public bool destroyOnClientStop;


        public override void OnStartServer()
        {
            base.OnStartServer();
            if (activeOnServerStarted) ChangeChildrenStatus(true);
            if (deactiveOnServerStarted) ChangeChildrenStatus(false);
            if (destroyOnServerStarted) DestroyChildren();
        }

        public override void OnStopServer()
        {
            base.OnStopServer();
            if (activeOnServerStop) ChangeChildrenStatus(true);
            if (deactiveOnServerStop) ChangeChildrenStatus(false);
            if (destroyOnServerStop) DestroyChildren();
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (activeOnClientStarted) ChangeChildrenStatus(true);
            if (deactiveOnClientStarted) ChangeChildrenStatus(false);
            if (destroyOnClientStarted) DestroyChildren();
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            if (activeOnClientStop) ChangeChildrenStatus(true);
            if (deactiveOnClientStop) ChangeChildrenStatus(false);
            if (destroyOnClientStop) DestroyChildren();
        }

        private void ChangeChildrenStatus(bool state)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(state);
            }
        }

        private void DestroyChildren()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }
    }
}