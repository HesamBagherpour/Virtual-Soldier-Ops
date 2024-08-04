using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using ArioSoren.GeneralUtility.Command;
using ArioSoren.GeneralUtility.Command.Models;
using ArioSoren.PackageTemplateUtility.Editor.Models;
using ArioSoren.PackageTemplateUtility.Editor.Validation;
using ArioSoren.PackageTemplateUtility.Editor.Validation.Logger;
using ArioSoren.ProjectTemplateUtility.Editor.Data;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using LogType = ArioSoren.PackageTemplateUtility.Editor.Validation.Logger.LogType;
using PackageManifest = ArioSoren.PackageTemplateUtility.Editor.Models.PackageManifestModel.PackageManifest;

namespace ArioSoren.PackageTemplateUtility.Editor
{
    public class ShowPackageWindow : OdinEditorWindow
    {
        private PackageManifest _packageManifest = new PackageManifest();
        private string packagePascalName;
        private bool _packageIsValid = false;
        
        [HideInInspector] public string pakagePathForEditor;

        public void Init(string path)
        {
            if (!File.Exists(path)) return;
            var oldPackageManifest = PackageTemplateUtilityManager.GetPackageManifestData(path);
            SetPackageData(oldPackageManifest, path);
        }
        
        private static void OpenWindow()
        {
            var window = GetWindow<ShowPackageWindow>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(600, 700);
        }
        
        [HideLabel] [Title("Package Information")]
        public PackageManifestDataForEditor packageManifestDataForEditor;
        
        [ButtonGroup("bottomBtns")]
        [Button(ButtonSizes.Small, ButtonStyle.Box)]
        public void Documentation()
        {
            DocsUtility.DocsGenerator.Editor.DocsUtility.Open(packageManifestDataForEditor.packagePath, packageManifestDataForEditor.Name, 
                packageManifestDataForEditor.version, packageManifestDataForEditor.displayName);
        }

        [TitleGroup("Validation")]
        [Button(ButtonSizes.Small, ButtonStyle.Box)]
        private void Validate()
        {
            var packName = Regex.Replace(packageManifestDataForEditor.displayName, @"((^\w)|(\.|\p{P})\w)", match => match.Value.ToUpper());
            packName = packName.Replace("-", string.Empty);
            var splitedName = packName.Split(" ");
            packName = string.Empty;
            foreach (var namePart in splitedName)
            {
                packName += namePart;
            }
            
            switch (_packageManifest.PackageType)
            {
                case "ariosoren":
                {
                    if (!Validator.Instance().PackageIsValid(packName, this))
                    {
                        logWindow.AddNewLog(LogType.ERROR, "The package is not valid!");
                        _packageIsValid = false;
                        return;
                    }

                    logWindow.AddNewLog(LogType.PASSED, "Package is valid.");
                    break;
                }
                case "external":
                {
                    if (!ExternalStandardValidator.Instance().PackageIsValid(packName, this))
                    {
                        logWindow.AddNewLog(LogType.ERROR, "The package is not valid!");
                        _packageIsValid = false;
                        return;
                    }

                    logWindow.AddNewLog(LogType.PASSED,"Package is valid.");
                    break;
                }
                default:
                {
                    if (!LegacyValidator.Instance().PackageIsValid(packName, this))
                    {
                        logWindow.AddNewLog(LogType.ERROR, "The package is not valid!");
                        _packageIsValid = false;
                        return;
                    }

                    logWindow.AddNewLog(LogType.PASSED, "Package is valid.");
                    break;
                }
            }

            _packageIsValid = true;
            Debug.unityLogger.Log("Validation did succefully . . .");
        }
        
        [TitleGroup("Validation")]
        [ShowInInspector]
        [HideLabel]
        public LogWindow logWindow;

        [ButtonGroup("bottomBtns")]
        [Button(ButtonSizes.Small, ButtonStyle.Box)]
        [EnableIf("_packageIsValid")]
        private void Publish()
        {
            var packagePath1 = packageManifestDataForEditor.packagePath;
            var artifactoryUrl = UrlData.ArtifactoryRegistry;
            Debug.Log("url     " + artifactoryUrl);
            Debug.Log("path     " + packagePath1);
            var commandModelObj1 = new CommandModel()
            {
                Command = $"npm publish --registry={artifactoryUrl}",
                RootFolder = Path.Combine(packagePath1)
            };

            var commandModels = new List<CommandModel>() {commandModelObj1};

            var executeCommandOption = new ExecuteCommandOption()
            {
                AutoCloseCommandWindow = false,
            };

            Executor.ExecuteCommand(commandModels, executeCommandOption);
        }

        

        private bool versionError;
        private string versionErrorText;
        private bool packageCreationState;
        private PackageAssemblyDefinitionManifest packageAssemblyDefinitionManifest;
        private bool previousHasRuntimeScript;
        private bool previousHasEditorScript;

        private bool updatePermission = false;

