using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ArioSoren.PackageTemplateUtility.Editor.Validation.Logger
{
    public enum LogType
    {
        ERROR,
        WARNING,
        INFO,
        PASSED
    }

    [Serializable]
    public class LogItem
    {
        [HideLabel] [GUIColor("ReturnColor")]
        public string logString;

        public static Color ReturnColor()
        {
            return _logColor;
        }
        
        [OnValueChanged("ReturnColor")]
        private static Color _logColor = Color.white;
        
        private LogType _logType;

        public LogItem(LogType logType, string logMessage)
        {
            _logType = logType;
            logString = logMessage;

            switch (_logType)
            {
                case LogType.ERROR:
                    _logColor = Color.red;
                    break;
                case LogType.WARNING:
                    _logColor = Color.yellow;
                    break;
                case LogType.INFO:
                    _logColor = Color.white;
                    break;
                case LogType.PASSED:
                    _logColor = Color.green;
                    break;
            }
        }
    }
}