using System.Collections.Generic;
using System.IO;
using ArioSoren.PackageTemplateUtility.Editor.Models.PackageManifestModel;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace ArioSoren.PackageTemplateUtility.Editor
{
    public class MainPage
    {
        [Title("packageManifestData")] [HideLabel] [ShowIf("initialData")] 
        public PackageManifestData packageManifestData;

        [HideIf("initialData")] [InfoBox("$ErrorText", InfoMessageType.Error, nameof(Error))] 
        public PackageManifestScriptableObject packageManifestScriptableObject;
        
        [HideInInspector] 
        public bool initialData;

        private bool Error;
        private string ErrorText;
        private bool ErrorScriptObject;
        
        [OnValueChanged("ValidateName")] [HideIf("initialData")] [HorizontalGroup("InitialButton")]
        [Button]
        public void Init()
        {
            if (PlayerPrefs.HasKey("PakcagePath"))
            {
                string scriptablePath = PlayerPrefs.GetString("PakcagePath");
                packageManifestScriptableObject = (PackageManifestScriptableObject) AssetDatabase.LoadAssetAtPath(scriptablePath, typeof(PackageManifestScriptableObject));
                if (packageManifestScriptableObject != null)
                {
                    Error = false;
                    ErrorText = string.Empty;
                    ErrorScriptObject = false;
                    Initial(packageManifestScriptableObject);
                }
                else
                {
                    ErrorText = "You must full ! PackageManifestScriptableObject !";
                    Error = true;
                    ErrorScriptObject = true;
                }
            }
        }

        public void Initial(PackageManifestScriptableObject scriptableObject)
        {
            packageManifestData = scriptableObject.packageManifestData;
            
            var paths = CreateAssetPaths();

            if (paths.Count > 0)
            {
                string path = paths[0];
                packageManifestData.packageManifestDataForEditor.packagePath = path;
                initialData = true;
                Error = false;
            }
            else
            {
                if (ErrorScriptObject)
                {
                    ErrorText = "You must full !PackageManifestScriptableObject! And First You should select the target folder in project window";
                }
                else
                {
                    ErrorText = "First You should select the target folder in project window ";
                }

                Error = true;
            }
        }

        [HideIf("initialData")] [HorizontalGroup("InitialButton")]
        [Button]
        public void CreateTemplateDataScriptableObject()
        {
            TemplateDataScriptableObjectCreator.ShowDialog<PackageManifestScriptableObject>(
                "Assets", obj =>
                {
                    var data = obj;
                    string path = AssetDatabase.GetAssetPath(data);
                    PlayerPrefs.SetString("PakcagePath", path);
                    Init();
                });
        }

        public static List<string> CreateAssetPaths()
        {
            List<string> assetPath = new List<string>();
            System.Object[] objs = Selection.GetFiltered<System.Object>(SelectionMode.Assets);

            foreach (Object obj in objs)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
                {
                    assetPath.Add(path);
                }
            }

            return assetPath;
        }
    }
}