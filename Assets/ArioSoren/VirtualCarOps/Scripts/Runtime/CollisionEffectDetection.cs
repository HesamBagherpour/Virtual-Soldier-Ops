using PG;
using UnityEngine;

namespace ArioSoren.VirtualCarOps
{
    public class CollisionEffectDetection : MonoBehaviour
    {
        private CarController carController;

        private void Start()
        {
            carController = GetComponent<CarController>();
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.TryGetComponent(out RoadEffectType roadType))
            {
                if(carController.SpeedInHour < 10)
                    SteeringWheelEffects.ResetEffects();
                else 
                    switch (roadType.effectType)
                    {
                        case SteeringWheelEffect.None:
                            SteeringWheelEffects.ResetEffects();
                            break;
                        case SteeringWheelEffect.SpringForce:
                            SteeringWheelEffects.ActiveSpringForce(true, roadType.offsetPercentage,
                                roadType.saturationPercentage, roadType.coefficientPercentage);
                            break;
                        case SteeringWheelEffect.ConstantForce:
                            SteeringWheelEffects.ActiveConstantForce(true, roadType.magnitudePercentage);
                            break;
                        case SteeringWheelEffect.DamperForce:
                            SteeringWheelEffects.ActiveDamperForce(true, roadType.coefficientPercentage);
                            break;
                        case SteeringWheelEffect.DirtRoad:
                            SteeringWheelEffects.ActiveDirtRoad(true, roadType.magnitudePercentage);
                            break;
                        case SteeringWheelEffect.BumpyRoad:
                            SteeringWheelEffects.ActiveBumpyRoad(true, roadType.magnitudePercentage);
                            break;
                        case SteeringWheelEffect.SlipperyRoad:
                            SteeringWheelEffects.ActiveSlipperRoad(true, roadType.magnitudePercentage);
                            break;
                        case SteeringWheelEffect.SurfaceForce:
                            SteeringWheelEffects.ActiveSurfaceForce(true, roadType.magnitudePercentage, roadType.period);
                            break;
                        case SteeringWheelEffect.CarAirborne:
                            SteeringWheelEffects.ActiveCarAirborne(true);
                            break;
                        case SteeringWheelEffect.SoftStopForce:
                            SteeringWheelEffects.ActiveSoftStopForce(true, roadType.usableRangePercentage);
                            break;
                        default:
                            SteeringWheelEffects.ResetEffects();
                            break;
                    }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.TryGetComponent(out RoadEffectType roadType))
            {
                switch (roadType.effectType)
                {
                    case SteeringWheelEffect.SideCollision:
                        SteeringWheelEffects.DoSideCollision(roadType.magnitudePercentage);
                        break;
                    case SteeringWheelEffect.FrontCollision:
                        SteeringWheelEffects.DoFrontCollision(roadType.magnitudePercentage);
                        break;
                    default:
                        return;
                }
            }
        }
    }
}
