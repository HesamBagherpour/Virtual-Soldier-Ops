using System;
using AS.Virtual_Soldier_Ops.Packages.UI.Scripts.Core;
using UnityEngine;
using UnityEngine.UI;

namespace AS.Virtual_Soldier_Ops.Project.Scripts.MainMenu.Ui_Menus
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
