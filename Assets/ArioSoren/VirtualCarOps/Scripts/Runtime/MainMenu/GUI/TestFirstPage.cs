using System;
using UnityEngine.UI;

namespace ArioSoren.VirtualCarOps.MainMenu.GUI
{
    public class TestFirstPage : Window
    {
        public event Action CloseBtnClicked;
        public Button nextPage;

        public override void Init()
        {
            nextPage.onClick.AddListener(TaskOnClick);
            base.Init();
        }

        void TaskOnClick()
        {
            Close();
            CloseBtnClicked?.Invoke();
        }
    }
}
