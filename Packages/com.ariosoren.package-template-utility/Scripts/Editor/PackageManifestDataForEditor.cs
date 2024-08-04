using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using ArioSoren.PackageTemplateUtility.Editor.Models.PackageManifestModel;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ArioSoren.PackageTemplateUtility.Editor
{
    [System.Serializable]
    public class PackageManifestDataForEditor
    {
        [ReadOnly] public string packagePath;
        [ReadOnly] public bool createPermit = false;
        
        [InfoBox("$nameErrorText", InfoMessageType.Error, nameof(nameError))] [OnValueChanged("ValidateName")]
        public string Name;

        private bool nameError;
        private bool versionError;
        private string versionErrorText;
        private string nameErrorText;

        [ReadOnly] public string displayName;
        
        [InfoBox("$versionErrorText", InfoMessageType.Error, nameof(versionError))] [OnValueChanged("ValidateVersion")]
        public string version;
        
        public string referenceVersion;
        
        public string unityVersion;

        public string unityRelease;

        [Space] public bool hasRuntimeScript;

        [Space] public bool hasEditorScript;


        [Space] [MultiLineProperty(10)] public string Description;

        public string host;
        public string documentationUrl;
        public string changelogUrl;
        public string license;
        public string licensesUrl;
        public bool hideInEditor;
        public string type;

        [Title("packageDependencies")] [HideLabel] [Space] [ShowInInspector]
        public Dictionary<string, string> packageDependencies = new Dictionary<string, string>();

        [Title("keywords")] [HideLabel] public List<string> keywords = new List<string>()
        {
            "ArioSoren",
            "Utility",
            "Utilities",
            "Tools"
        };

        [Title("author")] [HideLabel] [Space] public PackageManifestAuthor author = new PackageManifestAuthor()
        {
            Name = "ArioSoren Utilities Editor",
            Email = "tech-support@ArioSoren.com",
            Url = "https://ArioSoren.com"
        };

        [Title("Repository")] [HideLabel] [Space]
        public PackageManifestRepository packageManifestRepository;

        [Title("samples")] [HideLabel] [Space] public List<PackageManifestSample> samples;

        private bool ValidateName()
        {
            Regex regex = new Regex("^[a-z0-9-.]*$");
            if (regex.IsMatch(Name))
            {
                object packageTemplateUtilityManager;

                string[] nameSplit = Name.Split(".");
                displayName = PackageTemplateUtilityManager.GetPascalCase(nameSplit[^1], " ");
                if (Name.EndsWith('.'))
                {
                    nameErrorText = "Name should not finish with dash";
                    nameError = true;
                    createPermit = false;
                    return true;
                }
                else
                {
                    nameErrorText = string.Empty;
                    nameError = false;
                    if (!versionError)
                    {
                        createPermit = true;
                    }

                    if (PackageTemplateUtilityManager.CheckPackageExists(GetNameSpaceDirPath()))
                    {
                        nameErrorText = "Package Already Exists!";
                        nameError = true;
                        createPermit = false;
                    }

                    return false;
                }
            }
            else if (Name.EndsWith('.'))
            {
                nameErrorText = "Name should not finish with dash";
                nameError = true;
                createPermit = false;
                return true;
            }
            else
            {
                nameErrorText = "Name should only contain lowercase letters";
                nameError = true;
                createPermit = false;
                return true;
            }
        }

        private bool ValidateVersion()
        {
            Regex number = new Regex(@"^(\d+\.)?(\d+\.)?(\*|\d+)$");
            if (!number.IsMatch(version))
            {
                versionError = true;
                versionErrorText = "Version should be like 01.01.01";
                createPermit = false;
                return versionError;
            }
            else
            {
                if (!nameError)
                {
                    createPermit = true;
                }

                versionError = false;
                versionErrorText = string.Empty;
                return versionError;
            }
        }

        public string GetNameSpaceDirPath()
        {
            return Path.Combine(packagePath, displayName.Replace(" ", ""));
        }
    }
}