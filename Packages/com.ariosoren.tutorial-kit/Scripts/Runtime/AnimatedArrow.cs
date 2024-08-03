using System.Collections.Generic;
using UnityEngine;

namespace ArioSoren.TutorialKit
{
    public class AnimatedArrow : HighlightBehavior
    {
        [SerializeField] private List<GameObject> arrowObject;
        public override void Show()
        {
            foreach (var o in arrowObject)
            {
                o.SetActive(true);
            }
         
        }

        public override void Hide()
        {
            foreach (var o in arrowObject)
            {
                o.SetActive(false);
            }
        }



    }
}