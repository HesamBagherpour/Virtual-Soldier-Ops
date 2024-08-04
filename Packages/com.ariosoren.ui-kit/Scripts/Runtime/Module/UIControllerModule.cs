using System;
using ArioSoren.InjectKit;
using ArioSoren.UIKit.Core;
using Cysharp.Threading.Tasks;

namespace ArioSoren.UIKit.Module
{
    public class UIControllerModule :  MonoModule ,ILoadable
    {
        public override void OnRegister(IContext context)
        {
            UiModule ui = context.Register<UiModule>();
            if (ui != null)
            {
                ui.Load(OnUiLoaded);
                ui.Init();
            }
            // TODO: check for using project scope
            // var gameGameUI = ui.OpenWindow<GameUIManager>();
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
