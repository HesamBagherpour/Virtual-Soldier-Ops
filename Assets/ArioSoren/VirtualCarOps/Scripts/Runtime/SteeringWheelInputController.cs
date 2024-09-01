using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace ArioSoren.VirtualCarOps
{
    [System.Serializable]
    public class SteeringWheelButtonModel
    {
        public SteeringWheelKeyCode inputName;
        public UnityEvent started;
        public UnityEvent performed;
        public UnityEvent released;
    }
    [System.Serializable]
    public class SteeringWheelDirectionModel
    {
        public SteeringWheelKeyCode inputName;
        public UnityEvent onTrigger;
    }

    public class SteeringWheelInputController : MonoBehaviour
    {
        public static SteeringWheelInputController Instance;

        public bool isEnable = true;
        public UnityEvent<float> onSteeringWheelRotate;
        public UnityEvent<float> onGasPressed;
        public UnityEvent<float> onBrakePressed;
        public UnityEvent<float> onClutchPressed;
        public List<SteeringWheelButtonModel> buttonsList;
        public List<SteeringWheelDirectionModel> directionList;

        private LogitechGSDK.DIJOYSTATE2ENGINES rec;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
            {
                Destroy(this);
            }
        }

        private void Start()
        {
            Debug.Log("SteeringInit:" + LogitechGSDK.LogiSteeringInitialize(false));
        }
    
        void OnApplicationQuit()
        {
            Debug.Log("SteeringShutdown:" + LogitechGSDK.LogiSteeringShutdown());
        }

        private void Update()
        {
            if (!isEnable)
                return;

            LogitechGSDK.LogiUpdate();
            CheckAxis();
            CheckButtons();
            CheckDirection();
        }

    
        private void CheckAxis()
        {
            rec = LogitechGSDK.LogiGetStateUnity(0);

            onSteeringWheelRotate?.Invoke(rec.lX / 32760f);
            onGasPressed?.Invoke(rec.lY / -32760f);
            onClutchPressed?.Invoke(rec.rglSlider[0] / -32760f);
            onBrakePressed?.Invoke(rec.lRz / -32760f);

        }

        private void CheckButtons()
        {
            foreach (SteeringWheelButtonModel button in buttonsList)
            {
                if(LogitechGSDK.LogiButtonTriggered((int)GameController.FirstIndex, (int)button.inputName))
                    button.started?.Invoke();
            
                if(LogitechGSDK.LogiButtonIsPressed((int)GameController.FirstIndex, (int)button.inputName))
                    button.performed?.Invoke();
            
                if(LogitechGSDK.LogiButtonReleased((int)GameController.FirstIndex, (int)button.inputName))
                    button.released?.Invoke();
            }
        }

        private void CheckDirection()
        {
            SteeringWheelKeyCode currentDir = rec.rgdwPOV[0] switch
            {
                0 => SteeringWheelKeyCode.UPButton,
                4500 => SteeringWheelKeyCode.UP_RIGHTButton,
                9000 => SteeringWheelKeyCode.RIGHTButton,
                13500 => SteeringWheelKeyCode.DOWN_RIGHTButton,
                18000 => SteeringWheelKeyCode.DOWNButton,
                22500 => SteeringWheelKeyCode.DOWN_LEFTButton,
                27000 => SteeringWheelKeyCode.LEFTButton,
                31500 => SteeringWheelKeyCode.UP_LEFTButton,
                _ => SteeringWheelKeyCode.CENTER
            };

            try
            {
                var currentEvent = directionList.First(x => x.inputName == currentDir).onTrigger;
                currentEvent?.Invoke();
            }
            catch
            {
                Debug.Log("Direction hasent been set");
            }
        }

        public SteeringWheelButtonModel GetButton(SteeringWheelKeyCode keyCode)
        {
            return buttonsList.First(x => x.inputName == keyCode);
        }
    }
}