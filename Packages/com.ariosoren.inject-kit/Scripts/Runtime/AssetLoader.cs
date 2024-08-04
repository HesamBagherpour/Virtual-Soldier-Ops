using System;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace ArioSoren.InjectKit {

public class AssetLoader : Module, ILoadable
{
    private ILoadable _loadableImplementation;

    public override void Init()
    {

    }
    public UniTask<bool> Load(Action<IModule> onLoaded)
    {
        _loadableImplementation.Load(onLoaded);
        return new UniTask<bool>(true);
    }
    public void LoadScene(String key, Action<IModule> loadScene, LoadSceneMode mode = LoadSceneMode.Single)
    {
        //Todo  must be static class to load differently 
        SceneManager.LoadSceneAsync(key, mode);
        loadScene.Invoke(this);
    } 
}

}