        [Button(ButtonSizes.Large), GUIColor(0, 1, 1)]
        private void UpdatePackage()
        {
            PackageManifest packageManifest = new PackageManifest();
            packageManifest.Name = packageManifestDataForEditor.Name;
            packageManifest.DisplayName = packageManifestDataForEditor.displayName;
            packageManifest.Version = packageManifestDataForEditor.version;
            packageManifest.ReferenceVersion = packageManifestDataForEditor.referenceVersion;
            packageManifest.Unity = packageManifestDataForEditor.unityVersion;
            packageManifest.UnityRelease = packageManifestDataForEditor.unityRelease;
            packageManifest.Description = packageManifestDataForEditor.Description;
            packageManifest.Host = packageManifestDataForEditor.host;
            packageManifest.DocumentationUrl = packageManifestDataForEditor.documentationUrl;
            packageManifest.ChangelogUrl = packageManifestDataForEditor.changelogUrl;
            packageManifest.License = packageManifestDataForEditor.license;
            packageManifest.LicensesUrl = packageManifestDataForEditor.licensesUrl;
            packageManifest.HideInEditor = packageManifestDataForEditor.hideInEditor;
            packageManifest.Type = packageManifestDataForEditor.type;
            packageManifest.Dependencies = packageManifestDataForEditor.packageDependencies;
            packageManifest.Keywords = packageManifestDataForEditor.keywords;
            packageManifest.Author = packageManifestDataForEditor.author;
            packageManifest.Repository = packageManifestDataForEditor.packageManifestRepository;
            packageManifest.Samples = packageManifestDataForEditor.samples;

            var jsonSerializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
            };

            var json = JsonConvert.SerializeObject(packageManifest, Formatting.Indented, jsonSerializerSettings);
            File.WriteAllText(packageManifestDataForEditor.packagePath + "/package.json", json);
            updatePermission = false;
            packageCreationState = true;
            AssetDatabase.Refresh();
        }

        private bool enableEdit;
        private bool hideEditButton = true;

        /// <summary>
        /// fix Display Name writing structure 
        /// </summary>
        private void FixDisplayName()
        {
            packageManifestDataForEditor.displayName = Regex.Replace(packageManifestDataForEditor.displayName, @"((^\w)|(\.|\p{P})\w)", match => match.Value.ToUpper());
            packageManifestDataForEditor.displayName = packageManifestDataForEditor.displayName.Replace("-", " ");
            updatePermission = true;
        }

        private void OnDescriptionChange()
        {
            updatePermission = true;
        }

        private void OnUnityVersionChange()
        {
            updatePermission = true;
        }

        /// <summary>
        /// Validate That Version String match the Pattern , Example: 01.01.01.
        /// </summary>
        /// <returns> return version string is wrongly written.</returns>
        private bool ValidateUpdateVersion()
        {
            Regex number = new Regex(@"^(\d+\.)?(\d+\.)?(\*|\d+)$");
            if (!number.IsMatch(packageManifestDataForEditor.version))
            {
                versionError = true;
                versionErrorText = "Version should be like 01.01.01";
                updatePermission = false;
                return versionError;
            }
            else
            {
                updatePermission = true;
                versionError = false;
                versionErrorText = string.Empty;
                return versionError;
            }
        }

        private void SetPackageData(PackageManifest packageManifest, string packagePath)
        {
            _packageManifest = packageManifest;
            packageManifestDataForEditor = new PackageManifestDataForEditor();
            packageManifestDataForEditor.packagePath = packagePath.Remove(packagePath.Length - 13, 13);
            packageManifestDataForEditor.Name = packageManifest.Name;
            packageManifestDataForEditor.displayName = packageManifest.DisplayName;
            packageManifestDataForEditor.version = packageManifest.Version;
            packageManifestDataForEditor.referenceVersion = packageManifest.ReferenceVersion;
            packageManifestDataForEditor.unityVersion = packageManifest.Unity;
            packageManifestDataForEditor.unityRelease = packageManifest.UnityRelease;
            packageManifestDataForEditor.Description = packageManifest.Description;
            packageManifestDataForEditor.host = packageManifest.Host;
            packageManifestDataForEditor.documentationUrl = packageManifest.DocumentationUrl;
            packageManifestDataForEditor.changelogUrl = packageManifest.ChangelogUrl;
            packageManifestDataForEditor.license = packageManifest.License;
            packageManifestDataForEditor.licensesUrl = packageManifest.LicensesUrl;
            packageManifestDataForEditor.hideInEditor = packageManifest.HideInEditor;
            packageManifestDataForEditor.type = packageManifest.Type;
            packageManifestDataForEditor.packageDependencies = packageManifest.Dependencies;
            packageManifestDataForEditor.keywords = packageManifest.Keywords;
            packageManifestDataForEditor.author = packageManifest.Author;
            packageManifestDataForEditor.packageManifestRepository = packageManifest.Repository;
            packageManifestDataForEditor.samples = packageManifest.Samples;
        }

        private void HasEditorScriptChange()
        {
            if (previousHasEditorScript && !packageManifestDataForEditor.hasEditorScript)
            {
                bool deleteEditorScripts =
                    EditorUtility.DisplayDialog("Warning!", "Are you sure to delete Package Editor scripts?", "Yes",
                        "No");
                packageManifestDataForEditor.hasEditorScript = !deleteEditorScripts;
            }

            updatePermission = true;
        }

        private void HasRuntimeScriptChange()
        {
            if (previousHasRuntimeScript && !packageManifestDataForEditor.hasRuntimeScript)
            {
                bool deleteRuntimeScripts =
                    EditorUtility.DisplayDialog("Warning!", "Are you sure to delete Package Runtime scripts?", "Yes",
                        "No");
                packageManifestDataForEditor.hasRuntimeScript = !deleteRuntimeScripts;
            }

            updatePermission = true;
        }
    }
}