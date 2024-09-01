using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace ArioSoren.VirtualCarOps.Controllers
{
    public class GameUIManager : Window
    {
        private Stack<Window> _openWindows;
        private WelcomeWindow _welcomeWindow;
        private MenuWindow _menuWindow;

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
            _welcomeWindow = GetModal<WelcomeWindow>();
            _menuWindow = GetModal<MenuWindow>();

            _welcomeWindow.ProceedBtnClicked += OpenMenuWindow;
            _menuWindow.DriverBtnClicked += PG.GameController.Instance.SetPlayerTypeToDriver;
            _menuWindow.BodyguardBtnClicked += PG.GameController.Instance.SetPlayerTypeToBodyguard;
            _menuWindow.PlayBtnClicked += PG.GameController.Instance.StartGame;
            _menuWindow.ResetBtnClicked += PG.GameController.Instance.ResetGame;
            _menuWindow.PlayBtnClicked += CloseTopMostWindow;
            _menuWindow.ResetBtnClicked += CloseTopMostWindow;

            OpenWelcomePage();
        }

        private void OpenWelcomePage()
        {
            _welcomeWindow.Open();
            PushTopMostWindow(_welcomeWindow);
        }

        private void OpenMenuWindow()
        {
            _welcomeWindow.Close();
            PopTopMostWindow();
            _menuWindow.Open();
            PushTopMostWindow(_menuWindow);
        }

        //private async UniTaskVoid OpenGraphicWindowAsync()
        //{
        //    if (_graphicSettingOptionsWindow.gameObject.activeSelf)
        //    {
        //        return;
        //    }
        //    _settingWindow.graphicWindow.interactable = false;
        //    _settingWindow.controlWindow.interactable = false;
        //    _settingWindow.vrSpecificWindow.interactable = false;
        //    _settingWindow.audioWindow.interactable = false;
        //    _audioSettingOptionsWindow.Close();
        //    _audioSettingValuesWindow.Close();
        //    _audioSettingDescriptionsWindow.Close();
        //    _controlSettingOptionsWindow.Close();
        //    _controlSettingValuesWindow.Close();
        //    _controlSettingDescriptionWindow.Close();
        //    _vrSpecificSettingOptionsWindow.Close();
        //    _vrSpecificSettingValuesWindow.Close();
        //    _vrSpecificSettingDescriptionsWindow.Close();
        //   // await UniTask.WaitForSeconds(1f);

        //    PopTopMostWindow();

        //    _graphicSettingOptionsWindow.Open();
        //    _graphicSettingValuesWindow.Open();
        //    _graphicSettingDescriptionWindow.Open();
        //    PushTopMostWindow(_graphicSettingOptionsWindow);
        //    PushTopMostWindow(_graphicSettingValuesWindow);
        //    PushTopMostWindow(_graphicSettingDescriptionWindow);
        //    _settingWindow.graphicWindow.interactable = true;
        //    _settingWindow.controlWindow.interactable = true;
        //    _settingWindow.vrSpecificWindow.interactable = true;
        //    _settingWindow.audioWindow.interactable = true;
        //}

        private void KillTweens()
        {
            DOTween.KillAll(true);
        }

        private void PushTopMostWindow(Window window)
        {
            _openWindows.Push(window);
        }

        public void PopTopMostWindow()
        {
            if (_openWindows.Count > 1)
                _openWindows.Pop();
        }

        private void CloseTopMostWindow()
        {
            while(_openWindows.Count > 0)
            {
                _openWindows.Pop().Close();
            }
        }
    }
}
