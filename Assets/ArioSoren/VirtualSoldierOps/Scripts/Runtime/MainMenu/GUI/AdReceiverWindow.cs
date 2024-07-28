using System;
using ArioSoren.UIKit.Module;

namespace ArioSoren.VirtualSoldierOps.MainMenu.GUI
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