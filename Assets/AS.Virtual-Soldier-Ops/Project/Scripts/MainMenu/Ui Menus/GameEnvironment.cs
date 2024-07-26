using System;
using AS_Ekbatan_Showdown.Scripts.Core.GUI;
using UnityEngine;
using UnityEngine.UI;

namespace AS_Ekbatan_Showdown.Scripts.MainMenu.Ui_Menus
{
    public class GameEnvironment : PageBaseUI
    {
        [SerializeField] private Button backButton;
        [SerializeField] private Button serverButton;
        [SerializeField] private Button clientButton;
        [SerializeField] private Vector3 initScaleState;
        public bool isFinishedAnim = false;
        public override PageType Type => PageType.Game;
        public override event Action<PageType> OnOpenPage;
        public override event Action OnClosePage;

        public override void Init()
        {
            HideRoot();
          //  serverButton.onClick.AddListener(OnServerButtonClick);
            backButton.onClick.AddListener(Back);
            clientButton.onClick.AddListener(GameClient);
            root.transform.localScale = initScaleState;
        }

        public void Back()
        {
            Debug.Log("this back");
            OnClosePage?.Invoke();
        }

        private void GameClient()
        {
            OnOpenPage?.Invoke(PageType.Client);
        }
        private async void OnServerButtonClick()
        {
          //  GameManager.Instance.NetworkController.ConnectServer();
            //var result = await GameManager.Instance.NetworkController.ConnectClient();

            // if (result.IsSuccess)
            //     UIController.Instance.CloseAllPages();
        }
    }
}