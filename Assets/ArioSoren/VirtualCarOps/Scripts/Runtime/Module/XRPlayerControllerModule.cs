using System;
using ArioSoren.InjectKit;
using Cysharp.Threading.Tasks;

namespace ArioSoren.VirtualCarOps.Module
{
    public class XRPlayerControllerModule :  MonoModule ,ILoadable
    {
        public void OnRegister(IContext context)
        {
        }
        public UniTask<bool> Load(Action<IModule> onLoaded)
        {

            return new UniTask<bool>(true);
        }
    }
}