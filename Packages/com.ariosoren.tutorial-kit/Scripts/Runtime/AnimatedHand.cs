using DG.Tweening;
using UnityEngine;

namespace ArioSoren.TutorialKit
{
    public class AnimatedHand : HighlightBehavior
    {
        [SerializeField] private Vector2 _startPos, _endPos;
        [SerializeField] private RectTransform _handImage;

        [SerializeField] private GameObject _blocks;
        [SerializeField] private GameObject _dialog;

        private void Awake()
        {
        //    Show();
        }
        public override void Show()
        {
            _handImage.gameObject.SetActive(true);
            _blocks.SetActive(true);
            _dialog.SetActive(true);
            var seq = DOTween.Sequence();

            seq.Append(_handImage.DOAnchorPos(_endPos, 1.5f).SetEase(Ease.InOutSine).From(_startPos)).SetLoops(-1);
        }

        public override void Hide()
        {
           _handImage.gameObject.SetActive(false);
           _blocks.SetActive(false);
            _dialog.SetActive(false);

        }
    }
}