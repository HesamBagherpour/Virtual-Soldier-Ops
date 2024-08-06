using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ArioSoren.DocsGenerator.Editor.Models;
using ArioSoren.DocsUtility.DocsGenerator.Editor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using YamlDotNet.Serialization;
using Directory = UnityEngine.Windows.Directory;

namespace ArioSoren.DocsUtility.DocsGenerator.Editor
{
    public sealed class DocsUtility : OdinEditorWindow
    {
        #region Exposed Vars

        [ShowInInspector, DisableIf("isEditMode"), OnValueChanged("Search")]
        public static string _docName;

        [ShowInInspector, DisableIf("isEditMode"), OnValueChanged("Search")]
        public static string _docVersion;

        [ShowInInspector, DisableIf("isEditMode"), OnValueChanged("Search")]
        public static string _docDisplayName;

        [ShowInInspector, FolderPath(RequireExistingPath = true),
         OnValueChanged("Search"), OnValueChanged("EditOff")]
        public static string _docRelativePath;

        [ShowInInspector, ShowIf("_cachedDocument")]
        [Sirenix.OdinInspector.ReadOnly]
        public static string _docTempalteName;

        [ShowInInspector, ShowIf("_cachedDocument")]
        [Sirenix.OdinInspector.ReadOnly]
        public static string _docTempalteVersion;

        [Space(10)]
        [ShowInInspector, Sirenix.OdinInspector.ReadOnly, HideReferenceObjectPicker, ShowIf("_cachedAssemblies")]
        private static List<string> _docAssemblies = new List<string>();


        private static List<UnityEditor.Compilation.Assembly>
            _assemblies = new List<UnityEditor.Compilation.Assembly>();

        [Space(10), HideIf("_cachedDocument"), ListDrawerSettings(HideRemoveButton = true, HideAddButton = true,
             DraggableItems = false,
             ShowIndexLabels = false, Expanded = true, ShowItemCount = false), HideReferenceObjectPicker,
         ShowInInspector,
        ]
        private static List<DocTemplate> _templates;

        [HideReferenceObjectPicker, HideLabel, ShowInInspector]
        // [HorizontalGroup("Group2")]
        [Space(10), PropertyOrder(50)]
        [ShowIf("_cachedDocument")]
        public static OdinMenuTree tree;

        #endregion

        public static DocFxModel _docFxModel { get; set; }
        private static bool _cachedDocument;
        public static bool _cachedValidFolder;
        private static bool _cachedAssemblies;

        private static string result = string.Empty;
        private static Color textFieldColor = Color.black;
        [HideInInspector] public static bool isEditMode = true;

        public const string DOCUMENTATION_FOLDER_TAG = "/Documentation";
        public const string DOCFX_UTILITY_JSON_TAG = "/DocfxUtility.json";


        /// <summary>
        /// opens up an empty document. user must pick up a documented folder or create a new one
        /// </summary>
        [MenuItem("ArioSoren/Packages/DocsGenerator/Init New Doc")]
        public static void Open()
        {
            isEditMode = false;
            OpenEditorWindow();
            ResetFields();
            Search();
        }

        /// <summary>
        /// opens up a documented package
        /// </summary>
        /// <param name="path"></param>
        public static void Open(string path, string name = null, string version = null, string displayName = null)
        {
            _docRelativePath = path;
            _docName = name;
            _docVersion = version;
            _docDisplayName = displayName;
            isEditMode = false;
            OpenEditorWindow();
            Search();
        }

        public static void Fill(string path, string name = null, string version = null, string displayName = null)
        {
            _docRelativePath = path;
            _docName = name;
            _docVersion = version;
            _docDisplayName = displayName;
            isEditMode = false;
            // OpenEditorWindow();
            Search();
        }


        private static void OpenEditorWindow()
        {
            var window = GetWindow<DocsUtility>("Docs Utility");
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 500);
        }

        private void OnValidate()
        {
            Search();
        }

