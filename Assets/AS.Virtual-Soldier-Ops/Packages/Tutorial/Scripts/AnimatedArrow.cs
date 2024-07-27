using System.Collections.Generic;
using UnityEngine;

namespace AS.Virtual_Soldier_Ops.Packages.Tutorial.Scripts
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