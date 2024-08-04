using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ArioSoren.TerminalKit
{
    public class TerminalMethods
    {
        public static List<MethodInfo> Methods { get; private set; } = new List<MethodInfo>();
        private static List<string> methodNames = new List<string>();
        public TerminalMethods()
        {
            ReCacheMethods();
        }

        public string[] GetCommandsContaining(string input)
        {
            return methodNames.Where(k => k.Contains(input)).ToArray();
        }

        public void ReCacheMethods()
        {
            Methods = new List<MethodInfo>();
            methodNames = new List<string>();

            MonoBehaviour[] sceneActive = UnityEngine.Object.FindObjectsOfType<MonoBehaviour>();

            foreach (MonoBehaviour mono in sceneActive)
            {
                AddObjectMethodsToTerminal(mono);
            }
        }

        public static void AddObjectMethodsToTerminal(MonoBehaviour mono)
        {
            Type monoType = mono.GetType();

            // Retreive the fields from the mono instance
            MethodInfo[] methodFields = monoType.GetMethods(BindingFlags.Instance | BindingFlags.Public);

            // search all fields and find the attribute
            for (int i = 0; i < methodFields.Length; i++)
            {
                // if we detect any attribute print out the data.
                if (Attribute.GetCustomAttribute(methodFields[i], typeof(TerminalCommandAttribute)) is TerminalCommandAttribute attribute)
                {
                    if (methodNames.Contains(attribute.CommandName) == false)
                    {
                        // Debug.Log($"Add mathods from {mono.gameObject.name} - {attribute.commandName}");
                        methodNames.Add(attribute.CommandName);
                        Methods.Add(methodFields[i]);
                    }
                }
            }
        }
    }
}
