using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using ArioSoren.TerminalKit.Config;
using UnityEngine;

namespace ArioSoren.TerminalKit
{
    public class Terminal : MonoBehaviour
    {
        [SerializeField] public TerminalConfig config;
        public bool DisplayTerminal { get; private set; }
        public string InputText { get; private set; }
        public string History { get; private set; }
        public List<string> AutoCompList { get; private set; }
        public int AutoCompIndex { get; private set; }
        public string ConsoleLine => (config.console + " ");
        public TerminalMethods TerminalMethods;
        private TerminalInputHandler _inputHandler;
        private TerminalGUI _terminalGui;
        private LogStack _logStack;
        public TouchScreenKeyboard TouchScreenKeyboard;
        public int mobileTouchCount = 4;

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
            if (config == null) config = Resources.Load<TerminalConfig>("Config/ZSH");
            if (mobileTouchCount <= 0) mobileTouchCount = 4;
            AutoCompIndex = 0;
            AutoCompList = new List<string>();
            TerminalMethods = new TerminalMethods();
            _inputHandler = new TerminalInputHandler(this);
            _terminalGui = new TerminalGUI(this);
            _logStack = new LogStack(config);
        }

        void OnEnable()
        {
            Application.logMessageReceived += HandleLog;
        }

        void OnDisable()
        {
            Application.logMessageReceived -= HandleLog;
        }

        private void HandleLog(string logString, string stackTrace, LogType type)
        {
            _logStack.AddLog(logString, stackTrace, type);
        }


        [TerminalCommand("report", "report logs to support")]
        public void ReportLogs()
        {
            _logStack.Share();
        }

        [TerminalCommand("clearLogs", "clear all previous logs")]
        public void ClearLogs()
        {
            _logStack.Clear();
        }

        [TerminalCommand("help", "Shows list of available commands")]
        public string Help()
        {
            string helpString = "List of available commands:";
            foreach (var method in TerminalMethods.Methods)
            {
                foreach (var attribute in method.GetCustomAttributes(true))
                {
                    if (attribute is TerminalCommandAttribute) //Does not pass
                    {
                        TerminalCommandAttribute attr = (TerminalCommandAttribute)attribute;
                        helpString += "\n      " + attr.CommandName + " --> " + attr.CommandDesc;
                    }
                }
            }
            return helpString;
        }

        [TerminalCommand("hide", "Hides the terminal")]
        public void Hide()
        {
            DisplayTerminal = false;
        }

        void OnGUI()
        {
            if (!DisplayTerminal) return;
            _terminalGui.OnGUI();
        }

        void Update()
        {
            _inputHandler.Update();
        }

        /// <summary>
        /// For Android mobile keyboard input
        /// </summary>
        /// <param name="inputString"></param>
        internal void SetInputText(string inputString)
        {
            InputText = inputString;
        }

        /// <summary>
        /// For PC keyboard input
        /// </summary>
        /// <param name="input"></param>
        public void UpdateInputText(string input)
        {
            InputText += input;
            InputText = InputText.Replace("\b", "");
        }

        public void OnUpArrowPressed()
        {
            if (AutoCompList.Count > 0)
                AutoCompIndex = (int)Mathf.Repeat(AutoCompIndex - 1, AutoCompList.Count);
        }

        internal void ChangeInput(string input)
        {
            InputText = input;
        }

        public void OnDownArrowPressed()
        {
            if (AutoCompList.Count > 0)
                AutoCompIndex = (int)Mathf.Repeat(AutoCompIndex + 1, AutoCompList.Count);
        }

        public void PreExecute()
        {
            string result = ExecuteCommand(InputText);
            History += ConsoleLine + InputText + "\n" + (!string.IsNullOrEmpty(result) ? (result + "\n") : "");
            InputText = "";
        }

        public void OnTabPressed()
        {
            if (AutoCompList.Count != 0) { OnEnterPressed(); return; }
            AutoCompIndex = 0;
            AutoCompList.Clear();
            AutoCompList.AddRange(TerminalMethods.GetCommandsContaining(InputText));
        }

