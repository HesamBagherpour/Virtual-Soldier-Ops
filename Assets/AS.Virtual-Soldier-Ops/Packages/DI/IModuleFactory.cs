
namespace AS_Ekbatan_Showdown.Scripts.Core.DI
{
    public interface IModuleFactory
    {
        #region Public Methods

        IModule Create<T>() where T : IModule, new();

        #endregion
    }
}
