using System.Linq;
using ArioSoren.GeneralUtility;
using ArioSoren.InputControllerUtility;
using ArioSoren.PlayerSystem.Combat.PlayerSystem.Combat.Shoot;
using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;


// TODO @Network This can be disabled on a remote or server
// This is the handle animator state
namespace ArioSoren.PlayerSystem.Combat.PlayerSystem.Combat.VR.Player
{
    public class PlayerHandAnimation : NetworkBehaviour
    {
        [SerializeField] private PlayerHand hand;
        [SerializeField] private Transform controller;
        [SerializeField] private PlayerHandController playerHand;
        [SerializeField] private GameObject character;

        private HandsAnimation _handsAnimation;
        private Animator _handDeformAnimator;
        private Animator _directInteractionAnimator;

        private GameObject _handGameObject;

        //@NetworkHint: Just used and set on owner client - player input
        private bool _isActive = true;
        private bool _playRecoilToggle;

        private InputControllerComp _inputControl;
        private static readonly int Grip = Animator.StringToHash("Grip");
        private static readonly int Pinch = Animator.StringToHash("Pinch");


        #region Client

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (IsOwner)
            {
                ChangePlayerInputSubscription(true);
            }
            else
            {
                // All Remote Clients
            }

            // All Client
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            if (IsOwner)
            {
                ChangePlayerInputSubscription(false);
            }
            else
            {
                // All Remote Clients
            }

            // All Client
        }

        #endregion


        private void ChangePlayerInputSubscription(bool state)
        {
            if (state)
            {
                _inputControl.GetInputActionByName(hand == PlayerHand.Right
                    ? InputActionName.XRI_RightHand_Interaction_Select_Value.GetName()
                    : InputActionName.XRI_LeftHand_Interaction_Select_Value.GetName()).performed += OnGripping;
                _inputControl.GetInputActionByName(hand == PlayerHand.Right
                    ? InputActionName.XRI_RightHand_Interaction_Select_Value.GetName()
                    : InputActionName.XRI_LeftHand_Interaction_Select_Value.GetName()).canceled += OnGripRelease;
                _inputControl.GetInputActionByName(hand == PlayerHand.Right
                    ? InputActionName.XRI_RightHand_Interaction_Activate_Value.GetName()
                    : InputActionName.XRI_LeftHand_Interaction_Activate_Value.GetName()).performed += OnPinching;
                _inputControl.GetInputActionByName(hand == PlayerHand.Right
                    ? InputActionName.XRI_RightHand_Interaction_Activate_Value.GetName()
                    : InputActionName.XRI_LeftHand_Interaction_Activate_Value.GetName()).canceled += OnPinchRelease;
            }
            else
            {
                _inputControl.GetInputActionByName(hand == PlayerHand.Right
                    ? InputActionName.XRI_RightHand_Interaction_Select_Value.GetName()
                    : InputActionName.XRI_LeftHand_Interaction_Select_Value.GetName()).started -= OnGripping;
                _inputControl.GetInputActionByName(hand == PlayerHand.Right
                    ? InputActionName.XRI_RightHand_Interaction_Select_Value.GetName()
                    : InputActionName.XRI_LeftHand_Interaction_Select_Value.GetName()).canceled -= OnGripRelease;
                _inputControl.GetInputActionByName(hand == PlayerHand.Right
                    ? InputActionName.XRI_RightHand_Interaction_Activate_Value.GetName()
                    : InputActionName.XRI_LeftHand_Interaction_Activate_Value.GetName()).performed -= OnPinching;
                _inputControl.GetInputActionByName(hand == PlayerHand.Right
                    ? InputActionName.XRI_RightHand_Interaction_Activate_Value.GetName()
                    : InputActionName.XRI_LeftHand_Interaction_Activate_Value.GetName()).canceled -= OnPinchRelease;

                //TODO if 'this gameObject' possible will destroy -> should unsubscribe input in OnDestroy method too 
            }
        }


