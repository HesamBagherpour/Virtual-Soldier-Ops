using System;
using System.Threading.Tasks;
using AS_Ekbatan_Showdown.Scripts.Core.DI;
using AS_Ekbatan_Showdown.Scripts.Core.Module;
using AS.Virtual_Soldier_Ops.Packages.DI;
using AS.Virtual_Soldier_Ops.Packages.UI.Scripts.Core;
using AS.Virtual_Soldier_Ops.Packages.UI.Scripts.Module;
using UnityEngine;

namespace AS.Virtual_Soldier_Ops.Project.Scripts
{
    public class GameController : MonoBehaviour
    {
        private LoadableContext _context;
        private Action<IModule> _audioModule;
        private Action<IModule> _uiController;
        private Action<IModule> _xrController;
        private Action<IModule> _assetLoader;
        private Action<IModule> _onloadUI; 

            
        private void Awake()
        {
            IModuleFactory factory = new ImplicitModuleFactory(new GameObject("_ModuleContainer"), true);
            _context = new LoadableContext(factory);
            _assetLoader += AssetLoader;
            Init();
        }

        private void AssetLoader(IModule obj)
        {
            Debug.Log(" Scene Load here successfully  ");
        }

        private async Task Init()
        {
            AudioModule contextAudioModule= _context.Register<AudioModule>();
            AssetLoader assetLoader = _context.Register<AssetLoader>();
            UIControllerModule contextUiControllerModule= _context.Register<UIControllerModule>();
            UiModule ui = _context.Register<UiModule>();
            
            
            if (ui != null)
            {
                ui.Load(OnUiLoaded);
                ui.Init();
            }
            assetLoader.Init();
           //  var gameGameUI = ui.OpenWindow<GameUIManager>();
           // contextAudioModule.Load(_audioModule);
           // contextUiControllerModule.Load(_uiController);
           // assetLoader.LoadScene(GameString.Ekbatan,_assetLoader, LoadSceneMode.Single);

        }
        private void OnUiLoaded(IModule module)
        {
                Debug.Log("on ui loaded ");
        }
    }
}
 