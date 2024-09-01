using System;
using ArioSoren.UIKit.Module;
using UnityEngine.UI;

namespace ArioSoren.VirtualCarOps
{
    public class MenuWindow : Window
    {
        // Start is called before the first frame update
        public event Action DriverBtnClicked;
        public event Action BodyguardBtnClicked;
        public event Action PlayBtnClicked;
        public event Action ResetBtnClicked;
        public Button DriverBtn;
        public Button BodyguardBtn;
        public Button PlayBtn;
        public Button ResetBtn;

        public override void Init()
        {
            DriverBtn.onClick.AddListener(Driver);
            BodyguardBtn.onClick.AddListener(BodyGuard);
            PlayBtn.onClick.AddListener(Play);
            ResetBtn.onClick.AddListener(ResetGame);
            base.Init();
        }
        void Driver()
        {
            DriverBtnClicked?.Invoke();
        }

        void BodyGuard()
        {
            BodyguardBtnClicked?.Invoke();
        }

        void Play()
        {
            Close();
            PlayBtnClicked?.Invoke();
        }

        void ResetGame()
        {
            Close();
            ResetBtnClicked?.Invoke();
        }
    }
}
