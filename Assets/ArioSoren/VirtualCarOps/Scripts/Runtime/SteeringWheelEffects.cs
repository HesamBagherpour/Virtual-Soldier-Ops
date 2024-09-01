using UnityEngine;

namespace ArioSoren.VirtualCarOps
{
    public enum SteeringWheelEffect
    {
        None,
        SpringForce,
        ConstantForce,
        DamperForce,
        DirtRoad,
        BumpyRoad,
        SlipperyRoad,
        SurfaceForce,
        CarAirborne,
        SoftStopForce,
        SideCollision,
        FrontCollision,
    }
    public class SteeringWheelEffects
    {
        private static SteeringWheelEffect currentEffect = SteeringWheelEffect.None;

        public static void ResetEffects()
        {
            if (currentEffect != SteeringWheelEffect.None)
            {        
                Debug.Log("resetting effect ");
            
                if(currentEffect == SteeringWheelEffect.BumpyRoad)
                    ActiveBumpyRoad(false);
                else if(currentEffect == SteeringWheelEffect.SpringForce)
                    ActiveSpringForce(false);
                else if(currentEffect == SteeringWheelEffect.ConstantForce)
                    ActiveConstantForce(false);
                else if(currentEffect == SteeringWheelEffect.DamperForce)
                    ActiveDamperForce(false);
                else if(currentEffect == SteeringWheelEffect.DirtRoad)
                    ActiveDirtRoad(false);
                else if(currentEffect == SteeringWheelEffect.SlipperyRoad)
                    ActiveSlipperRoad(false);
                else if(currentEffect == SteeringWheelEffect.SurfaceForce)
                    ActiveSurfaceForce(false);
                else if(currentEffect == SteeringWheelEffect.CarAirborne)
                    ActiveCarAirborne(false);
                else if(currentEffect == SteeringWheelEffect.SoftStopForce)
                    ActiveSoftStopForce(false);
            
                currentEffect = SteeringWheelEffect.None;
            }
        }

        //Spring Force
        public static void ActiveSpringForce(bool active, int offset = 50,int saturation = 50, int coefficient = 50)
        {
            if (active && currentEffect != SteeringWheelEffect.SpringForce)
            {
                ResetEffects();
                LogitechGSDK.LogiPlaySpringForce(0, offset, saturation, coefficient);
                currentEffect = SteeringWheelEffect.SpringForce;
            }
            else if(!active /*&& LogitechGSDK.LogiIsPlaying(0, LogitechGSDK.LOGI_FORCE_SPRING)*/)
            {
                LogitechGSDK.LogiStopSpringForce(0);
            }
        }

        //Constant Force
        public static void ActiveConstantForce(bool active, int magnitude = 50)
        {
            if (active && currentEffect != SteeringWheelEffect.ConstantForce)
            {       
                ResetEffects();
                LogitechGSDK.LogiPlayConstantForce(0, magnitude);
                currentEffect = SteeringWheelEffect.ConstantForce;
            }
            else if(!active /*&& LogitechGSDK.LogiIsPlaying(0, LogitechGSDK.LOGI_FORCE_CONSTANT)*/)
            {
                LogitechGSDK.LogiStopConstantForce(0);
            }
        }

        //Damper Force
        public static void ActiveDamperForce(bool active, int coefficient = 50)
        {
            if (active && currentEffect != SteeringWheelEffect.DamperForce)
            {
                ResetEffects();
                LogitechGSDK.LogiPlayDamperForce(0, coefficient);
                currentEffect = SteeringWheelEffect.DamperForce;
            }
            else if(!active /*&& LogitechGSDK.LogiIsPlaying(0, LogitechGSDK.LOGI_FORCE_DAMPER)*/)
            {           
                LogitechGSDK.LogiStopDamperForce(0);
            }
        }

        //Dirt Road Effect
        public static void ActiveDirtRoad(bool active, int magnitude = 50)
        {
            if (active && currentEffect != SteeringWheelEffect.DirtRoad)
            {
                ResetEffects();
                LogitechGSDK.LogiPlayDirtRoadEffect(0, magnitude);
                currentEffect = SteeringWheelEffect.DirtRoad;
            }
            else if(!active /*&& LogitechGSDK.LogiIsPlaying(0, LogitechGSDK.LOGI_FORCE_DIRT_ROAD)*/)
            {
                LogitechGSDK.LogiStopDirtRoadEffect(0);
            }

        }

        //Bumpy Road Effect
        public static void ActiveBumpyRoad(bool active, int magnitude = 50)
        {
            if (active && currentEffect != SteeringWheelEffect.BumpyRoad)
            {
                ResetEffects();
                LogitechGSDK.LogiPlayBumpyRoadEffect(0, magnitude);
                currentEffect = SteeringWheelEffect.BumpyRoad;
            }
            else if(!active /*&& LogitechGSDK.LogiIsPlaying(0, LogitechGSDK.LOGI_FORCE_BUMPY_ROAD)*/)
            {
                LogitechGSDK.LogiStopBumpyRoadEffect(0);
            }

        }

        //Slippery Road Effect
        public static void ActiveSlipperRoad(bool active, int magnitude = 50)
        {
            if (active && currentEffect != SteeringWheelEffect.SlipperyRoad)
            {
                ResetEffects();
                LogitechGSDK.LogiPlaySlipperyRoadEffect(0, magnitude);
                currentEffect = SteeringWheelEffect.SlipperyRoad;
            }
            else if(!active /*&& LogitechGSDK.LogiIsPlaying(0, LogitechGSDK.LOGI_FORCE_SLIPPERY_ROAD)*/)
            {
                LogitechGSDK.LogiStopSlipperyRoadEffect(0);

            }
        }

        //Surface Effect
        public static void ActiveSurfaceForce(bool active, int magnitude = 50,int period = 1000)
        {
            if (active && currentEffect != SteeringWheelEffect.SurfaceForce)
            {
                ResetEffects();
                LogitechGSDK.LogiPlaySurfaceEffect(0, LogitechGSDK.LOGI_PERIODICTYPE_SQUARE, magnitude, period);
                currentEffect = SteeringWheelEffect.SurfaceForce;
            }
            else if(!active)
            {
                LogitechGSDK.LogiStopSurfaceEffect(0);

            }
        }

        //Car Airborne
        public static void ActiveCarAirborne(bool active)
        {
            if (active && currentEffect != SteeringWheelEffect.CarAirborne)
            {
                ResetEffects();
                LogitechGSDK.LogiStopCarAirborne(0);
                currentEffect = SteeringWheelEffect.CarAirborne;
            }
            else if(!active)
            {
                LogitechGSDK.LogiPlayCarAirborne(0);

            }
        }

        //Soft Stop Force
        public static void ActiveSoftStopForce(bool active,int usableRange = 20)
        {
            if (active && currentEffect != SteeringWheelEffect.SoftStopForce)
            {
                ResetEffects();
                LogitechGSDK.LogiPlaySoftstopForce(0, usableRange);
                currentEffect = SteeringWheelEffect.SoftStopForce;
            }
            else if(!active)
            {
                LogitechGSDK.LogiStopSoftstopForce(0);
            }
        }
    
        //Side Collision Force
        public static void DoSideCollision(int magnitude = 50)
        {
            LogitechGSDK.LogiPlaySideCollisionForce(0, magnitude);
        }

        //Front Collision Force 
        public static void DoFrontCollision(int magnitude = 50)
        {
            LogitechGSDK.LogiPlayFrontalCollisionForce(0, magnitude);
        }

    }
}