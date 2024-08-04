using UnityEngine;

namespace ArioSoren.VirtualSoldierOps.MainMenu.UIMenus
{
    public class MainMenuUI : MonoBehaviour
    {
        public static MainMenuUI Instance;
        public Canvas rootCanvas;
        public Transform target;
        public Vector3 positionToOpen;

        private void Awake()
        {
            Instance = this;
        }

        public void Init(Transform _target)
        {
            rootCanvas.enabled = false;
            target = _target;
        }

        public void Show()
        {
            rootCanvas.transform.position = target.position + positionToOpen;
            rootCanvas.enabled = true;
        }

        public void Hide()
        {
            rootCanvas.enabled = false;
        }

        public bool IsActive() => rootCanvas.enabled;
    }
}
