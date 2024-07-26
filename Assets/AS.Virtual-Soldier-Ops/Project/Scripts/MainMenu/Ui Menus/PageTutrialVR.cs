using System;
using AS_Ekbatan_Showdown.Scripts.Core.GUI;
using UnityEngine;
using UnityEngine.UI;

namespace AS_Ekbatan_Showdown.Scripts.MainMenu.Ui_Menus
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
