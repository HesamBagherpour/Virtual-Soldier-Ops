using System;
using System.Collections;
using System.Collections.Generic;
using ArioSoren.InjectKit;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ArioSoren.VirtualSoldierOps.Module
{
    public class MultiplayerModule : MonoModule ,ILoadable
    {
        public UniTask<bool> Load(Action<IModule> onLoaded)
        {
            Debug.Log(" Load Audio Module ");
            //GameObject go = Resources.Load<GameObject>("Fishnet Controller ");
            //  = Instantiate(go).GetComponent<Fishnet Controller>();
            // DontDestroyOnLoad(Fishnet Controller);
            return new UniTask<bool>(true);
        }
        public override void OnRegister(IContext context)
        {

        }
        
        public void init()
        {
            
            Debug.Log(" init  fishnet  ");
        }
        

    }
}
