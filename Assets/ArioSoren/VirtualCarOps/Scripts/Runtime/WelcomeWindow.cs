using System;
using UnityEngine.UI;

namespace ArioSoren.VirtualCarOps
{
    public class WelcomeWindow : Window
    {
        // Start is called before the first frame update
        public event Action ProceedBtnClicked;
        public Button ProceedBtn;

        public override void Init()
        {
            ProceedBtn.onClick.AddListener(Proceed);
            base.Init();
        }
        void Proceed()
        {
            Close();
            ProceedBtnClicked?.Invoke();
        }
    }
}
