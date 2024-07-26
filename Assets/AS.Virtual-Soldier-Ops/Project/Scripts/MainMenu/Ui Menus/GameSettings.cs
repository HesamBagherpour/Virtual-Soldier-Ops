using System;
using AS_Ekbatan_Showdown.Scripts.Core.GUI;
using UnityEngine;
using UnityEngine.UI;

namespace AS_Ekbatan_Showdown.Scripts.MainMenu.Ui_Menus
{
    public class GameSettings : PageBaseUI
    {
        public bool isFinishedAnim = false;

        [SerializeField] private Button backButton;
        public override PageType Type => PageType.Setting;
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