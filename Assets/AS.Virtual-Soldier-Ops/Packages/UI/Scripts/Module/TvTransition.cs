using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace AS.Virtual_Soldier_Ops.Packages.UI.Scripts.Module
{
    public class TvTransition : UiTransition
    {
        [SerializeField] private TransitionAction _transitionAction;
        [SerializeField] private float _tweenTime = 0.4f;
        [SerializeField] private Image _darkBg;


        protected override void OnApply()
        {

            if (_transitionAction == TransitionAction._close)
                CloseSeq();
            else
                OpenSeq();
        }

        private void CloseSeq()
        {
            var seq = DOTween.Sequence();
            seq.Append(_darkBg?.DOFade(0, _tweenTime / 2)).Append(rectTransform.DOScaleY(0.02f, _tweenTime)).Append(rectTransform.DOScaleX(0f, _tweenTime)).OnComplete(OnComplete);

        }

        private void OpenSeq()
        {
            var seq = DOTween.Sequence();
            seq.Append(rectTransform.DOScaleX(1f, _tweenTime)).Append(rectTransform.DOScaleY(1f, _tweenTime)).Append(_darkBg?.DOFade(0.7f, _tweenTime / 2)).OnComplete(OnComplete);
        }
    }
    public enum TransitionAction
    {
        _open,
        _close
    }
}