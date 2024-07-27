using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace AS.Virtual_Soldier_Ops.Packages.Tutorial
{
    public class TutorialStep : MonoBehaviour
    {
        public int Step;
        [SerializeField] private List<GameObject> _highlightObjects;
        [SerializeField] private HighlightType _highlightTypeName;
        [SerializeField] private RectTransform _dialogueFrame;

        private HighlightBehavior _behaviour;
        public bool Startable;
        public string AdjustEventOnShow;
        public string AdjustEventOnPass;

        public string AnalatycsEventOnShow;
        public string AnalatycsEventOnPass;


        private void Awake()
        {
            _behaviour = GetComponent<HighlightBehavior>();
        }
        public void ShowStep()
        {
            _behaviour.Init(_highlightObjects);

            _behaviour.Show();
            if (_dialogueFrame != null)
            {
                OpenDialogue();
            }
            
        }

        private void OpenDialogue()
        {
            var seq = DOTween.Sequence();
            seq.Append(_dialogueFrame.DOScale(1.3f, 0.3f).From(0).SetEase(Ease.OutBack, 1.2f).SetDelay(0.1f));
        }

        public void HideStep()
        {
            _behaviour.Hide();
            if (_dialogueFrame != null)
            {
                CloseDialogue();
            }
        }

        private void CloseDialogue()
        {
            var seq = DOTween.Sequence();
            seq.Append(_dialogueFrame.DOScale(0, 0.03f).From(1));

        }


    }
}