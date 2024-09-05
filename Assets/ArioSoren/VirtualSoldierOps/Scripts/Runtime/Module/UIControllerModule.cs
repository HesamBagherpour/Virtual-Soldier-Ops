using System;
using System.Collections;
using System.Collections.Generic;
using ArioSoren.InjectKit;
using ArioSoren.UIKit.Core;
using ArioSoren.VirtualSoldierOps.MainMenu.GUI;
using Cysharp.Threading.Tasks;
using UnityEngine;


namespace ArioSoren.VirtualSoldierOps.Module
{
    public class UIControllerModule : MonoModule ,ILoadable
    {
        // Start is called before the first frame update
        public override void OnRegister(IContext context)
        {
            UiModule ui = context.Register<UiModule>();
            if (ui != null)
            {
                ui.Load(OnUiLoaded);
                ui.Init();
            }
            // TODO: check for using project scope
           var gameGameUI = ui.OpenWindow<LoadingWindow>();

        }
        private void OnUiLoaded(IModule obj)
        {
        }

        public UniTask<bool> Load(Action<IModule> onLoaded)
        {
            return new UniTask<bool>(true);
        }

    }
}
