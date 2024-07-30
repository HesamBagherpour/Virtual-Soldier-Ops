using UnityEngine;
using UnityEngine.Events;

namespace ArioSoren.UIKit.Core
{
    public class AppUI : MonoBehaviour
    {
        public static AppUI Instance;
        [Header("Self Properties")] 
        [SerializeField] private Canvas rootCanvas;
        [SerializeField] private GameObject root;

        [Header("Child Properties")] 
        [SerializeField] private Canvas loadingOverlay;
        [SerializeField] private PopupUI popupUI;

        public void Awake()
        {
            Instance = this;
            Init();
        }
        public void Init() 
        {
            loadingOverlay.enabled = false;
            popupUI.Init();
        }
        public void ShowMessage(string message)=> popupUI.ShowMessage(message);
        public void ShowPrompt(string message, UnityAction customAction) => popupUI.ShowMessage(message, customAction);
        public void ShowLoading() => loadingOverlay.enabled = true;
        public void HideLoading() => loadingOverlay.enabled = false;
        public void HandleLoading(bool condition) => loadingOverlay.enabled = condition;
    }
}