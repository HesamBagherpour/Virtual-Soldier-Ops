using System;
using AS_Ekbatan_Showdown.Scripts.Core.Module.GUI;
using UnityEngine;
using UnityEngine.UI;

namespace AS_Ekbatan_Showdown.Scripts.MainMenu.GUI
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
