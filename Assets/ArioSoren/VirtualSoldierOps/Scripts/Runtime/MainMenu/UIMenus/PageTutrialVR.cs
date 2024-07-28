using System;
using ArioSoren.UIKit.Core;
using UnityEngine;
using UnityEngine.UI;

namespace ArioSoren.VirtualSoldierOps.MainMenu.UIMenus
{
    public class PageTutrialVR : PageBaseUI
    {
        public override PageType Type => PageType.Tutorial;
        public override event Action<PageType> OnOpenPage;
        public override event Action OnClosePage;
        [SerializeField] private Button backButton;

  
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
