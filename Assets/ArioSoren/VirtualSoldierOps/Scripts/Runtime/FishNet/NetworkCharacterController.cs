using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

namespace ArioSoren.VirtualSoldierOps
{
    public class NetworkCharacterController : NetworkBehaviour
    {

	    public CharacterInputControllers characterInputControllers;
        
	    public List<Behaviour> compList;

        public override void OnStartNetwork()
        {
            init();
        }

        private void init()
	    {
		    Debug.unityLogger.Log($"[NetworkCharacterController] [init] - IsLocalClient:{base.Owner.IsLocalClient} IsHostInitialized:{base.IsHostInitialized}");
            if (base.Owner.IsLocalClient || base.IsHostInitialized)
            {
	            characterInputControllers.EventSystem.SetActive(true);
                characterInputControllers.InputActionManager.SetActive(true);
                characterInputControllers.XRInteractionManager.SetActive(true);
	            characterInputControllers.MainCamera.SetActive(true);
	            
	            characterInputControllers.LeftController.SetActive(true);
	            characterInputControllers.LeftControllerStabilized.SetActive(true);
	            characterInputControllers.RightController.SetActive(true);
	            characterInputControllers.RightControllerStabilized.SetActive(true);
	            characterInputControllers.LocomotionSystem.SetActive(true);
	            
	            characterInputControllers.XROrigin.SetActive(true);
            }
            else
            {
	            compList.ForEach(item => item.enabled = false);
	            
	            characterInputControllers.LeftController.SetActive(false);
	            characterInputControllers.LeftControllerStabilized.SetActive(false);
	            characterInputControllers.RightController.SetActive(false);
	            characterInputControllers.RightControllerStabilized.SetActive(false);
	            characterInputControllers.LocomotionSystem.SetActive(false);
	            

                characterInputControllers.LeftController.SetActive(true);
	            characterInputControllers.LeftControllerStabilized.SetActive(true);
	            characterInputControllers.RightController.SetActive(true);
	            characterInputControllers.RightControllerStabilized.SetActive(true);

	            characterInputControllers.LocomotionSystem.SetActive(false);           
	         
	            characterInputControllers.XROrigin.SetActive(true);	 
	            
            }
        }

    }

    [Serializable]
    public struct CharacterInputControllers
	{
		
        public GameObject XROrigin;
        public GameObject InputActionManager;
        public GameObject XRInteractionManager;
        public GameObject EventSystem;
        public GameObject MainCamera;
        public GameObject LeftController;
        public GameObject LeftControllerStabilized;
        public GameObject RightController;
        public GameObject RightControllerStabilized;
        public GameObject LocomotionSystem;       
    }
}
