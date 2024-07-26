using System;
using AS_Ekbatan_Showdown.Scripts.Core.Module.GUI;

namespace AS_Ekbatan_Showdown.Scripts.MainMenu.GUI
{
    public abstract class AdReceiverWindow : Window
    {

        public event Action<AdReceiverWindow> SeeAdsBtnClicked;
        public abstract void RewardConfirmed();

        public virtual void VideoAvailable(bool isAvailable, int lastMatch3LevelWon)
        {
        }

        protected void InvokeAdsBtnClick()
        {

            SeeAdsBtnClicked?.Invoke(this);
        }
        
        
    }
}