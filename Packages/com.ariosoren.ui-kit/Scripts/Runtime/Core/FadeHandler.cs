using System.Collections;
using UnityEngine;

namespace ArioSoren.UIKit.Core
{
    public class FadeHandler : AnimationBaseUI
    {

        public CanvasGroup canvasGroup;
        [SerializeField] private int target;
        [SerializeField] private float duration;
        [SerializeField] public CurrentAnimationState state;
        [SerializeField] private MoveType moveType;
        public override CurrentAnimationState type => state;
        public override ComponentType componentType => ComponentType.Fade;
        public enum MoveType : byte
        {
            FadeOn = 0,
            FadeOff = 1
        }

        public override void Show()
        {
            FadeON();
        }

        public override void Close()
        {
            FadeOff();
        }

        public float fadeDuration = 1f;


        public IEnumerator FadeOff()
        {
            float elapsedTime = 0f;
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
                Debug.Log(canvasGroup.alpha);
                yield return null;
            }

            canvasGroup.alpha = 0;
        }

        public IEnumerator FadeON()
        {
            float elapsedTime = 0f;
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);
                Debug.Log(canvasGroup.alpha);
                yield return null;
            }

            canvasGroup.alpha = 1;
        }
    }
}
