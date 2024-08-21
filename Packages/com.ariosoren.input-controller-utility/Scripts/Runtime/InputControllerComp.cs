using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;


namespace ArioSoren.InputControllerUtility
{
    public class InputControllerComp : MonoBehaviour
    {
        public bool autoEnable = true;
        public bool autoDisable = true;

        [SerializeField] public List<ActionModel> dataList;
        private List<ActionModel> _actionDataList;
        private InputActionMap _actionMap;

        private void Awake()
        {
            _actionMap = new InputActionMap();
            _actionDataList = new List<ActionModel>();
            InitialList(dataList);
        }


        private void OnEnable()
        {
            if (autoEnable)
            {
                EnableAllInputAction();
            }
        }

        private void OnDisable()
        {
            if (autoDisable)
            {
                DisableAllInputAction();
            }
        }

        private void InitialList(List<ActionModel> models)
        {
            foreach (ActionModel actionData in models)
            {
                AddInputAction(actionData);
            }
        }

        public bool AddInputAction(ActionModel actionModel)
        {

            bool isFind = IsExistInputAction(actionModel.actionName);
            if (!isFind)
            {
                _actionDataList.Add(actionModel);
                actionModel.inputAction.performed += context => { actionModel.onAction?.Invoke(context); };
                if (actionModel.reference == null)
                    actionModel.inputAction.Rename(actionModel.actionName);
                if (actionModel.isActive)
                    actionModel.inputAction.Enable();
                _actionMap.AddAction(actionModel.actionName);
            }
            else
            {
                Debug.LogError("Action is exist");
            }

            return isFind;
        }

        public void RemoveInputAction(string actionName)
        {
            bool isFind = IsExistInputAction(actionName);
            if (isFind)
            {
                ActionModel data = _actionDataList.First(x => x.actionName == actionName);
                _actionDataList.Remove(data);
            }
            else
            {
                Debug.LogError("Action is exist");
            }
        }

        public void EnableInputAction(string actionName)
        {
            bool isFind = IsExistInputAction(actionName);
            if (isFind)
            {
                ActionModel data = _actionDataList.First(x => x.actionName == actionName);
                data.inputAction.Enable();
            }
            else
            {
                Debug.LogError("Action is exist");
            }
        }

        public void DisableInputAction(string actionName)
        {
            bool isFind = IsExistInputAction(actionName);
            if (isFind)
            {
                ActionModel data = _actionDataList.First(x => x.actionName == actionName);
                data.inputAction.Disable();
            }
            else
            {
                Debug.LogError("Action is exist");
            }
        }

        public bool IsExistInputAction(string actionName)
        {
            return _actionDataList.Exists(x => x.actionName == actionName);
        }

        public void DisableAllInputAction()
        {
            foreach (var action in _actionDataList)
            {
                action.inputAction.Disable();
            }
        }

        public void EnableAllInputAction()
        {
            foreach (var action in _actionDataList)
            {
                action.inputAction.Enable();
            }
        }


        public List<InputAction> GetAllInputActions()
        {
            List<InputAction> list = new List<InputAction>();
            foreach (var action in _actionDataList)
            {
                list.Add(action.inputAction);
            }

            return list;
        }


        public InputAction GetInputActionByName(string actionName)
        {
            bool isFind = IsExistInputAction(actionName);
            if (isFind)
            {
                ActionModel data = _actionDataList.First(x => x.actionName == actionName);
                return data.inputAction;
            }
            else
            {
                return null;
            }
        }

        public List<InputBinding> GetAllInputBindingByActionName(string actionName)
        {
            bool isFind = IsExistInputAction(actionName);
            if (isFind)
            {
                ActionModel data = _actionDataList.First(x => x.actionName == actionName);
                return data.inputAction.bindings.ToList();
            }
            else
            {
                return null;
            }
        }

        public Tuple<InputBinding, bool> GetInputBindingById(InputAction inputAction, Guid Id)
        {
            foreach (var item in inputAction.bindings)
            {
                if (item.id == Id)
                {
                    return new Tuple<InputBinding, bool>(item, true);
                }
            }
            return new Tuple<InputBinding, bool>(new InputBinding(), false);
        }

