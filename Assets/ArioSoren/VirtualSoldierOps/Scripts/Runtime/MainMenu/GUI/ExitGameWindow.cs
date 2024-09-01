using System;
using ArioSoren.UIKit.Module;
using UnityEngine.UI;

namespace ArioSoren.VirtualSoldierOps.MainMenu.GUI
{
    public class ExitGameWindow : Window
    {
        public event Action ExitGameBtnClicked;
        public event Action BackToMenuBtnClicked;
        
        public Button confirmExit;
        public Button returnMenu;
        // Start is called before the first frame update
        public override void Init()
        {
            confirmExit.onClick.AddListener(OnExitGame);
            returnMenu.onClick.AddListener(OnBackBtnPressed);
            base.Init();
        }

        private void OnExitGame()
        {
            ExitGameBtnClicked();
        }

        public override void OnBackBtnPressed()
        {
            BackToMenuBtnClicked();

        }
    }
}
