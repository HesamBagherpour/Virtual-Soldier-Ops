using System;
using AS.Virtual_Soldier_Ops.Packages.UI.Scripts.Module;

namespace AS.Virtual_Soldier_Ops.Project.Scripts.MainMenu.GUI
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