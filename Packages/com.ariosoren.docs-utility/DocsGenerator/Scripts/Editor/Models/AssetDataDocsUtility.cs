using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ArioSoren.DocsUtility.DocsGenerator.Editor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using UnityEngine.Serialization;
using YamlDotNet.Serialization;

namespace ArioSoren.DocsGenerator.Editor.Models
{
    [CreateAssetMenu(fileName = "AssetData_DocsUtility", menuName = "ArioSoren/Create Docs Utility AssetData", order = 0)]
    public class AssetDataDocsUtility : SerializedScriptableObject
    {
        public string assetName;
        public string assetDisplayName;
        public string assetPath;
        public string assetVersion;

        [ShowIf("@this.docValidationState.HasFlag(DocValidationState.Documented)")] [ReadOnly]
        public string templateName;

        [FormerlySerializedAs("assemblies")] [ReadOnly]
        public List<string> rootNamespaces = new List<string>();

        private List<UnityEditor.Compilation.Assembly> _assemblies = new List<UnityEditor.Compilation.Assembly>();

        [HideInInspector] public DocFxModel docFxModel;

        [Space(10)]
        [InlineProperty]
        [ListDrawerSettings(HideRemoveButton = true, HideAddButton = true, DraggableItems = false,
            ShowIndexLabels = false, Expanded = true, ShowItemCount = false)]
        [ShowIf(
            "@this.docValidationState.HasFlag(DocValidationState.EmptyProject) || this.docValidationState.HasFlag(DocValidationState.CorruptedDocument)")]
        [InfoBox("docfx.json does not exist in Documentation folder", InfoMessageType.Error,
            "@this.docValidationState.HasFlag(DocValidationState.CorruptedDocFX)")]
        [InfoBox("toc.yml does not exist in Documentation folder", InfoMessageType.Error,
            "@this.docValidationState.HasFlag(DocValidationState.CorruptedTOC)")]
        public List<DocTemplate> templates;

        [HideReferenceObjectPicker, HideLabel]
        [HorizontalGroup("Group2")]
        [Space(10), PropertyOrder(50)]
        [ShowIf("@this.docValidationState.HasFlag(DocValidationState.Documented)")]
        public OdinMenuTree tree;

        [HideInInspector] public DocValidationState docValidationState = DocValidationState.None;

        [ContextMenu("Validate")]
        public void OnValidate()
        {
            // Debug.Log("OnValidate");

            ValidateDocFX();
            UpdateTemplates();
            BuildPreviewMenuTree();
            GetAllAssemblies(docValidationState == DocValidationState.Documented);
        }

        public static List<AssetDataDocsUtility> LoadAllAssets()
        {
            // Debug.Log("LoadAllAssets");

            var packages = AssetDatabase.FindAssets("t:AssetDataDocsUtility", new[] { "Packages/" }).ToList();
            packages.AddRange(AssetDatabase.FindAssets("t:AssetDataDocsUtility", new[] { "Assets/" }));

            List<AssetDataDocsUtility> assets = new List<AssetDataDocsUtility>();
            for (int i = 0; i < packages.Count; i++)
            {
                var path = AssetDatabase.GUIDToAssetPath(packages[i]);
                var loadAssetAtPath = (AssetDataDocsUtility)AssetDatabase.LoadAssetAtPath(path, typeof(AssetDataDocsUtility));
                loadAssetAtPath.ValidateDocFX();
                assets.Add(loadAssetAtPath);
                // Debug.Log($"{loadAssetAtPath.assetName}");
            }

            return assets;
        }

        

