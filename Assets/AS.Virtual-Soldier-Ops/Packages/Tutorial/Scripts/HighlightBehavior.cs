using System.Collections.Generic;
using UnityEngine;

namespace AS.Virtual_Soldier_Ops.Packages.Tutorial.Scripts
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