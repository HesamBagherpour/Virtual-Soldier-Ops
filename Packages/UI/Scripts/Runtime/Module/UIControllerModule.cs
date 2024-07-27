using System;
using AS_Ekbatan_Showdown.Scripts.Core.DI;
using AS.Virtual_Soldier_Ops.Packages.UI.Scripts.Core;
using Cysharp.Threading.Tasks;

namespace AS.Virtual_Soldier_Ops.Packages.UI.Scripts.Module
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
