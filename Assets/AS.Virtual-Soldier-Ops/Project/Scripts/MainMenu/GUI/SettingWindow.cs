using System;
using AS.Virtual_Soldier_Ops.Packages.UI.Scripts.Module;
using UnityEngine;
using UnityEngine.UI;

namespace AS.Virtual_Soldier_Ops.Project.Scripts.MainMenu.GUI
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
 