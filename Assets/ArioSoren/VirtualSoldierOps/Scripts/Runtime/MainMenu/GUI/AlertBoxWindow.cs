using System;
using ArioSoren.UIKit.Module;
using UnityEngine.UI;

namespace ArioSoren.VirtualSoldierOps.MainMenu.GUI
{
    public class AlertBoxWindow : Window
    {

        public event Action CloseBtnClicked;
        public Button close;
        public override void Init()
        {
            close.onClick.AddListener(TaskOnClick);
            base.Init();
        }
        private void OnStageSelectorEnter()
        {

        }
        void TaskOnClick()
        {
            Close();
            CloseBtnClicked?.Invoke();
        }
    }
}
