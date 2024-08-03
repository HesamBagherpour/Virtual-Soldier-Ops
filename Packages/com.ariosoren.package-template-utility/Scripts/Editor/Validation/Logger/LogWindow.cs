using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace ArioSoren.PackageTemplateUtility.Editor.Validation.Logger
{
    [Serializable]
    public class LogWindow
    {
        [ShowInInspector] [DisableInEditorMode] [HideReferenceObjectPicker]
        [ListDrawerSettings(ShowPaging = false, ShowIndexLabels = false, IsReadOnly = true, ShowItemCount = false, 
            DraggableItems = false, HideAddButton = true, HideRemoveButton = true, Expanded = true)]
        public static List<LogItem> logs = new List<LogItem>();

        public void AddNewLog(LogType logType, string logMessage)
        {
            logs.Add(new LogItem(logType, logMessage));
        }
    }
}