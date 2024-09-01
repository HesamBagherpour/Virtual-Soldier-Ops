using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ArioSoren.VirtualCarOps.MainMenu.GUI
{
    public class TrainButtonWindow : Window, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Window hoverableWindow;// choose better name
        private Stack<Window> _openWindows;
        //private bool isOpening;
        //private bool isClosing;غ

        public override void Init()
        {
            base.Init();
        
            Opened += IsOpeningFalse;
            Closed += IsClosingFalse;
            _openWindows = new Stack<Window>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            /*if (isClosing || isOpening)
        {
            //Delay();
            return;
        }*/
            if (!DOTween.IsTweening(hoverableWindow))
            {
                DOTween.KillAll(true);

                hoverableWindow.Open();
                PushTopMostWindow(hoverableWindow);
            }
            else
            {
                DOTween.KillAll(true);
            }
            //OnOpened();
            //isOpening = true;

        }

        public void OnPointerExit(PointerEventData eventData)
        {
            /*if (isOpening || isClosing)
        {
            //Delay();
            return;
        }*/
            if (!DOTween.IsTweening(hoverableWindow))
            {
                DOTween.KillAll(true);

                hoverableWindow.Close();
                PopTopMostWindow();

            }
            else
            {
                DOTween.KillAll(true);
            }
            //OnClosed();
            //isClosing = true;

        }

        private void IsOpeningFalse(UiWidget uiWidget)
        {
            //isOpening = false;
        } 
        private void IsClosingFalse(UiWidget uiWidget)
        {
            //isClosing = false;
        }
        private async UniTaskVoid Delay()
        {
            await UniTask.WaitForSeconds(.5f);
        }
        public void PopTopMostWindow()
        {
            if (_openWindows.Count > 1)
                _openWindows.Pop();

        }
        private void PushTopMostWindow(Window window)
        {
            _openWindows.Push(window);
        }
    }
}
