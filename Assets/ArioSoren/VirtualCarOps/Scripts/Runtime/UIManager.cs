using PG;
using UnityEngine;

namespace ArioSoren.VirtualCarOps
{
    public class UIManager : MonoBehaviour
    {
        public GameObject resetPage,firstPage, rolePage;

        private bool menuIsOpen = false;

        private void Start()
        {
            firstPage.SetActive(true);
            rolePage.SetActive(false);
            menuIsOpen = true;
            SteeringWheelInputController.Instance.GetButton(SteeringWheelKeyCode.OPTION).started.AddListener(ToggleMenu);
        }

        private void ToggleMenu()
        {
            SetActiveMenu(!menuIsOpen);
        }

        public void SetActiveMenu(bool active)
        {
            firstPage.SetActive(false);
            rolePage.SetActive(active);
            menuIsOpen = active;
        
            if(active)
                PG.GameController.Instance.PauseGame();
            else 
                PG.GameController.Instance.ResumeGame();
        }
    }
}
