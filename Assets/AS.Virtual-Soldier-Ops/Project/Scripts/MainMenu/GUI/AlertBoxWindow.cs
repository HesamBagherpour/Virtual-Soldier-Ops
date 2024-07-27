using System;
using AS.Virtual_Soldier_Ops.Packages.UI.Scripts.Module;
using UnityEngine.UI;

namespace AS.Virtual_Soldier_Ops.Project.Scripts.MainMenu.GUI
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
