using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace ArioSoren.InputControllerUtility
{
    [Serializable]
    public class ActionModel
    {

        [OnValueChanged("reference_OnValueChanged")]
        public InputActionReference reference;


        [DisableIf("isReferenceNotNull")]
        [SerializeField] private string _actionName;

        public string actionName => reference != null ? reference.name : _actionName;

        [ShowIf("isReferenceNull")]
        [SerializeField] private bool _isActive = true;

        public bool isActive => reference != null ? reference.action.enabled : _isActive;

        [ShowIf("isReferenceNull")]
        [SerializeField] private InputAction _inputAction;

        public InputAction inputAction => reference != null ? reference.action : _inputAction;

        public UnityEvent<InputAction.CallbackContext> onAction;

        private bool isReferenceNull()
        {
            return reference == null;
        }
        private bool isReferenceNotNull()
        {
            return reference != null;
        }

        public void reference_OnValueChanged()
        {
            if (reference != null)
            {
                _actionName = reference.name;
            }
            else
            {
                _actionName = "";
            }
        }

    }
}