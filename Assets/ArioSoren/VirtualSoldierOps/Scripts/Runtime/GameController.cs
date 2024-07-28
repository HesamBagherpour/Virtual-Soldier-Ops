using System;
using System.Threading.Tasks;
using ArioSoren.InjectKit;
using ArioSoren.VirtualSoldierOps.Module;
using ArioSoren.UIKit.Core;
using ArioSoren.UIKit.Module;
using UnityEngine;

namespace ArioSoren.VirtualSoldierOps
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
 