using System;
using UnityEngine;
using UnityEngine.UI;

namespace ArioSoren.VirtualCarOps.MainMenu.GUI
{
    public class MainMenuWindow : Window
    {
        // Start is called before the first frame update
        public event Action SettingBtnClicked;
        public event Action ExitBtnClicked;
        public Button setting;
        public Button exit;
        public override void Init()
        {
            setting.onClick.AddListener(TaskOnClick);
            exit.onClick.AddListener(OnExitGame);
            base.Init();
        }
        void TaskOnClick()
        {
            Close();
            SettingBtnClicked?.Invoke();
        }

        private void OnExitGame()
        {
            //Close();
            ExitBtnClicked?.Invoke();
        }
        private void OnStageSelectorEnter()
        {
            Debug.Log("  StageSelectorEnter : ");
        }
    }
}
 