        private string ExecuteCommand(string inputText)
        {
            AutoCompList.Clear();
            bool registered = false;
            string result = null;
            string insideParentheses = Regex.Match(inputText, @"\(([^)]*)\)").Groups[1].Value;
            List<string> args = new List<string>();
            string command;
            if (!string.IsNullOrEmpty(insideParentheses))
            {
                args = insideParentheses.Split(new char[] { ',' }).ToList();
                command = inputText.Replace(insideParentheses, "").Replace("(", "").Replace(")", "").Replace(";", "");
            }
            else command = inputText.Replace("(", "").Replace(")", "").Replace(";", "");
            foreach (var method in TerminalMethods.Methods)
            {
                foreach (object attribute in method.GetCustomAttributes(true)) // Returns all 3 of my attributes.
                    if (attribute is TerminalCommandAttribute)
                    {
                        TerminalCommandAttribute attr = (TerminalCommandAttribute)attribute;
                        if (attr.CommandName == command)
                        {
                            if (registered) Debug.LogError(TerminalStrings.MultipleCommandNames + command);
                            Type type = (method.DeclaringType);
                            ParameterInfo[] methodParameters = method.GetParameters();
                            List<object> argList = new List<object>();

                            // check if method parameters and arguments count are equal
                            if (methodParameters.Length != args.Count)
                            {
                                result = string.Format(TerminalStrings.ArgumentCountMissmatch, command, methodParameters.Length, args.Count);
                                Debug.Log(result);
                                return result;
                            }

                            // Cast Arguments if there is any
                            if (args.Count != 0)
                            {
                                if (methodParameters.Length == args.Count)
                                {
                                    // Cast string arguments to input objects types
                                    for (int i = 0; i < methodParameters.Length; i++)
                                    {
                                        try
                                        {
                                            var a = Convert.ChangeType(args[i], methodParameters[i].ParameterType);
                                            argList.Add(a);
                                        }
                                        catch
                                        {
                                            result = string.Format("Counld not convert {0} to Type {1}", args[i], methodParameters[i].ParameterType);
                                            Debug.LogError(result);
                                            return result;
                                        }
                                    }
                                }
                            }
                            if (type.IsSubclassOf(typeof(UnityEngine.Object)))
                            {
                                var instance_classes = GameObject.FindObjectsOfType(type);
                                if (instance_classes != null)
                                {
                                    foreach (var instance_class in instance_classes)
                                    {
                                        result = (string)method.Invoke(instance_class, argList.ToArray());
                                    }
                                }
                            }
                            else
                            {
                                var instance_class = Activator.CreateInstance(type);
                                result = (string)method.Invoke(instance_class, argList.ToArray());
                            }
                            registered = true;
                            break;
                        }
                    }
            }
            if (!string.IsNullOrEmpty(result)) return result;
            if (registered) return null;
            return TerminalStrings.CommandNotFound;
        }

        [TerminalCommand("clear", "clears the terminal screen")]
        public void Clear()
        {
            StartCoroutine(ClearTerminalCoroutine());
        }

        private IEnumerator ClearTerminalCoroutine()
        {
            yield return new WaitForEndOfFrame();
            History = "";
        }

        internal void ToggleTerminal()
        {
            DisplayTerminal = !DisplayTerminal;
            DisplayTouchScreenKeyboard();
        }

        public void DisplayTouchScreenKeyboard()
        {
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                if (DisplayTerminal)
                {
                    TouchScreenKeyboard = TouchScreenKeyboard.Open(InputText, TouchScreenKeyboardType.Default);
                    TouchScreenKeyboard.hideInput = true;
                }
            }
        }

        internal void OnBackSpacePressed()
        {
            if (InputText.Length >= 1) InputText = InputText.Substring(0, InputText.Length - 1);
        }
        internal void OnEnterPressed()
        {
            if (AutoCompList.Count > 0)
            {
                InputText = AutoCompList[AutoCompIndex];
                AutoCompList.Clear();
            }
            else
                PreExecute();
        }
    }
}