        private void GetAllAssemblies(bool isDocumented)
        {
            // Debug.Log("GetAllAssemblies");
            #region Get all asseblies

            rootNamespaces.Clear();
            _assemblies.Clear();
            var enumerable = CompilationPipeline.GetAssemblies();
            foreach (var assembly in enumerable)
            {
                if (!assembly.outputPath.Contains(assetName.Replace(" ", ""))) continue;
                rootNamespaces.Add(assembly.name);
                _assemblies.Add(assembly);
            }

            if (!isDocumented) return;

            #endregion

            var stringBuilder = new StringBuilder();

            if (_assemblies.Count <= 0)
            {
                string NoAPITemplate = @"apiRules:
  - exclude:
      uidRegex: ^*.
      type: Namespace";

                stringBuilder.Append(NoAPITemplate);
            }
            else
            {
                string NoAPITemplate = $@"apiRules:
  - include:
      uidRegex: {_assemblies[0].rootNamespace}
      type : Namespace";

                stringBuilder.Append(NoAPITemplate);
            }

            var stringWriter = new StringWriter(stringBuilder);
            var serializer = new Serializer();
            File.WriteAllText(assetPath + "/Documentation/filterConfig.yml", stringBuilder.ToString());

            
            string text = File.ReadAllText(new FileInfo(assetPath + "/Documentation/docfx.json").FullName);

            string all = string.Join(",", rootNamespaces.Select(s => $"\"{s}.csproj\""));

            text = text.Replace("\"[[FilesName]]\"", all);
            File.WriteAllText(new FileInfo(assetPath + "/Documentation/docfx.json").FullName, text);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void UpdateTemplates()
        {
            // Debug.Log("UpdateTemplates");

            if (!docValidationState.HasFlag(DocValidationState.EmptyProject) ||
                !docValidationState.HasFlag(DocValidationState.CorruptedDocument)) return;

            templates.Clear();

            // read through documentation folder for templates
            const string _path =
                "Packages/com.ariosoren.docs-utility/Templates";
            DirectoryInfo dir = new DirectoryInfo(_path);
            DirectoryInfo[] info = dir.GetDirectories("*.*");
            int count = dir.GetDirectories().Length;


            for (int i = 0; i < count; i++)
            {
                var newTemplate = new DocTemplate
                {
                    displayType = info[i].Name,
                    templatePath = _path + "/" + info[i].Name,
                    docfxPath = _path + "/" + info[i].Name + "/docfx.json",
                    selfAssetData = this
                };

                templates.Add(newTemplate);
            }
        }

        public void ValidateDocFX()
        {
            docValidationState = DocValidationState.None;

            #region DocFx Exists?

            if (!Directory.Exists(assetPath + "/" +
                                  "Documentation")) //? Check weather documentation folder exists or not
            {
                docValidationState = DocValidationState.EmptyProject;
            }
            else
            {
                if (!File.Exists(assetPath +
                                 "/Documentation/docfx.json")) //? Check weather docfx.json file exists or not
                {
                    docValidationState = DocValidationState.CorruptedDocument | DocValidationState.CorruptedDocFX;
                }

                if (!File.Exists(assetPath + "/Documentation/toc.yml")) //? Check weather toc.yml file exists or not
                {
                    docValidationState = DocValidationState.CorruptedDocument | DocValidationState.CorruptedTOC |
                                         docValidationState;
                }
            }

            if (docValidationState == DocValidationState.None)
            {
                docValidationState = DocValidationState.Documented;
            }

            #endregion
        }

        public void GenerateDocFX(string templateType)
        {
            string mSource =
                "Packages/com.ariosoren.docs-utility/Templates/" +
                templateType;

            // if (Directory.Exists(dest))
            // {
            //     DeleteDocumentation();
            // }

            // FileUtil.CopyFileOrDirectory(mSource, dest);
            AssetDatabase.Refresh();
            docValidationState = DocValidationState.Documented;
        }

        [ShowIf("@this.docValidationState.HasFlag(DocValidationState.Documented)")]
        [Button("Preview", ButtonSizes.Medium)]
        [GUIColor("@Color.green")]
        public void Preview()
        {
            Utility.ExecuteCommandAllStage(assetPath + "/Documentation/docfx.json");
        }

        [ShowIf("@this.docValidationState.HasFlag(DocValidationState.CorruptedDocFX)")]
        [InfoBox("docfx.json does not exist in Documentation folder", InfoMessageType.Error)]
        [GUIColor("@Color.red")]
        public void Fix_FXDoc_Json()
        {
            // string m_source = "Packages/com.ariosoren.docs-utility/Documentation/" + templatePath;

            // FileUtil.CopyFileOrDirectory(m_source, assetPath + "/Documentation");
            // AssetDatabase.Refresh();
        }

        [ShowIf("@this.docValidationState.HasFlag(DocValidationState.CorruptedTOC)")]
        [InfoBox("toc.yml does not exist in Documentation folder", InfoMessageType.Error)]
        [GUIColor("@Color.red")]
        public void Fix_Root_TOC()
        {
        }

        public void DeleteDocumentation(bool useConfirmation = false)
        {
            if (!useConfirmation)
            {
                AssetDatabase.DeleteAsset(assetPath + "/Documentation");
                OnValidate();
                AssetDatabase.Refresh();
                return;
            }

            DocsEditorWindow.DeleteConfirmation(() =>
            {
                AssetDatabase.DeleteAsset(assetPath + "/Documentation");
                OnValidate();
                AssetDatabase.Refresh();
            });
        }

        private void BuildPreviewMenuTree()
        {
            // Debug.Log("BuildPreviewMenuTree");
            if (!docValidationState.HasFlag(DocValidationState.Documented)) return;

            this.tree = new OdinMenuTree();

            tree.DefaultMenuStyle = OdinMenuStyle.TreeViewStyle;
            tree.DefaultMenuStyle.IconSize = 28.00f;
            tree.Config.DrawSearchToolbar = true;
            tree.DefaultMenuStyle.DrawFoldoutTriangle = true;
            tree.Config.DefaultMenuStyle.IconSize = 20;


            var allAssets = AssetDatabase.GetAllAssetPaths()
                .Where(x => x.EndsWith("toc.yml") && x.StartsWith(assetPath) && !x.Contains("Template"))
                .OrderBy(x => x);

            var docTreeFiles = Utility.GetAllAssetPaths(allAssets.ToList()[allAssets.ToList().Count - 1]);

            foreach (var path in docTreeFiles)
            {
                List<string> list = path.Split("/").ToList();
                int index = list.FindIndex(x => x == "Documentation");
                string newPath = string.Empty;

                for (int i = index + 1; i < list.Count; i++)
                {
                    newPath += list[i] + "/";
                }

                tree.AddAssetAtPath(newPath, newPath);
            }

            tree.EnumerateTree().ForEach(x => x.Toggled = true);
            tree.EnumerateTree().ForEach(x => DocsEditorWindow.DrawEdit(x, this, allAssets.ToArray()));

            tree.Selection.SelectionConfirmed += (value) =>
            {
                // window = new OdinEditorWindow();
                // window.DrawUnityEditorPreview = true;
                // var btnRect = GUIHelper.GetCurrentLayoutRect();
                // window = OdinEditorWindow.InspectObject(window, new Stuff());
                // window.Repaint();
                // window.Show();
            };
        }
    }

