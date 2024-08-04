using ArioSoren.PackageTemplateUtility.Editor.Models;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace ArioSoren.PackageTemplateUtility.Editor
{
    public class PackageDataInput :OdinEditorWindow
    {
        [ReadOnly] public string PackageNameSpace = "ae.ariosoren";
        [ReadOnly] public string assemblyName = "ArioSoren";
        
        [InfoBox("$nameErrorText", InfoMessageType.Error, nameof(nameError))]
        [OnValueChanged("ValidateName")]
        [BoxGroup("Package Information")]
        public string Name;
        public  bool nameError;
        public  bool versionError;
        public  string versionErrorText;
        public  string nameErrorText;
        public static bool createPermit = false;
        
        [BoxGroup("Package Information")] public string displayName;
        [InfoBox("$versionErrorText", InfoMessageType.Error, nameof(versionError))]
        [OnValueChanged("ValidateVersion")]
        [BoxGroup("Package Information")]
        public string version;
        [BoxGroup("Package Information")] public string unityVersion;

        [Space] [BoxGroup("Package Information")]
        public bool hasRuntimeScript;

        [Space] [BoxGroup("Package Information")]
        public bool hasEditorScript;

        [Space] [MultiLineProperty(10)] [BoxGroup("Package Information")]
        public string Description;
        
        public static bool packageCreationState;
        public PackageAssemblyDefinitionManifest packageAssemblyDefinitionManifest;
    }
}