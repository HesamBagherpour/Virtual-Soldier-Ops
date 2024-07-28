using ArioSoren.UIKit.Controllers;
using ArioSoren.UIKit.Core;
using UnityEngine;

namespace ArioSoren.UIKit.Module
{
    public class AnimationController : MonoBehaviour
    {

        public static AnimationController Instance;
        [SerializeField] private UIController uiController;
        private void Awake()
        {
            Instance = this;
        }
        public void HandleAnimation(ComponentType componentType, CurrentAnimationState state, PageType type)
        {
            var page = uiController.pages.Find(t => t.Type == type);
            if (page == null)
                return;
            var component = page.animationComponents.Find(t => t.type == state);

            var failCondition = component == null || state == CurrentAnimationState.None;

            if (failCondition)
                return;
            if (state == CurrentAnimationState.StartAnimation)
            {

                component.Show();
                Debug.Log("start");
                return;
            }

            if (state == CurrentAnimationState.EndAnimation)
            {
                Debug.Log("end");

                component.Close();
            }
        }
    }
}
