using System.Collections.Generic;
using ArioSoren.UIKit.Module;
using ArioSoren.VirtualSoldierOps.MainMenu.GUI;
using UnityEngine;

namespace ArioSoren.VirtualSoldierOps
{
    public class GameUIManager : Window
    {
    
        private Stack<Window> _openWindows;
        private AlertBoxWindow _alertBoxWindow;
        private SettingWindow _settingWindow;
        private ExitGameWindow _exitGameWindow;
        private SpatialGameWindow _spatialPanelScrollWindowWindow;
        
        private LoadingWindow _loadingWindow;
        private List<AdReceiverWindow> _adReceivers;
        private AdReceiverWindow _activeReceiver;

        // Start is called before the first frame update
        public override void Init()
        {
            base.Init();
            {
                var rt = GetComponent<RectTransform>();
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;
            }
            _openWindows = new Stack<Window>();
            _adReceivers = new List<AdReceiverWindow>();
            _alertBoxWindow = GetModal<AlertBoxWindow>();
            _loadingWindow = GetModal<LoadingWindow>();
            _settingWindow = GetModal<SettingWindow>();
            _exitGameWindow = GetModal<ExitGameWindow>();
            _spatialPanelScrollWindowWindow = GetModal<SpatialGameWindow>();
            
            _alertBoxWindow.CloseBtnClicked += AlertBox_Closed;
            _settingWindow.CloseBtnClicked += setting_Closed;
            OpenAlertBox();
            
        }
        
        private void OpenSettingMenu()
        {
            _alertBoxWindow.Open();
            PushTopMostWindow(_settingWindow);
        }

        
        private void PushTopMostWindow(Window window)
        {
            _openWindows.Push(window);
        }

        private void OpenAlertBox()
        {
            _alertBoxWindow.Open();
            PushTopMostWindow(_alertBoxWindow);
        }
        
        public void PopTopMostWindow()
        {
            if (_openWindows.Count > 1)
                _openWindows.Pop();

        }

        private void CloseTopMostWindow()
        {
            if (_openWindows.Count < 1)
            {
                _exitGameWindow.Open();
                PushTopMostWindow(_exitGameWindow);
                return;
            }

            var topMost = _openWindows.Peek();


            if (!topMost.enabled)
            {
                _openWindows.Pop();
                CloseTopMostWindow();
            }
            else
            {
                topMost.OnBackBtnPressed();
            }
        }
        private void setting_Closed()
        {
            PopTopMostWindow();
            _alertBoxWindow.Open();
        }
        
        private void AlertBox_Closed()
        {
            PopTopMostWindow();
            //_settingWindow.Open();
            _spatialPanelScrollWindowWindow.Open();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseTopMostWindow();
            }
        }

    
    }
}
