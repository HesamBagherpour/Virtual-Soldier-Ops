using UnityEngine;

namespace ArioSoren.VirtualCarOps
{
    public class RoadEffectType : MonoBehaviour
    {
        public SteeringWheelEffect effectType = SteeringWheelEffect.None;
        public int offsetPercentage = 50;
        public int saturationPercentage = 50;
        public int coefficientPercentage = 50;
        public int magnitudePercentage = 50;
        public int period = 1000;
        public int usableRangePercentage = 20;
    }
}
