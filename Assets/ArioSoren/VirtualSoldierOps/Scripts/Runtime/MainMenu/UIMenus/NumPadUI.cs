using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ArioSoren.VirtualSoldierOps.MainMenu.UIMenus
{
    public class NumPadUI : MonoBehaviour
    {
        public Canvas rootCanvas;
        public RectTransform root;
        public List<Button> numpadKeys = new();
        public Button dotButton;
        public Button backspaceButton;
        public Button deleteButton;
        public TMP_InputField inputView;
        public async void Init()
        {

            for (int i = 0; i < numpadKeys.Count; i++)
                numpadKeys[i].onClick.RemoveAllListeners();

            AssignNumbers();
            
            dotButton.onClick.RemoveAllListeners();
            dotButton.onClick.AddListener(()=>AddText("."));
     
            backspaceButton.onClick.RemoveAllListeners();
            backspaceButton.onClick.AddListener(()=> RemoveText());
            
            deleteButton.onClick.RemoveAllListeners();
            deleteButton.onClick.AddListener(()=>RemoveText(true));
        }

        public void AssignNumbers()
        {
            numpadKeys[0].onClick.AddListener(()=> AddText("0"));
            numpadKeys[1].onClick.AddListener(()=> AddText("1"));
            numpadKeys[2].onClick.AddListener(()=> AddText("2"));
            numpadKeys[3].onClick.AddListener(()=> AddText("3"));
            numpadKeys[4].onClick.AddListener(()=> AddText("4"));
            numpadKeys[5].onClick.AddListener(()=> AddText("5"));
            numpadKeys[6].onClick.AddListener(()=> AddText("6"));
            numpadKeys[7].onClick.AddListener(()=> AddText("7"));
            numpadKeys[8].onClick.AddListener(()=> AddText("8"));
            numpadKeys[9].onClick.AddListener(()=> AddText("9"));
        }
        
        public void AddText(string newText)
        {
            print(newText);
            inputView.text += newText;
        } 

        public void RemoveText(bool fullText = false)
        {
            if(inputView.text.Length <= 0)
                return;
            
            if (fullText)
            {
                inputView.text = "";
                return;
            }
            
            var currentText = inputView.text;
            currentText = currentText.Remove(currentText.Length -1);
            inputView.text = currentText;
        }
    }
}