    [Serializable]
    public class DocTemplate
    {
        [HorizontalGroup("Group1")] [ReadOnly] [HideLabel]
        public string displayType;

        [HideInInspector] public string templatePath;
        [HideInInspector] public string docfxPath;
        [HideInInspector] public string assetPath;

        [HideInInspector] public AssetDataDocsUtility selfAssetData;
        // [HideInInspector] public DocFxModel docFxModel;

        [HorizontalGroup("Group1")]
        [Button("Generate")]
        [GUIColor("@Color.yellow")]
        public void Generate()
        {
            selfAssetData.GenerateDocFX(displayType);
            selfAssetData.docFxModel = Utility.ExtractDocFxJson(docfxPath);
            selfAssetData.templateName = selfAssetData.docFxModel.templateData.templateName;
        }

        [HorizontalGroup("Group1")]
        [Button("Preview")]
        [GUIColor("@Color.yellow")]
        public void Preview()
        {
            // Utility.ExecuteCommand(docfxPath, "D:/Workspace/media-wall/Temp/DocBuild");
        }
    }

    [System.Serializable]
    public class Stuff
    {
        public int ads = 5;
    }

    [System.Flags]
    public enum DocValidationState
    {
        None = 0,
        Documented = 2,
        EmptyProject = 4,
        CorruptedDocument = 8,
        CorruptedDocFX = 16,
        CorruptedTOC = 32
    }


    public class DocumentationInfo
    {
        public bool isDocumented;
        public string path;
    }
}