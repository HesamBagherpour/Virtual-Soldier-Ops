using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ArioSoren.UIKit.Core
{
    public class PopupUI : MonoBehaviour
    {
        public static PopupUI Instance;

        public Canvas rootCanvas;
        public TextMeshProUGUI body;
        public Button actionButton;
        public Button okayButton;

        private void Awake()
        {
            Instance = this;
        }

        public void Init()
        {
            Hide();
            okayButton.onClick.RemoveAllListeners();
            actionButton.onClick.RemoveAllListeners();
            okayButton.onClick.AddListener(Hide);
        }

        public void ShowMessage(string message, UnityAction newAction = null)
        {
            rootCanvas.enabled = true;
            body.text = message;
            actionButton.gameObject.SetActive(newAction != null);
            if (newAction == null)
                return;
            print(newAction);
            actionButton.onClick.RemoveAllListeners();
            actionButton.onClick.AddListener(newAction);
        }

        private void Hide()
        {
            rootCanvas.enabled = false;
            actionButton.gameObject.SetActive(false);
            actionButton.onClick.RemoveAllListeners();
            body.text = "???";
        }
    }
}