        public void SetInputBinding(InputAction inputAction, InputBinding inputBinding)
        {
            for (int i = 0; i < inputAction.bindings.Count; i++)
            {
                var item = inputAction.bindings[i];
                if (inputAction.bindings[i].id == inputBinding.id)
                {
                    inputAction.ApplyBindingOverride(i, inputBinding);
                }
            }
        }

        // public InputAction GetInputAction(string inputActionName)
        // {
        //     return actionDataList.First(x => x.actionName == inputActionName).inputAction;
        // }


#if UNITY_EDITOR
        [ContextMenu("Append Selected to Input Action References")]
        [Button("Append Selected to Input Action References")]
        private void AppendSelectedToInputActionReferences()
        {
            InputActionReference[] references = Selection.GetFiltered<InputActionReference>(SelectionMode.Unfiltered);

            var newActionModels = references
                .Where(reference => !dataList.Any(x => x.reference == reference))
                .Select(reference =>
                {
                    var actionModel = new ActionModel { reference = reference };
                    actionModel.reference_OnValueChanged();
                    return actionModel;
                })
                .ToList();

            dataList.AddRange(newActionModels);
        }

        [SerializeField] private MonoBehaviour WriteEnumToScriptTarget;

        [ShowIf("IsWriteEnumScriptTargetNotNull")]
        [SerializeField] private string enumName = "InputActionName";

        [ContextMenu("Write Enum From Action Names")]
        [Button("Write Enum From Action Names")]
        [ShowIf("IsWriteEnumScriptTargetNotNull")]
        private void WriteEnumFromActionNames()
        {
            if (WriteEnumToScriptTarget == null) return;

            string startMarker = "////// Generated Code [Start] --- InputController inspector -- Don't change this block /////";
            string endMarker = "////// Generated Code [End] --- InputController inspector -- Don't change this block /////";

            string scriptPath = AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(WriteEnumToScriptTarget));

            string scriptText = File.ReadAllText(scriptPath);

            // Removing the code block between the specified lines
            // Constructing the Regex pattern to find the code block
            string patternToRemove = Regex.Escape(startMarker) + @"[\s\S]*?" + Regex.Escape(endMarker);
        
            // Removing the code block between the specified lines
            scriptText = Regex.Replace(scriptText, patternToRemove, string.Empty, RegexOptions.Multiline);


            StringBuilder enumCode = new StringBuilder();

            enumCode.AppendLine(startMarker);
            enumCode.AppendLine("#region InputActionName");
            enumCode.AppendLine($"public enum {enumName}");
            enumCode.AppendLine("{");

            foreach (var action in dataList)
            {
                string cleanedName = CleanEnumName(action.actionName);
                enumCode.AppendLine($"    [EnumName(\"{action.actionName}\")]");
                enumCode.AppendLine($"    {cleanedName},");
            }
            enumCode.AppendLine("}");

            enumCode.AppendLine("#endregion InputActionName");
            enumCode.AppendLine(endMarker);


            // Finding the appropriate point to insert the enum
            int insertIndex = scriptText.LastIndexOf("}");
            string newScriptText = scriptText.Insert(insertIndex, enumCode.ToString() + "\n");

            File.WriteAllText(scriptPath, newScriptText);

            AssetDatabase.Refresh();

        }
        private bool IsWriteEnumScriptTargetNotNull()
        {
            return WriteEnumToScriptTarget != null;
        }

        private string CleanEnumName(string name)
        {
            // Replacing invalid characters with _
            var cleaned = new StringBuilder();
            foreach (char c in name)
            {
                if (char.IsLetterOrDigit(c) || c == '_')
                    cleaned.Append(c);
                else
                    cleaned.Append('_');
            }

            // Ensuring the name starts with a letter
            if (char.IsDigit(cleaned[0]))
                cleaned.Insert(0, '_');

            return cleaned.ToString();
        }
#endif

    }
}