        private void FixDocRelativePath()
        {
            if (_docRelativePath.StartsWith("Packages/") && _docRelativePath.Contains(" "))
            {
                GetAllPackageList();
            }
        }
        static ListRequest Request;
        static void GetAllPackageList()
        {
            Request = Client.List();
            EditorApplication.update += Progress;
        }
        static void Progress()
        {
            if (Request.IsCompleted)
            {
                if (Request.Status == StatusCode.Success)
                    foreach (var package in Request.Result)
                    {
                        // Debug.Log("Package name: " + package.displayName);
                        if (_docRelativePath.Contains(package.displayName))
                        {
                            _docRelativePath.Replace(package.displayName, package.name);
                            break;
                        }
                    }
                EditorApplication.update -= Progress;
                Search();
            }
        }

        public static void Search()
        {
            // if (_docRelativePath.StartsWith("Packages/") && _docRelativePath.Contains("@"))
            // {
            //     _docRelativePath = _docRelativePath.Substring(0, _docRelativePath.IndexOf('@'));
            // }

            _cachedValidFolder = Directory.Exists(_docRelativePath);
            if (string.IsNullOrEmpty(_docRelativePath)
                || string.IsNullOrEmpty(_docName)
                || string.IsNullOrEmpty(_docVersion)
                || string.IsNullOrEmpty(_docDisplayName)) _cachedValidFolder = false;

            var hasExtracted = Utility.TryExtractJson<DocfxUtilityJson>(
                _docRelativePath + DOCUMENTATION_FOLDER_TAG + DOCFX_UTILITY_JSON_TAG,
                out DocfxUtilityJson extractedJson);

            _cachedDocument = hasExtracted;

            _docAssemblies.Clear();
            _assemblies.Clear();
            if (hasExtracted && isEditMode)
            {
                if ((extractedJson.docName != _docName
                     || extractedJson.docVersion != _docVersion
                     || extractedJson.docDisplayName != _docDisplayName))
                {
                    OverridePrompt(() =>
                    {
                        extractedJson.docName = _docName;
                        extractedJson.docrelativePath = _docRelativePath;
                        extractedJson.docVersion = _docVersion;
                        extractedJson.docDisplayName = _docDisplayName;

                        Utility.TryCreateJson(extractedJson,
                            file: DocsUtility._docRelativePath + DocsUtility.DOCUMENTATION_FOLDER_TAG +
                                  DocsUtility.DOCFX_UTILITY_JSON_TAG);
                    }, () =>
                    {
                        UpdateFields(extractedJson);
                        BuildPreviewMenuTree();
                    });
                }
                else
                {
                    UpdateFields(extractedJson);
                    BuildPreviewMenuTree();
                    // GetAllAssemblies(true);
                }

                // UpdateAssemblies();

                GetAllAssemblies(true);
            }
            else
            {
                // ResetFields();
                UpdateTemplates();
            }
        }


        [ShowIf("_cachedDocument")]
        [Button("Preview", ButtonSizes.Medium)]
        [GUIColor("@Color.green")]
        public void Preview()
        {
            Utility.ExecuteCommandAllStage(new FileInfo(_docRelativePath + "/Documentation/docfx.json").FullName);
        }

        [ShowIf("_cachedDocument")]
        [Button("Clear Build", ButtonSizes.Medium)]
        [GUIColor("@Color.yellow")]
        public void ClearBuild()
        {
            Utility.DeleteDirectory(Path.Combine(_docRelativePath, "Documentation", "_site~"));
        }

        [ShowIf("_cachedDocument")]
        [Button("Edit", ButtonSizes.Medium)]
        [GUIColor("@Color.green")]
        public static void EditFields()
        {
            isEditMode = !isEditMode;
            Search();
        }

        public static void EditOff()
        {
            Search();
            isEditMode = DocsUtility._cachedDocument;
            Search();
        }

