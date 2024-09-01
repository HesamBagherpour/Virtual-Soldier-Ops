using System;
using UnityEngine;
using UnityEngine.UI;

using ArioSoren.UIKit.Module;

namespace ArioSoren.VirtualCarOps.MainMenu.GUI
{
    public class SettingWindow : Window
    {
        // Start is called before the first frame update
        public event Action CloseBtnClicked;
        public event Action AudioBtnClicked;
        public event Action GraphicBtnClicked;
        public event Action ControlBtnClicked;
        public event Action VRSpecificBtnClicked;
        public Button close;
        public Button audioWindow;
        public Button graphicWindow;
        public Button controlWindow;
        public Button vrSpecificWindow;
        public override void Init()
        {
            close.onClick.AddListener(TaskOnClick);
            audioWindow.onClick.AddListener(OnAudioWindowOpened);
            graphicWindow.onClick.AddListener(OnGraphicWindowOpened);
            controlWindow.onClick.AddListener(OnControlWindowOpened);
            vrSpecificWindow.onClick.AddListener(OnVRSpecificWindowOpened);
            base.Init();
        }
        void TaskOnClick()
        {
            Close();
            CloseBtnClicked?.Invoke();
        }

        private void OnGraphicWindowOpened()
        {
            Debug.Log("OnGraphicButtonPressed");
            GraphicBtnClicked();
        }

        private void OnAudioWindowOpened()
        {
            Debug.Log("OnAudioWindowOpened");
            AudioBtnClicked();
        }
        private void OnControlWindowOpened()
        {
            Debug.Log("OnControlWindowOpened");
            ControlBtnClicked();
        }
        private void OnVRSpecificWindowOpened()
        {
            Debug.Log("OnVRSpecificWindowOpened");
            VRSpecificBtnClicked();
        }
        private void OnStageSelectorEnter()
        {
            Debug.Log("  StageSelectorEnter : ");
        }
    }
}
