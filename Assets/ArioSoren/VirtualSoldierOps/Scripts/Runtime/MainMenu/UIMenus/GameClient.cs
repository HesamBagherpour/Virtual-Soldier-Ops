using System;
using ArioSoren.VirtualSoldierOps.MainMenu.UIMenus;
using ArioSoren.UIKit.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ArioSoren.VirtualSoldierOps.MainMenu.UIMenus
{
    public class GameClient : PageBaseUI
    {
        [SerializeField] private TMP_InputField _addressInput;
        [SerializeField] private Button _joinButton;
        [SerializeField] private Button _backButton;

        [SerializeField] private NumPadUI numberpad;
        public override PageType Type => PageType.Client;
        public override event Action<PageType> OnOpenPage;
        public override event Action OnClosePage;

        public override void Init()
        {
            HideRoot();
            _joinButton.onClick.AddListener(OnJoinButtonClick);
            _backButton.onClick.AddListener(Back);
            numberpad.Init();
        }

        private void Back()
        {
            OnClosePage?.Invoke();
        }

        private async void OnJoinButtonClick()
        {
            AppUI.Instance.ShowLoading();
            AppUI.Instance.HideLoading();
            OnClosePage?.Invoke();
        }
    }
}