        private static void GetAllAssemblies(bool isDocumented)
        {
            #region Get all asseblies

            _docAssemblies = new List<string>();
            _assemblies = new List<Assembly>();

            Assembly[] enumerable = CompilationPipeline.GetAssemblies();
            foreach (var assembly in enumerable)
            {
                if (!assembly.sourceFiles[0].Contains(_docName)) continue;
                _docAssemblies.Add(assembly.name);
                _assemblies.Add(assembly);
            }

            if (!isDocumented) return;
            _cachedAssemblies = _docAssemblies.Count > 0;

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
            // File.WriteAllText(new FileInfo(_docRelativePath + "/Documentation/filterConfig.yml").FullName,
            //     stringBuilder.ToString());

            string text = File.ReadAllText(new FileInfo(_docRelativePath + "/Documentation/docfx.json").FullName);

            string all = string.Join(",", _docAssemblies.Select(s => $"\"{s}.csproj\""));

            text = text.Replace("\"[[FilesName]]\"", all);
            File.WriteAllText(new FileInfo(_docRelativePath + "/Documentation/docfx.json").FullName, text);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

        }

        private static void BuildPreviewMenuTree()
        {
            tree = new OdinMenuTree();
            tree.DefaultMenuStyle = OdinMenuStyle.TreeViewStyle;
            tree.DefaultMenuStyle.IconSize = 28.00f;
            tree.Config.DrawSearchToolbar = true;
            tree.DefaultMenuStyle.DrawFoldoutTriangle = true;
            tree.Config.DefaultMenuStyle.IconSize = 20;

            var ymls = System.IO.Directory.GetFiles(_docRelativePath + DOCUMENTATION_FOLDER_TAG + "/", "toc.yml",
                SearchOption.AllDirectories).ForEach(x => x = new FileInfo(x).FullName).ToList();

            var indexOfAPi = ymls.FindIndex(x => x.Contains("/_api~"));
            if (indexOfAPi != -1) ymls.RemoveAt(indexOfAPi);
            var docTreeFiles = Utility.GetAllAssetPaths(ymls.ToList()[0]);

            foreach (var path in docTreeFiles)
            {
                List<string> list = path.Split("/").ToList();
                int index = list.FindIndex(x => x == "Documentation");
                string newPath = string.Empty;

                for (int i = index + 1; i < list.Count; i++)
                {
                    newPath += list[i] + "/";
                }

                if (newPath == "_api~//") continue;
                tree.AddAssetAtPath(newPath, newPath);
            }

            tree.EnumerateTree().ForEach(x => x.Toggled = true);
            tree.EnumerateTree().ForEach(x => DrawEdit(x, ymls.ToArray()));
        }

        private static void ResetFields()
        {
            _docName = String.Empty;
            _docDisplayName = String.Empty;
            _docVersion = String.Empty;
            _docRelativePath = String.Empty;
            _docAssemblies.Clear();
            _assemblies.Clear();
        }

        private static void UpdateFields(DocfxUtilityJson extractedJson)
        {
            Utility.TryExtractJson(_docRelativePath + DOCUMENTATION_FOLDER_TAG + DOCFX_UTILITY_JSON_TAG,
                out DocFxModel _docFxModel);

            _docName = extractedJson.docName;
            _docVersion = extractedJson.docVersion;
            _docDisplayName = extractedJson.docDisplayName;
            _docTempalteName = _docFxModel.templateData.templateName;
            _docTempalteVersion = _docFxModel.templateData.version;
        }

        private static void UpdateAssemblies()
        {
            _docAssemblies = new List<string>();
            if (_docRelativePath == String.Empty) return;

            var enumerable = CompilationPipeline.GetAssemblies();
            var keyword = _docRelativePath.Split("/");

            foreach (var assembly in enumerable)
            {
                if (!assembly.outputPath.Contains(keyword[^1])) continue;
                _docAssemblies.Add(assembly.outputPath);

                _cachedAssemblies = _docAssemblies.Count > 0;
            }
        }

