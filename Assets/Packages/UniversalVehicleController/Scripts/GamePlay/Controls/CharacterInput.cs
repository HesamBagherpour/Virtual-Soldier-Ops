using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PG
{
    /// <summary>
    /// For character input, UI input and device input are combined in this component.
    /// </summary>
    public class CharacterInput :MonoBehaviour
    {
        [Header ("UI input settings")]
        public GameObject PfrentForUI;              //Shown if mobile platform is selected.

        public event System.Action OnEntrerInCar;

        CharacterInputActions InputActions;

        public Vector2 MoveInput { get; private set; }
        public Vector2 ViewInput { get; private set; }

        private void Awake ()
        {
            InputActions = new CharacterInputActions ();
        }

        private void Start ()
        {
            PfrentForUI.SetActive (GameSettings.IsMobilePlatform);

            InputActions.Input.EnterExit.started += (context) => OnEntrerInCar.SafeInvoke();
        }

        private void OnEnable ()
        {
            InputActions.Enable ();
        }

        private void OnDisable ()
        {
            InputActions.Disable ();
        }

        private void Update ()
        {
            MoveInput = InputActions.Input.Move.ReadValue<Vector2> ();
            ViewInput = InputActions.Input.View.ReadValue<Vector2> ();
        }
    }
}
