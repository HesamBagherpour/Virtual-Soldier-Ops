using System;
using ArioSoren.UIKit.Module;
using UnityEngine;
using UnityEngine.UI;

namespace ArioSoren.VirtualSoldierOps.MainMenu.GUI
{
    public class SettingWindow : Window
    {
        // Start is called before the first frame update
        public event Action CloseBtnClicked;
        public Button close;
        public override void Init()
        {
            close.onClick.AddListener(TaskOnClick);
            base.Init();
        }
        public void OnEnable()
        {
            // startGame
        }
        public void OnDisable()
        {
            // End Game
        }
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
        void TaskOnClick()
        {
            Close();
            CloseBtnClicked?.Invoke();
        }

        private void OnStageSelectorEnter()
        {
            Debug.Log("  StageSelectorEnter : ");
        }
    }
}
 