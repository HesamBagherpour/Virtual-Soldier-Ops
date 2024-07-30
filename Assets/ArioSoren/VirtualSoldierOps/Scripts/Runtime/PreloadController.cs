using System;
using ArioSoren.InjectKit;
using ArioSoren.VirtualSoldierOps.Module;
using ArioSoren.StateMachine;
using ArioSoren.UIKit.Module;
using ArioSoren.VirtualSoldierOps.Conditions;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ArioSoren.VirtualSoldierOps
{
    public class PreloadController : MonoBehaviour
    {

        private Fsm<PreloadController> _fsm;
        private abstract class PreloadState : State<PreloadController>
        {
        }


        #region  InitGame

        private class InitializeGame : PreloadState
        {
            private LoadableContext _context;
            private Action<IModule> _audioController;
            private Action<IModule> _uiController;
            private Action<IModule> _xrController;
            private Action<IModule> _assetLoader;
            private Action<IModule> _onloadUI; 
            private bool _finished;

            protected override async void OnEnter()
            {
                IModuleFactory factory = new ImplicitModuleFactory(new GameObject("_ModuleContainer"), true);
                _context = new LoadableContext(factory);
                
                AssetLoader assetLoader = _context.Register<AssetLoader>();
                AudioModule contextAudioModule= _context.Register<AudioModule>();
                
                UIControllerModule contextUiControllerModule= _context.Register<UIControllerModule>();
                
                var _ui = await InitUI(contextUiControllerModule);

                _finished = _ui;

            }
    
            private async UniTask<bool> InitUI(UIControllerModule  contextUiModule)
            {
   
                contextUiModule.Init();
                contextUiModule.Load(_uiController);
                
                return true;

            }
            protected override void OnUpdate(float deltaTime)
            {
                base.OnUpdate(deltaTime);
                if (_finished)
                    Finished();
            }
            protected override void OnExit()
            {
                base.OnExit();
            }
        }


        #endregion
        #region Network

        private class InitializeNetwork : PreloadState
        {
            private bool _finished;
            protected override async void OnEnter()
            {
                
                Debug.Log("Load Scene Here ");
                _finished = true;
                
            }
            protected override void OnUpdate(float deltaTime)
            {
                base.OnUpdate(deltaTime);
                if (_finished)
                    Finished();
            }
            
            protected override void OnExit()
            {
                base.OnExit();
            }
            
        }


        #endregion
        #region OfflineMode

        private class OfflineMode : PreloadState
        {
            private bool _finished;
            protected override async void OnEnter()
            {
                
                Debug.Log("Load offline Mode ");
                _finished = true;
                
            }
            protected override void OnUpdate(float deltaTime)
            {
                base.OnUpdate(deltaTime);
                if (_finished)
                    Finished();
            }
            
            protected override void OnExit()
            {
                base.OnExit();
            }
            
        }

        

        #endregion
        #region GameScene

        private class GameSceneState : PreloadState
        {
            private bool _finished;
            protected override async void OnEnter()
            {
                _finished = true;
            }
            protected override void OnUpdate(float deltaTime)
            {
                base.OnUpdate(deltaTime);
                if (_finished)
                    Finished();
            }
            
            protected override void OnExit()
            {
                base.OnExit();
            }
            
        }




        #endregion
        #region InitVR

        private class InitializeVR : PreloadState
        {
            private bool _finished;
            protected override async void OnEnter()
            {
                _finished = true;
            }
            protected override void OnUpdate(float deltaTime)
            {
                base.OnUpdate(deltaTime);
                if (_finished)
                    Finished();
            }
            
            protected override void OnExit()
            {
                base.OnExit();
            }
            
        }



        #endregion
        
        
        
        
        #region InitTutorial

        private class InitializeTutorial: PreloadState
        {
            private bool _finished;
            protected override async void OnEnter()
            {
                _finished = true;
            }
            protected override void OnUpdate(float deltaTime)
            {
                base.OnUpdate(deltaTime);
                if (_finished)
                    Finished();
            }
            
            protected override void OnExit()
            {
                base.OnExit();
            }
            
        }



        #endregion
        #region UNITY

            protected void Awake()
            {

                #region condition

                IfOnlineMode ifOnlineMode = new IfOnlineMode();
                IfTutorialMode ifTutorialMode = new IfTutorialMode();


                #endregion

                #region InitClass

                InitializeGame initialState = new InitializeGame { Name = "Init App state" };
                InitializeVR initialVR = new InitializeVR { Name = "Init VR state" };
                InitializeTutorial initialTutorial = new InitializeTutorial { Name = "Init Tutorial  state" };
                InitializeNetwork initializeNetwork = new InitializeNetwork { Name = "Next initialize Network state" };
                GameSceneState gameSceneState = new GameSceneState { Name = "Next Game Scene state" };
                OfflineMode offlineMode = new OfflineMode { Name = "Go to offline  Mode  state" };


                #endregion
                
                #region Transition

                
                Transition.CreateAndAssign(initialState, initializeNetwork);
                
                Transition.CreateAndAssign(initializeNetwork, gameSceneState,offlineMode,ifOnlineMode);
                Transition.CreateAndAssign(initializeNetwork, gameSceneState,offlineMode,ifOnlineMode);
                #endregion

                _fsm = new Fsm<PreloadController>(this, initialState) { Name = "Preload FSM" };
                _fsm.Start();
        
            }       
            private void Update()
            {
                _fsm?.Update(Time.deltaTime);
            }


        #endregion
        

    }
    


}

