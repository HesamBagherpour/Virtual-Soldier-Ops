namespace AS_Ekbatan_Showdown.Scripts.Core.DI
{
    public class Module : IModule
    {
        #region IModule Interface
        

        public virtual void OnRegister(IContext context)
        {
        }

        public virtual void Init()
        {

        }

        public override string ToString()
        {
            
            return GetType().Name;
        }

        #endregion
  
    }
}
