using UnityEngine;

namespace ArioSoren.TutorialKit
{
    public class CirculateAroundObject : HighlightBehavior
    {
        [SerializeField] private GameObject _inverseMask;
        [SerializeField] private GameObject _actionObject;
    
        public override void Show()
        {
            _inverseMask.SetActive(true);
        }

        public override void Hide()
        {
            _inverseMask.SetActive(false);
        }

   
    }
}