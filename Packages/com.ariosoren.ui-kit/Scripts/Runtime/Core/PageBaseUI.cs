using System;
using System.Collections.Generic;
using ArioSoren.UIKit.Module;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ArioSoren.UIKit.Core
{
    public abstract class PageBaseUI : MonoBehaviour
    {
        public abstract PageType Type { get; }
        [Header("Page Properties")] public Canvas rootCanvas;
        public GameObject root;

        [Header("Animation Properties")] public Vector3 defaultScale;
        public Vector3 defaultPosition;
        public List<AnimationBaseUI> animationComponents = new();
        public abstract event Action<PageType> OnOpenPage;
        public  abstract event Action  OnClosePage;

        public abstract void Init();

        public async void Show()
        {
            rootCanvas.enabled = true;
            var startComponent = animationComponents.Find(t => t.type == CurrentAnimationState.StartAnimation);
            if (startComponent == null)
                return;
            AnimationController.Instance.HandleAnimation(startComponent.componentType, startComponent.type, Type);
            await UniTask.WaitUntil(() => startComponent.isFinished);
            startComponent.isFinished = false;
            
        }

        public async void Hide()
        {
            var endComponent = animationComponents.Find(t => t.type == CurrentAnimationState.EndAnimation);
            print(animationComponents.Count);

            if (endComponent == null)
            {
                HideRoot();
                return;
            }

            print(endComponent.componentType + "    " + endComponent.type);
            AnimationController.Instance.HandleAnimation(endComponent.componentType, CurrentAnimationState.EndAnimation, Type);
            await UniTask.WaitUntil(() => endComponent.isFinished);
            endComponent.isFinished = false;
            print("Animation ended");
            ResetAnimation();
            HideRoot();
        }

        public void ResetAnimation()
        {
          //  root.transform.localScale = defaultScale;
          //  root.transform.localPosition = defaultPosition;
        }

        public void HideRoot()
        {
            rootCanvas.enabled = false;
        }
    }


    public enum PageType
    {
        None = 0,
        MainMenu = 1,
        Setting = 2,
        Game = 3,
        Client = 4,
        Tutorial = 5,
        Multiplayer = 6
    }
}