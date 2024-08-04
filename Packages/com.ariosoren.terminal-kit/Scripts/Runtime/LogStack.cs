using System.Collections.Generic;
using ArioSoren.TerminalKit.Config;
using UnityEngine;

namespace ArioSoren.TerminalKit
{
    public class LogStack
    {
        private readonly TerminalConfig _config;
        public List<LogType> logTypes { get; private set; }
        public List<string> logTitles { get; private set; }
        public List<string> logStacks { get; private set; }
        public static string Clipboard
        {
            get { return GUIUtility.systemCopyBuffer; }
            set { GUIUtility.systemCopyBuffer = value; }
        }
        public LogStack(TerminalConfig config)
        {
            this._config = config;
            logTypes = new List<LogType>();
            logTitles = new List<string>();
            logStacks = new List<string>();
        }

        public void AddLog(string logString, string stackTrace, LogType type)
        {
            if (logTitles.Count > _config.LogCount)
            {
                logTitles.RemoveAt(0);
                logStacks.RemoveAt(0);
                logTypes.RemoveAt(0);
            }
            logTitles.Add(logString);
            logStacks.Add(stackTrace);
            logTypes.Add(type);
        }
        public void Share()
        {

            string _result = "";
            for (int i = 0; i < logTypes.Count; i++)
            {
                _result += logTypes[i] + " : ";
                _result += logTitles[i] + "\n";
                switch (logTypes[i])
                {
                    case LogType.Log:
                        if (_config.infoStacks)
                            _result += logStacks[i] + "\n";
                        break;
                    case LogType.Warning:
                        if (_config.warningStacks)
                            _result += logStacks[i] + "\n";
                        break;
                    case LogType.Error:
                        if (_config.errorStacks)
                            _result += logStacks[i] + "\n";
                        break;
                    case LogType.Assert:
                        if (_config.assertStacks)
                            _result += logStacks[i] + "\n";
                        break;
                    case LogType.Exception:
                        if (_config.exceptionStacks)
                            _result += logStacks[i] + "\n";
                        break;
                    default:
                        break;
                }
            }
            Clipboard = _result;
            Application.OpenURL("mailto:" + _config.supportEmail + "?subject=" + _config.EmailTitle + "&body=" + MyEscapeURL(_result));
        }

        public void Clear()
        {
            logTitles.Clear();
            logStacks.Clear();
            logTypes.Clear();
        }

        string MyEscapeURL(string url)
        {
            return WWW.EscapeURL(url).Replace("+", "%20");
        }
    }
}