        private static void UpdateTemplates()
        {
            _templates = new List<DocTemplate>();

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
                    templateType = info[i].Name,
                    templatePath = _path + "/" + info[i].Name,
                    docfxPath = _path + "/" + info[i].Name + "/docfx.json",
                };

                _templates.Add(newTemplate);
            }
        }

        public static void DrawEdit(OdinMenuItem menuItem, string[] allYamlFiles)
        {
            menuItem.OnDrawItem += x =>
            {
                var isBand = !menuItem.GetFullPath().Contains("manual");
                // menuItem.Style.SetDrawFoldoutTriangle(!isBand);
                if (!menuItem.IsSelected) return;

                var isFile = menuItem.SmartName.Contains(".md");

                #region Button rects

                var firstButtonRect = new Rect(new Vector2(menuItem.Rect.width - 25, menuItem.Rect.y + 3),
                    new Vector2(20, 20));
                var secondButtonRect = new Rect(new Vector2(menuItem.Rect.width - 50, menuItem.Rect.y + 3),
                    new Vector2(20, 20));
                var thirdButtonRect = new Rect(new Vector2(menuItem.Rect.width - 75, menuItem.Rect.y + 2),
                    new Vector2(20, 20));
                var fourthButtonRect = new Rect(new Vector2(menuItem.Rect.width - 95, menuItem.Rect.y + 3),
                    new Vector2(20, 20));
                var fifthButtonRect = new Rect(new Vector2(menuItem.Rect.width - 115, menuItem.Rect.y + 1),
                    new Vector2(20, 20));
                var textFieldRect = new Rect(new Vector2(menuItem.Rect.width - 260, menuItem.Rect.y + 2),
                    new Vector2(140, 20));

                #endregion

                // Del button
                if (!isBand)
                    if (SirenixEditorGUI.IconButton(firstButtonRect, EditorIcons.X))
                    {
                        DeleteConfirmation(() =>
                        {
                            DeleteFileOrDirectory(allYamlFiles, menuItem.Name);
                            Search();
                        });
                    }

                if (isFile)
                {
                    // Edit file button
                    if (SirenixEditorGUI.IconButton(secondButtonRect, EditorIcons.Info))
                    {
                        string value =
                            Utility.LoadFile(_docRelativePath + "/Documentation/" + menuItem.GetFullPath());
                        var window = OdinEditorWindow.InspectObject(new ReadMd(value,
                            _docRelativePath + "/Documentation/" + menuItem.GetFullPath()));
                        window.position = GUIHelper.GetEditorWindowRect().AlignCenter(1000, 500);
                        window.Show(true);
                    }

                    if (isBand) return;

                    if (CanMoveFile(allYamlFiles, menuItem, up: true))
                    {
                        if (SirenixEditorGUI.IconButton(thirdButtonRect, EditorIcons.ArrowUp))
                        {
                            MoveFileElement(allYamlFiles, menuItem, up: true);
                            Search();
                        }
                    }

                    if (CanMoveFile(allYamlFiles, menuItem, up: false))
                    {
                        if (SirenixEditorGUI.IconButton(fourthButtonRect, EditorIcons.ArrowDown))
                        {
                            MoveFileElement(allYamlFiles, menuItem, up: false);
                            Search();
                        }
                    }
                }
                else
                {
                    if (isBand) return;
                    FontStyle fontStyleTest = FontStyle.Normal;

                    result = SirenixEditorFields.TextField(textFieldRect, null, result, new GUIStyle()
                    {
                        normal = new GUIStyleState()
                        {
                            textColor = textFieldColor,
                            // background = (Texture2D)EditorGUILayout.ObjectField(Texture2D.redTexture, typeof(Texture2D),
                            //     false,
                            //     GUILayout.Width(70), GUILayout.Height(70))
                            background = new Texture2D(70, 70)
                        },
                        fontStyle = FontStyle.Bold,
                        fontSize = 16,
                        padding = new RectOffset(5, 0, 0, 0)
                    });


                    if (result != String.Empty &&
                        menuItem.ChildMenuItems.All(x => Utility.RemoveLastFileExtension(x.Name) != result))
                    {
                        textFieldColor = Color.black;
                        if (SirenixEditorGUI.IconButton(secondButtonRect, EditorIcons.File))
                        {
                            AddFile(menuItem, result);
                            Search();
                            menuItem.MenuTree.UpdateMenuTree();
                            result = String.Empty;
                        }

                        if (SirenixEditorGUI.IconButton(thirdButtonRect, EditorIcons.Folder))
                        {
                            AddFolder(menuItem, result);
                            Search();
                            menuItem.MenuTree.UpdateMenuTree();
                            result = String.Empty;
                        }
                    }
                    else
                    {
                        textFieldColor = Color.red;
                    }

                    if (CanMoveFolder(allYamlFiles, menuItem, up: true))
                    {
                        if (SirenixEditorGUI.IconButton(fifthButtonRect, EditorIcons.ArrowUp))
                        {
                            MoveFolderElement(allYamlFiles, menuItem, up: true);
                            Search();
                        }
                    }

                    if (CanMoveFolder(allYamlFiles, menuItem, up: false))
                    {
                        if (SirenixEditorGUI.IconButton(fourthButtonRect, EditorIcons.ArrowDown))
                        {
                            MoveFolderElement(allYamlFiles, menuItem, up: false);
                            Search();
                        }
                    }
                }
            };
        }

        private static void AddFolder(OdinMenuItem menuItem, string fileName)
        {
            var assetDataAssetPath = _docRelativePath + "/Documentation/" +
                                     menuItem.GetFullPath() + "/toc.yml";
            string yml = Utility.LoadFile(assetDataAssetPath);

            var data = Utility.Deserializer(yml);
            if (data.content == null)
            {
                data.content = new List<TOCContent>();
            }

            data.content.Add(new TOCContent()
            {
                name = fileName,
                href = fileName + "/toc.yml"
            });

            Directory.CreateDirectory(_docRelativePath + "/Documentation/" +
                                      menuItem.GetFullPath() + "/" + fileName);
            var docRelativePath = _docRelativePath + "/Documentation/" +
                                  menuItem.GetFullPath() + "/" + fileName + "/" + "toc.yml";
            using (FileStream fs = File.Create(docRelativePath))
            {
            }

            File.WriteAllText(docRelativePath, "[]");

            var stringBuilder = new StringBuilder();
            var stringWriter = new StringWriter(stringBuilder);

            var serializer = new Serializer();
            serializer.Serialize(stringWriter, data.content);

            File.WriteAllText(assetDataAssetPath, stringBuilder.ToString());
        }

        private static void AddFile(OdinMenuItem menuItem, string fileName)
        {
            var assetDataAssetPath = _docRelativePath + "/Documentation/" +
                                     menuItem.GetFullPath() + "/toc.yml";
            string yml = Utility.LoadFile(assetDataAssetPath);

            var data = Utility.Deserializer(yml);
            if (data.content == null)
            {
                data.content = new List<TOCContent>();
            }

            data.content.Add(new TOCContent()
            {
                name = fileName,
                href = fileName + ".md"
            });

            using (FileStream fs = File.Create(_docRelativePath + "/Documentation/" +
                                               menuItem.GetFullPath() + "/" + fileName + ".md"))
            {
            }


            var stringBuilder = new StringBuilder();
            var stringWriter = new StringWriter(stringBuilder);

            var serializer = new Serializer();
            serializer.Serialize(stringWriter, data.content);

            File.WriteAllText(assetDataAssetPath, stringBuilder.ToString());
        }


        public static bool CanMoveFolder(string[] allYamlFiles, OdinMenuItem targetFileOrDirectory, bool up)
        {
            string yml = String.Empty;
            TOC data;

            // int index = allYamlFiles.ToList().FindIndex(x => x.Contains(Utility.RemoveLastDirSlash(targetFileOrDirectory.GetFullPath())));


            for (int i = 0; i < allYamlFiles.Length; i++)
            {
                // if (File.ReadAllText(allYamlFiles[i]) == String.Empty) continue;

                // #1- Read Through File
                try
                {
                    using (StreamReader reader = new StreamReader(allYamlFiles[i]))
                    {
                        yml = reader.ReadToEnd();
                    }
                }
                catch
                {
                    allYamlFiles.ToList().RemoveAt(i);
                }


                // #2- Find it into all yaml file
                // Cache yaml
                data = Utility.Deserializer(yml);
                if (yml == String.Empty) continue;
                int selectedIndex = data.content.FindIndex(x => x.href.Contains(targetFileOrDirectory.Name));
                if (selectedIndex == -1) continue;

                // Swap element
                if (up && selectedIndex > 0)
                {
                    return true;
                }

                if (!up && selectedIndex < data.content.Count - 1)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool CanMoveFile(string[] allYamlFiles, OdinMenuItem targetFileOrDirectory, bool up)
        {
            string yml;
            TOC data;

            int index = allYamlFiles.ToList().FindIndex(x =>
                x.Contains(Utility.RemoveLastDirSlash(targetFileOrDirectory.GetFullPath())));

            if (index == -1)
            {
                for (int i = 0; i < allYamlFiles.Length; i++)
                {
                    // #1- Read Through File
                    using (StreamReader reader = new StreamReader(allYamlFiles[i]))
                    {
                        yml = reader.ReadToEnd();
                    }

                    // #2- Find it into all yaml file
                    if (yml.Contains(targetFileOrDirectory.Name))
                    {
                        index = i;
                    }
                }
            }

            if (index == -1) return false;

            // #1- Read Through File
            using (StreamReader reader = new StreamReader(allYamlFiles[index]))
            {
                yml = reader.ReadToEnd();
            }

            // #2- Find it into all yaml file

            // Cache yaml
            data = Utility.Deserializer(yml);
            int selectedIndex = data.content.FindIndex(x => x.href.Contains(targetFileOrDirectory.Name));

            // Swap element
            if (up && selectedIndex > 0)
            {
                return true;
            }
            else if (!up && selectedIndex < data.content.Count - 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static void MoveFileElement(string[] allYamlFiles, OdinMenuItem targetFileOrDirectory, bool up)
        {
            string yml;
            TOC data;

            int index = allYamlFiles.ToList().FindIndex(x =>
                x.Contains(Utility.RemoveLastDirSlash(targetFileOrDirectory.GetFullPath())));


            if (index == -1)
            {
                for (int i = 0; i < allYamlFiles.Length; i++)
                {
                    // #1- Read Through File
                    using (StreamReader reader = new StreamReader(allYamlFiles[i]))
                    {
                        yml = reader.ReadToEnd();
                    }

                    // #2- Find it into all yaml file
                    if (yml.Contains(targetFileOrDirectory.Name))
                    {
                        index = i;
                    }
                }
            }

            // #1- Read Through File
            using (StreamReader reader = new StreamReader(allYamlFiles[index]))
            {
                yml = reader.ReadToEnd();
            }

            // #2- Find it into all yaml file
            // Cache yaml
            data = Utility.Deserializer(yml);
            int selectedIndex = data.content.FindIndex(x => x.href.Contains(targetFileOrDirectory.Name));

            // Swap element
            if (up && selectedIndex > 0)
            {
                TOCContent temp;

                temp = data.content[selectedIndex];
                data.content[selectedIndex] = data.content[selectedIndex - 1];
                data.content[selectedIndex - 1] = temp;
            }

            if (!up && selectedIndex < data.content.Count - 1)
            {
                TOCContent temp;

                temp = data.content[selectedIndex];
                data.content[selectedIndex] = data.content[selectedIndex + 1];
                data.content[selectedIndex + 1] = temp;
            }

            var stringBuilder = new StringBuilder();
            var stringWriter = new StringWriter(stringBuilder);

            var serializer = new Serializer();
            serializer.Serialize(stringWriter, data.content);

            File.WriteAllText(allYamlFiles[index], stringBuilder.ToString());
        }

        private static void MoveFolderElement(string[] allYamlFiles, OdinMenuItem targetFileOrDirectory, bool up)
        {
            string yml;
            TOC data;

            // int index = allYamlFiles.ToList().FindIndex(x => x.Contains(Utility.RemoveLastDirSlash(targetFileOrDirectory.GetFullPath())));


            for (int i = 0; i < allYamlFiles.Length; i++)
            {
                // #1- Read Through File
                using (StreamReader reader = new StreamReader(allYamlFiles[i]))
                {
                    yml = reader.ReadToEnd();
                }

                // #2- Find it into all yaml file
                // Cache yaml
                data = Utility.Deserializer(yml);
                int selectedIndex = data.content.FindIndex(x => x.href.Contains(targetFileOrDirectory.Name));
                if (selectedIndex == -1) continue;

                // Swap element
                if (up && selectedIndex > 0)
                {
                    TOCContent temp;

                    temp = data.content[selectedIndex];
                    data.content[selectedIndex] = data.content[selectedIndex - 1];
                    data.content[selectedIndex - 1] = temp;
                }

                if (!up && selectedIndex < data.content.Count - 1)
                {
                    TOCContent temp;

                    temp = data.content[selectedIndex];
                    data.content[selectedIndex] = data.content[selectedIndex + 1];
                    data.content[selectedIndex + 1] = temp;
                }

                var stringBuilder = new StringBuilder();
                var stringWriter = new StringWriter(stringBuilder);

                var serializer = new Serializer();
                serializer.Serialize(stringWriter, data.content);

                File.WriteAllText(allYamlFiles[i], stringBuilder.ToString());
            }
        }

        public static void DeleteConfirmation(Action callback = null)
        {
            bool option = EditorUtility.DisplayDialog("Delete documentation",
                "Do you want to delete the documentation?",
                "Yes",
                "Cancel");

            if (option)
            {
                callback?.Invoke();
            }
        }

        public static void OverridePrompt(Action callback = null, Action callbackCancel = null)
        {
            bool option = EditorUtility.DisplayDialog("Would you Like to override?",
                "", "Yes",
                "No");

            if (option)
            {
                callback?.Invoke();
            }
            else
            {
                callbackCancel?.Invoke();
            }
        }

        public static void DeleteFileOrDirectory(string[] allYamlFiles, string targetFileOrDirectory)
        {
            string yml;
            TOC data;
            for (int index = 0; index < allYamlFiles.Length; index++)
            {
                // #1- Read Through File
                using (StreamReader reader = new StreamReader(allYamlFiles[index]))
                {
                    yml = reader.ReadToEnd();
                }

                // #2- Find it into all yaml file
                if (yml.Contains(targetFileOrDirectory))
                {
                    data = Utility.Deserializer(yml);
                    int indexToRemove = data.content.FindIndex(x => x.href.Contains(targetFileOrDirectory));
                    string pathToRemove = String.Empty;
                    if (data.content[indexToRemove].href.Contains("toc.yml"))
                    {
                        pathToRemove = Utility.RemoveLastDirSlash(allYamlFiles[index].Replace("\\", "/")) + "/" +
                                       Utility.RemoveLastDirSlash(data.content[indexToRemove].href);
                    }
                    else
                    {
                        pathToRemove = Utility.RemoveLastDirSlash(allYamlFiles[index].Replace("\\", "/")) + "/" +
                                       data.content[indexToRemove].href;
                    }

                    // MyCustomEditorWindow.DeleteFileFolder(pathToRemove);
                    // System.IO.Directory.Delete(pathToRemove, true);
                    UnityEditor.AssetDatabase.DeleteAsset(pathToRemove);
                    UnityEditor.AssetDatabase.Refresh();
                    data.content.RemoveAt(indexToRemove);

                    var stringBuilder = new StringBuilder();
                    var stringWriter = new StringWriter(stringBuilder);

                    var serializer = new Serializer();
                    serializer.Serialize(stringWriter, data.content);

                    File.WriteAllText(allYamlFiles[index], stringBuilder.ToString());
                    BuildPreviewMenuTree();
                    break;
                }
            }
        }
    }
}


[Serializable]
public class DocTemplate
{
    [HorizontalGroup("Group1")]
    [Sirenix.OdinInspector.ReadOnly]
    [HideLabel]
    public string templateType;

    [HideInInspector] public string templatePath;
    [HideInInspector] public string docfxPath;
    [HideInInspector] public string assetPath;

    // [HideInInspector] public AssetData selfAssetData;

    [HorizontalGroup("Group1")]
    [Button("Generate")]
    [ShowIf("@DocsUtility._cachedValidFolder")]
    [GUIColor("@Color.yellow")]
    public void Generate()
    {
        // if (!DocsUtility.isEditMode) DocsUtility.EditFields();

        Utility.GenerateDocFX(templateType, DocsUtility._docRelativePath + DocsUtility.DOCUMENTATION_FOLDER_TAG);
        Utility.ExtractDocFxJson(docfxPath);
        Utility.TryExtractJson<DocFxModel>(docfxPath, out DocFxModel extract);

        var newModel = new DocfxUtilityJson()
        {
            docName = DocsUtility._docName,
            docDisplayName = DocsUtility._docDisplayName,
            docrelativePath = DocsUtility._docRelativePath,
            docVersion = DocsUtility._docVersion,
            templateData = new TemplateData()
            {
                templateName = extract.templateData.templateName,
                version = extract.templateData.version
            }
        };

        Utility.TryCreateJson(newModel,
            file: DocsUtility._docRelativePath + DocsUtility.DOCUMENTATION_FOLDER_TAG +
                  DocsUtility.DOCFX_UTILITY_JSON_TAG);
        // DocsUtility._docFxModel = extract;
        DocsUtility.isEditMode = true;
        DocsUtility.Search();

        AssetDataDocsUtility assetData = AssetDataDocsUtilityFromJson(newModel);
        string path = $"{DocsUtility._docRelativePath}{DocsUtility.DOCUMENTATION_FOLDER_TAG}/DocAssetData_{DocsUtility._docName}.asset";
        AssetDatabase.CreateAsset(assetData, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private AssetDataDocsUtility AssetDataDocsUtilityFromJson(DocfxUtilityJson docfxUtilityJson)
    {
        AssetDataDocsUtility assetData = ScriptableObject.CreateInstance<AssetDataDocsUtility>();
        assetData.assetName = docfxUtilityJson.docName;
        assetData.assetDisplayName = docfxUtilityJson.docDisplayName;
        assetData.assetPath = docfxUtilityJson.docrelativePath;
        assetData.assetVersion = docfxUtilityJson.docVersion;
        assetData.templateName = docfxUtilityJson.templateData.templateName;
        // assetData.rootNamespaces  = docfxUtilityJson.;

        return assetData;
    }

    [HorizontalGroup("Group1")]
    [Button("Preview")]
    [GUIColor("@Color.yellow")]
    public void Preview()
    {
        Utility.ExecuteCommandAllStage(docfxPath);
    }

    [HorizontalGroup("Group1")]
    [Button("Clear Build")]
    [GUIColor("@Color.yellow")]
    public void ClearBuild()
    {
        Utility.DeleteDirectory(Path.Combine(Path.GetDirectoryName(Path.GetFullPath(docfxPath)), "_site~"));
    }
}

public class DocfxUtilityJson
{
    public string docName;
    public string docDisplayName;
    public string docVersion;
    public string docrelativePath;
    public TemplateData templateData;
}

[Serializable]
public class TemplateData
{
    public string templateName;
    public string version;
}