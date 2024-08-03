using System.Collections.Generic;
using UnityEngine;

namespace ArioSoren.TutorialKit
{
    public abstract class HighlightBehavior : MonoBehaviour
    {

        private List<GameObject> _highlightObjects;


        public void Init(List<GameObject> objects)
        {
            _highlightObjects = objects;
        }

        public abstract void Show();
        public abstract void Hide();


    }
}