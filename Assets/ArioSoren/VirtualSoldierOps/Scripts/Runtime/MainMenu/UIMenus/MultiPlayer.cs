using System;
using ArioSoren.UIKit.Core;
using UnityEngine;
using UnityEngine.UI;

namespace ArioSoren.VirtualSoldierOps.MainMenu.UIMenus
{
    public class MultiPlayer : PageBaseUI
    {
        public bool isFinishedAnim = false;

        [SerializeField] private Button backButton;
        public override PageType Type => PageType.Multiplayer;
        public override event Action<PageType> OnOpenPage;
        public override event Action OnClosePage;

        public override void Init()
        {
            HideRoot();
            backButton.onClick.AddListener(Back);
        }

        private void Back()
        {
            OnClosePage?.Invoke();
        }
    }
}