        protected void Awake()
        {
            var handIndex = Enumerable.Range(0, controller.childCount)
                .First(x => controller.GetChild(x).CompareTag("Hand"));
            _handGameObject = controller.GetChild(handIndex).gameObject;
            _handDeformAnimator = _handGameObject.GetComponent<Animator>();
            _directInteractionAnimator = GetComponent<Animator>();
            _inputControl = gameObject.GetComponent<InputControllerComp>();

            if (character != null)
                _handsAnimation = character.GetComponent<HandsAnimation>();
        }

        //TODO player input - animation sync - client 
        private void OnGripping(InputAction.CallbackContext obj)
        {
            if (character != null)
                _handsAnimation.Grab(playerHand.Hand, obj.ReadValue<float>());

            if (_isActive)
                _handDeformAnimator.SetFloat(Grip, obj.ReadValue<float>());
        }

        //TODO player input - animation sync - client 
        private void OnGripRelease(InputAction.CallbackContext obj)
        {
            GripRelease();
        }

        //TODO player input - animation sync - client 
        private void OnPinching(InputAction.CallbackContext obj)
        {
            if (character != null)
                _handsAnimation.Pinch(playerHand.Hand, obj.ReadValue<float>());
            if (_isActive)
                _handDeformAnimator.SetFloat(Pinch, obj.ReadValue<float>());
        }

        //TODO player input - animation sync - client 
        private void OnPinchRelease(InputAction.CallbackContext obj)
        {
            PinchRelease();
        }

        private void GripRelease()
        {
            if (character != null)
                _handsAnimation.Grab(playerHand.Hand, 0f);

            _handDeformAnimator.SetFloat(Grip, 0f);
        }

        private void PinchRelease()
        {
            if (character != null)
                _handsAnimation.Pinch(playerHand.Hand, 0f);

            _handDeformAnimator.SetFloat(Pinch, 0f);
        }

        // @NetworkHint: called by input action 
        public void Active()
        {
            _isActive = true;
        }

        // @NetworkHint: called by input action 
        public void Deactivate()
        {
            _isActive = false;
            GripRelease();
            PinchRelease();
        }

        //TODO animation sync
        public void PlayRecoil()
        {
            if (_playRecoilToggle)
            {
                _directInteractionAnimator.CrossFade("Recoil2", 0.1f);
                _playRecoilToggle = false;
            }
            else
            {
                _directInteractionAnimator.CrossFade("Recoil1", 0.1f);
                _playRecoilToggle = true;
            }
        }


////// Generated Code [Start] --- InputController inspector -- Don't change this block /////

        #region InputActionName

        public enum InputActionName
        {
            [EnumName("XRI LeftHand Interaction/Switch Down")]
            XRI_LeftHand_Interaction_Switch_Down,

            [EnumName("XRI LeftHand/Position")] XRI_LeftHand_Position,

            [EnumName("XRI LeftHand Interaction/Select Value")]
            XRI_LeftHand_Interaction_Select_Value,

            [EnumName("XRI LeftHand Interaction/Activate Value")]
            XRI_LeftHand_Interaction_Activate_Value,

            [EnumName("XRI RightHand/Position")] XRI_RightHand_Position,

            [EnumName("XRI RightHand Interaction/Select Value")]
            XRI_RightHand_Interaction_Select_Value,

            [EnumName("XRI RightHand Interaction/Activate Value")]
            XRI_RightHand_Interaction_Activate_Value,

            [EnumName("XRI RightHand Interaction/Switch Up")]
            XRI_RightHand_Interaction_Switch_Up,

            [EnumName("XRI RightHand Interaction/Switch Down")]
            XRI_RightHand_Interaction_Switch_Down,

            [EnumName("XRI LeftHand Interaction/Switch Up")]
            XRI_LeftHand_Interaction_Switch_Up,
        }

        #endregion InputActionName

////// Generated Code [End] --- InputController inspector -- Don't change this block /////
    }
}