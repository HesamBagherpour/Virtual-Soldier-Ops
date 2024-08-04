using System;

namespace ArioSoren.TerminalKit
{
    [AttributeUsage(AttributeTargets.Method)]
    public class TerminalCommandAttribute : Attribute
    {
        public string CommandName;
        public string CommandDesc;

        public TerminalCommandAttribute(string name)
        {
            CommandName = name;
        }
        public TerminalCommandAttribute(string name, string desc)
        {
            CommandName = name;
            CommandDesc = desc;
        }
    }
}