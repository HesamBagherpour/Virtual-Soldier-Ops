using System;
using System.Collections.Generic;
using System.IO;
using ArioSoren.PackageTemplateUtility.Editor.Models;
using ArioSoren.PackageTemplateUtility.Editor.Models.PackageManifestModel;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEditor;

namespace ArioSoren.PackageTemplateUtility.Editor
{
    [Serializable]
    public class PackageManifestData
    {
        [HideLabel] [Title("Package Information")]
        public PackageManifestDataForEditor packageManifestDataForEditor = new PackageManifestDataForEditor();
        
        public bool IsCreatePermit()
        {
            return packageManifestDataForEditor.createPermit;
        }
        
        [DisableInEditorMode]
        [EnableIf(nameof(IsCreatePermit))]
        [Button(ButtonSizes.Large), GUIColor(0, 1, 1)]
        private void AddPackage()
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

            string json = JsonConvert.SerializeObject(packageManifest, Formatting.Indented, jsonSerializerSettings);
            string packagePascalName = packageManifestDataForEditor.displayName.Replace(" ", "");
            string packageFolder = Path.Combine(packageManifestDataForEditor.GetNameSpaceDirPath(), packagePascalName);

            if (!CheckPackageExists((packageManifestDataForEditor.GetNameSpaceDirPath())))
            {
                Directory.CreateDirectory((packageManifestDataForEditor.GetNameSpaceDirPath()));
                if (packageManifestDataForEditor.hasRuntimeScript)
                {
                    Directory.CreateDirectory(Path.Combine(packageFolder, "Scripts/Runtime"));
                    string packageName = packageManifestDataForEditor.Name;
                    string fileName = Path.Combine(packageFolder, "Scripts/Runtime/") + PackageTemplateUtilityManager.assemblyName + ".Packages." +
                                      packagePascalName +
                                      ".asmdef";
                    PackageAssemblyDefinitionManifest packageAssemblyDefinition =
                        new PackageAssemblyDefinitionManifest();
                    packageAssemblyDefinition.Name =
                        PackageTemplateUtilityManager.assemblyName + ".Packages." + packagePascalName;
                    packageAssemblyDefinition.RootNamespace = packageName;

                    JsonSerializer serializer = new JsonSerializer();
                    using (StreamWriter sw = new StreamWriter(@fileName))
                    using (JsonWriter writer = new JsonTextWriter(sw))
                    {
                        serializer.Serialize(writer, packageAssemblyDefinition);
                    }
                }

                if (packageManifestDataForEditor.hasEditorScript)
                {
                    Directory.CreateDirectory(Path.Combine(packageFolder, "Scripts/Editor"));
                    string packageName = packageManifestDataForEditor.Name;
                    string fileName = Path.Combine(packageFolder, "Scripts/Editor/") + PackageTemplateUtilityManager.assemblyName + ".Packages." +
                                      packagePascalName + ".Editor" + ".asmdef";
                    PackageAssemblyDefinitionManifest packageAssemblyDefinition =
                        new PackageAssemblyDefinitionManifest();
                    packageAssemblyDefinition.Name =
                        PackageTemplateUtilityManager.assemblyName + ".Packages." + packagePascalName + ".Editor";
                    packageAssemblyDefinition.IncludePlatforms = new List<string>()
                    {
                        "Editor"
                    };
                    packageAssemblyDefinition.RootNamespace = packageName;
                    JsonSerializer serializer = new JsonSerializer();
                    using (StreamWriter sw = new StreamWriter(@fileName))
                    using (JsonWriter writer = new JsonTextWriter(sw))
                    {
                        serializer.Serialize(writer, packageAssemblyDefinition);
                    }
                }

                Directory.CreateDirectory(Path.Combine(packageManifestDataForEditor.GetNameSpaceDirPath(), "Docutmentation"));
                File.WriteAllText(packageManifestDataForEditor.GetNameSpaceDirPath() + "/package.json", json);
                packageManifestDataForEditor.createPermit = false;
                AssetDatabase.Refresh();
            }
            else
            {
                packageManifestDataForEditor.createPermit = false;
            }
        }
        
        private bool CheckPackageExists(string path)
        {
            if (Directory.Exists(path))
            {
                return true;
            }

            return false;
        }
        
        private void CopyAssemblyDefinition(string dest, string name)
        {
            FileUtil.CopyFileOrDirectory(
                "Packages/ae.ariosoren.package-template-utility/PackageTemplateUtility/Temp/AssemblyExample.asmdef",
                dest);
            string json = File.ReadAllText(dest);
            dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
            jsonObj["name"] = name;
            string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(dest, output);
            AssetDatabase.Refresh();
        }
    }
}