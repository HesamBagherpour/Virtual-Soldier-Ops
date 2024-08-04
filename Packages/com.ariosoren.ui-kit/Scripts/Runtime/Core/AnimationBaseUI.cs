using System;
using UnityEngine;

namespace ArioSoren.UIKit.Core
{
    public abstract class AnimationBaseUI : MonoBehaviour
    {
        public RectTransform root;
        public abstract CurrentAnimationState type { get; }
        public abstract ComponentType componentType { get; }
        public abstract void Show();
        public abstract void Close();
        public bool isFinished = false;

    }

    [Serializable]
    public enum ComponentType : byte
    {
        Move = 0,
        Scale = 1,
        Fade = 2
    }

    public enum CurrentAnimationState : byte
    {
        None = 0,
        StartAnimation = 1,
        EndAnimation = 2
    }
}