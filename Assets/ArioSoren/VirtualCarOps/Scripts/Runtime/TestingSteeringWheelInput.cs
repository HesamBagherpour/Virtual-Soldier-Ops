using PG;
using UnityEngine;

namespace ArioSoren.VirtualCarOps
{
    public class TestingSteeringWheelInput : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            SteeringWheelInputController.Instance.GetButton(SteeringWheelKeyCode.L2).started.AddListener(l2);
            SteeringWheelInputController.Instance.GetButton(SteeringWheelKeyCode.L3).started.AddListener(l3);
            SteeringWheelInputController.Instance.GetButton(SteeringWheelKeyCode.R2).started.AddListener(r2);
            SteeringWheelInputController.Instance.GetButton(SteeringWheelKeyCode.R3).started.AddListener(r3);
            SteeringWheelInputController.Instance.GetButton(SteeringWheelKeyCode.SHARE).started.AddListener(share);
            SteeringWheelInputController.Instance.GetButton(SteeringWheelKeyCode.OPTION).started.AddListener(option);
            SteeringWheelInputController.Instance.GetButton(SteeringWheelKeyCode.RedClockWise).started.AddListener(RedClockWise);
            SteeringWheelInputController.Instance.GetButton(SteeringWheelKeyCode.RedAntiClockWise).started.AddListener(RedAntiClockWise);
            SteeringWheelInputController.Instance.GetButton(SteeringWheelKeyCode.LogiPlus).started.AddListener(LogiPlus);
            SteeringWheelInputController.Instance.GetButton(SteeringWheelKeyCode.LogiMinus).started.AddListener(LogiMinus);
            SteeringWheelInputController.Instance.GetButton(SteeringWheelKeyCode.XBoxButton).started.AddListener(XBoxButton);
            SteeringWheelInputController.Instance.GetButton(SteeringWheelKeyCode.ArrowButton).started.AddListener(ArrowButton);
            SteeringWheelInputController.Instance.GetButton(SteeringWheelKeyCode.Shifter1).started.AddListener(Shifter1);
            SteeringWheelInputController.Instance.GetButton(SteeringWheelKeyCode.Shifter2).started.AddListener(Shifter2);
            SteeringWheelInputController.Instance.GetButton(SteeringWheelKeyCode.Shifter3).started.AddListener(Shifter3);
            SteeringWheelInputController.Instance.GetButton(SteeringWheelKeyCode.Shifter4).started.AddListener(Shifter4);
            SteeringWheelInputController.Instance.GetButton(SteeringWheelKeyCode.Shifter5).started.AddListener(Shifter5);
            SteeringWheelInputController.Instance.GetButton(SteeringWheelKeyCode.Shifter6).started.AddListener(Shifter6);
            SteeringWheelInputController.Instance.GetButton(SteeringWheelKeyCode.Shifter7).started.AddListener(Shifter7);
            SteeringWheelInputController.Instance.GetButton(SteeringWheelKeyCode.Shifter1).released.AddListener(Shifter1);
            Invoke(nameof(EffectTest),1);
            Invoke(nameof(EffectTest2),2);
            Invoke(nameof(EffectTest),4);
        }

        public void EffectTest()
        {
            SteeringWheelEffects.ActiveDirtRoad(true,20);
        }
        public void EffectTest2()
        {
            SteeringWheelEffects.ResetEffects();
        }
        public void r2() => Debug.Log(("R2"));
        public void l2() => Debug.Log(("L2"));
        public void r3() => Debug.Log(("R3"));
    
        public void l3() => Debug.Log(("L3"));

        public void share() => Debug.Log(("Share"));
        public void option() => Debug.Log(("option"));
        public void RedClockWise() => Debug.Log(("RedClockWise"));
        public void RedAntiClockWise() => Debug.Log(("RedAntiClockWise"));
        public void LogiPlus() => Debug.Log(("LogiPlus"));
        public void LogiMinus() => Debug.Log(("LogiMinus"));
        public void XBoxButton() => Debug.Log(("XBoxButton"));
        public void ArrowButton() => Debug.Log(("ArrowButton"));
        public void Shifter1() => Debug.Log(("Shifter1"));
        public void Shifter2() => Debug.Log(("Shifter2"));
        public void Shifter3() => Debug.Log(("Shifter3"));
        public void Shifter4() => Debug.Log(("Shifter4"));
        public void Shifter5() => Debug.Log(("Shifter5"));
        public void Shifter6() => Debug.Log(("Shifter6"));
        public void Shifter7() => Debug.Log(("Shifter7"));
    }
}
