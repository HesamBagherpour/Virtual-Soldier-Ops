using System;
using AS.Virtual_Soldier_Ops.Packages.UI.Scripts.Core;
using UnityEngine;
using UnityEngine.UI;

namespace AS.Virtual_Soldier_Ops.Project.Scripts.MainMenu.Ui_Menus
{
    public class MainMenu : PageBaseUI
    {
        [Header("Buttons")] [SerializeField] private Button gamePageButton;
        [SerializeField] private Button settingPageButton;
        [SerializeField] private Button exitGameButton;
        [SerializeField] private Button tutorialButton;
        [SerializeField] private Button multiPlayer;


        public override PageType Type => PageType.MainMenu;
        public override event Action<PageType> OnOpenPage;
        public override event Action OnClosePage;

        public override void Init()
        {
            HideRoot();

            settingPageButton.onClick.RemoveAllListeners();
            gamePageButton.onClick.RemoveAllListeners();
            exitGameButton.onClick.RemoveAllListeners();
            tutorialButton.onClick.RemoveAllListeners();
            multiPlayer.onClick.RemoveAllListeners();

            settingPageButton.onClick.AddListener(OpenGameSettings);
            gamePageButton.onClick.AddListener(OpenGamePage);
            exitGameButton.onClick.AddListener(ExitGame);
            tutorialButton.onClick.AddListener(OpenTutorialPage);
            multiPlayer.onClick.AddListener(OpenMultiPlayerPage);
        }


        private void OpenGamePage()
        {
            Debug.Log("this");

            OnOpenPage?.Invoke(PageType.Game);
        }

        private void OpenGameSettings()
        {
            OnOpenPage?.Invoke(PageType.Setting);
        }

        private void OpenTutorialPage()
        {
            OnOpenPage?.Invoke(PageType.Tutorial);
        }

        private void OpenMultiPlayerPage()
        {
            OnOpenPage?.Invoke(PageType.Multiplayer);
        }

        private void ExitGame()
        {
            Application.Quit();
        }
    }
}