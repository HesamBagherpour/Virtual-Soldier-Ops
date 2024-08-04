using System;
using ArioSoren.InjectKit;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ArioSoren.VirtualSoldierOps.Module
{
    public class AudioModule : MonoModule ,ILoadable
    {

        public string test;
        public static void MuteAll()
        {
            AudioListener.pause = true;
        }

        public static void UnMuteAll()
        {
            AudioListener.pause = false;
        }

        private string _loader;
        public override void OnRegister(IContext context)
        {

        }

        public void SoundOn()
        {
            
            Debug.Log(" Sound On ");
        }

        public void soundOff()
        {
            Debug.Log(" Sound Off ");
        }
            
        public UniTask<bool> Load(Action<IModule> onLoaded)
        {
            Debug.Log(" Load Audio Module ");
            //GameObject go = Resources.Load<GameObject>("AudioPlayer");
            // _audioPlayer = Instantiate(go).GetComponent<AudioPlayer>();
            // DontDestroyOnLoad(_audioPlayer);
            return new UniTask<bool>(true);
        }
    }


}
