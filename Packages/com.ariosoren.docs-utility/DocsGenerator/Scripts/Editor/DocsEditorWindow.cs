using System.Collections.Generic;
using ArioSoren.DocsGenerator.Editor.Models;
#if UNITY_EDITOR
namespace ArioSoren.DocsUtility.DocsGenerator.Editor
{
    using Sirenix.OdinInspector.Editor;
    using Sirenix.Utilities;
    using Sirenix.Utilities.Editor;
    using UnityEditor;
    using UnityEngine;
    using System.Linq;
    using System;
    using System.IO;
    using System.Text;
    using YamlDotNet.Serialization;
    using Sirenix.OdinInspector;

    public class DocsEditorWindow : OdinMenuEditorWindow
    {
        // public static string AssetPackagesFolderPath =
        //     "Packages/com.ariosoren.docs-utility/DocsGenerator/Resources/AssetDatabase/Packages";

        // public static string AssetProjectsFolderPath =
        //     "Packages/com.ariosoren.docs-utility/DocsGenerator/Resources/AssetDatabase/Projects";

        // public static Texture2D errorTexture = CreateSolidTexture2D(new Color(255, 0, 34, 30));

        private DocsUtility docsUtility;


        [MenuItem("ArioSoren/Packages/DocsGenerator/Doc List Panel",priority = 0)]
        private static void Open()
        {
            Open(string.Empty);
        }

        private static string selectedAsset;

        public static void Open(string assetName)
        {
            var window = GetWindow<DocsEditorWindow>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 500);
            selectedAsset = assetName;
            // Debug.Log("Open with param");
        }


        protected override OdinMenuTree BuildMenuTree()
        {
            // Debug.Log("BuildMenuTree");
            var tree = new OdinMenuTree(false)
                        {
                { "Projects",null},
                { "Packages",null},
            };
            tree.DefaultMenuStyle.IconSize = 28.00f;
            // tree.Config.DrawSearchToolbar = true;
            // tree.DefaultMenuStyle.DrawFoldoutTriangle = true;
            tree.Config.DefaultMenuStyle.IconSize = 20;
            // tree.Config.DefaultMenuStyle.LabelVerticalOffset = 200;


            // PackageTemplateUtilityManager.
            // GetPackages();


            tree.Selection.SelectionChanged += OnSelectionChanged;

            Texture2D checkIcon = (Texture2D)AssetDatabase.LoadAssetAtPath(
                "Packages/com.ariosoren.docs-utility/DocsGenerator/Resources/Sprites/Check.png",
                typeof(Texture2D));

            Texture2D crossIcon = (Texture2D)AssetDatabase.LoadAssetAtPath(
                "Packages/com.ariosoren.docs-utility/DocsGenerator/Resources/Sprites/CrossIcon.png",
                typeof(Texture2D));

            Texture2D warningIcon = (Texture2D)AssetDatabase.LoadAssetAtPath(
                "Packages/com.ariosoren.docs-utility/DocsGenerator/Resources/Sprites/WarningIcon.png",
                typeof(Texture2D));


            // Adds all Packages.
            tree.AddAllAssetsAtPath("Packages/", "Packages/", typeof(AssetDataDocsUtility), true, true);

            // Adds all Projects.
            tree.AddAllAssetsAtPath("Projects/", "Assets/", typeof(AssetDataDocsUtility), true, true);

            // return tree;


            tree.EnumerateTree().AddIcons<AssetDataDocsUtility>((x) =>
            {

                if (x.docValidationState.HasFlag(DocValidationState.CorruptedDocument))
                {
                    return crossIcon;
                }
                else if (x.docValidationState.HasFlag(DocValidationState.EmptyProject))
                {
                    return warningIcon;
                }
                else
                {
                    return checkIcon;
                }
            });

            if (selectedAsset != string.Empty)
            {
                if (tree.MenuItems[0].ChildMenuItems.Any(x => x.Name == selectedAsset))
                {
                    tree.MenuItems[0].ChildMenuItems.Find(x => x.Name == selectedAsset).Select(true);
                    selectedAsset = String.Empty;
                }
            }

            if (selectedAsset != string.Empty)
            {
                if (tree.MenuItems[1].ChildMenuItems.Any(x => x.Name == selectedAsset))
                {
                    tree.MenuItems[1].ChildMenuItems.Find(x => x.Name == selectedAsset).Select(true);
                    selectedAsset = String.Empty;
                }
            }

            tree.EnumerateTree().ForEach(DrawAssetDelete<AssetDataDocsUtility>);

            foreach (var item in tree.EnumerateTree())
            {
                // item.OnDrawItem += x => { GUILayout.Button("Test"); };
                item.Toggled = true;
            }

            return tree;
        }


        private void OnSelectionChanged(SelectionChangedType obj)
        {
            // Debug.Log("OnSelectionChanged");
            if (this.MenuTree != null)
            {
                AssetDataDocsUtility asset = (AssetDataDocsUtility)this.MenuTree.Selection.SelectedValue;
                if (asset != null)
                {
                    asset.OnValidate();
                    // docsUtility = GetWindow<DocsUtility>("Docs Utility");
                    // DocsUtility.Fill(asset.assetPath, asset.assetName, asset.assetVersion, asset.assetDisplayName);
                }
                // Debug.Log($"OnSelectionChanged: {asset.assetName}");
            }
        }

        protected override void OnBeginDrawEditors()
        {
            var selected = this.MenuTree.Selection.FirstOrDefault();
            var toolbarHeight = this.MenuTree.Config.SearchToolbarHeight;

            // AssetData asset = (AssetData)this.MenuTree.Selection.SelectedValue;
            // asset?.DrawDocsGeneratorVariant();

            SirenixEditorGUI.BeginHorizontalToolbar(toolbarHeight);
            {
                if (selected != null)
                {
                    GUILayout.Label(selected.Name);
                }
            }
            SirenixEditorGUI.EndHorizontalToolbar();
        }

        public static void DrawAssetDelete<T>(OdinMenuItem menuItem) where T : AssetDataDocsUtility
        {
            menuItem.OnDrawItem += x =>
            {
                if (menuItem.Value == null ||
                    (menuItem.Value as T).docValidationState != DocValidationState.Documented) return;

                var buttonRect = new Rect(new Vector2(menuItem.Rect.width - 25, menuItem.Rect.y + 5),
                    new Vector2(20, 20));

                // GUI.Button(buttonRect, "");
                // Debug.Log("Clicked1");

                if (SirenixEditorGUI.IconButton(buttonRect, EditorIcons.X))
                {
                    // DeletePopUp.OpenWindow((T)menuItem.Value, buttonRect);
                    // this.MenuTree.MenuItems.Remove((T)menuItem.Value)
                    ((T)menuItem.Value).DeleteDocumentation(true);
                }
            };
        }

        static string result = string.Empty;
        static Color textFieldColor = Color.black;

        public static void DrawEdit(OdinMenuItem menuItem, AssetDataDocsUtility docsUtility, string[] allYamlFiles)
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
                            docsUtility.OnValidate();
                        });
                    }

                if (isFile)
                {
                    // Edit file button
                    if (SirenixEditorGUI.IconButton(secondButtonRect, EditorIcons.Info))
                    {
                        string value =
                            Utility.LoadFile(docsUtility.assetPath + "/Documentation/" + menuItem.GetFullPath());
                        var window = OdinEditorWindow.InspectObject(new ReadMd(value,
                            docsUtility.assetPath + "/Documentation/" + menuItem.GetFullPath()));
                        window.position = GUIHelper.GetEditorWindowRect().AlignCenter(1000, 500);
                        window.Show(true);
                    }

                    if (isBand) return;

                    if (CanMoveFile(allYamlFiles, menuItem, up: true))
                    {
                        if (SirenixEditorGUI.IconButton(thirdButtonRect, EditorIcons.ArrowUp))
                        {
                            MoveFileElement(allYamlFiles, menuItem, up: true);
                            docsUtility.OnValidate();
                        }
                    }

                    if (CanMoveFile(allYamlFiles, menuItem, up: false))
                    {
                        if (SirenixEditorGUI.IconButton(fourthButtonRect, EditorIcons.ArrowDown))
                        {
                            MoveFileElement(allYamlFiles, menuItem, up: false);
                            docsUtility.OnValidate();
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
                            AddFile(docsUtility, menuItem, result);
                            docsUtility.OnValidate();
                            menuItem.MenuTree.UpdateMenuTree();
                            result = String.Empty;
                        }

                        if (SirenixEditorGUI.IconButton(thirdButtonRect, EditorIcons.Folder))
                        {
                            AddFolder(docsUtility, menuItem, result);
                            docsUtility.OnValidate();
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
                            docsUtility.OnValidate();
                        }
                    }

                    if (CanMoveFolder(allYamlFiles, menuItem, up: false))
                    {
                        if (SirenixEditorGUI.IconButton(fourthButtonRect, EditorIcons.ArrowDown))
                        {
                            MoveFolderElement(allYamlFiles, menuItem, up: false);
                            docsUtility.OnValidate();
                        }
                    }
                }
            };
        }

        private static void AddFolder(AssetDataDocsUtility assetData, OdinMenuItem menuItem, string fileName)
        {
            var assetDataAssetPath = assetData.assetPath + "/Documentation/" +
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

            Directory.CreateDirectory(assetData.assetPath + "/Documentation/" +
                                      menuItem.GetFullPath() + "/" + fileName);
            using (FileStream fs = File.Create(assetData.assetPath + "/Documentation/" +
                                               menuItem.GetFullPath() + "/" + fileName + "/" + "toc.yml"))
            {
            }

            var stringBuilder = new StringBuilder();
            var stringWriter = new StringWriter(stringBuilder);

            var serializer = new Serializer();
            serializer.Serialize(stringWriter, data.content);

            File.WriteAllText(assetDataAssetPath, stringBuilder.ToString());
        }

        private static void AddFile(AssetDataDocsUtility assetData, OdinMenuItem menuItem, string fileName)
        {
            var assetDataAssetPath = assetData.assetPath + "/Documentation/" +
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

            using (FileStream fs = File.Create(assetData.assetPath + "/Documentation/" +
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
                    string pathToRemove = Utility.RemoveLastDirSlash(allYamlFiles[index]) + "/" +
                                          data.content[indexToRemove].href;
                    AssetDatabase.DeleteAsset(pathToRemove);
                    data.content.RemoveAt(indexToRemove);

                    var stringBuilder = new StringBuilder();
                    var stringWriter = new StringWriter(stringBuilder);

                    var serializer = new Serializer();
                    serializer.Serialize(stringWriter, data.content);

                    File.WriteAllText(allYamlFiles[indexToRemove], stringBuilder.ToString());
                }
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
    }

    class ReadMd
    {
        [HideLabel]
        [EnableIf("IsEditMode")]
        [TextArea(20, 34)]
        public string text;

        [HideInInspector] public string fullPath;
        [HideInInspector] public bool IsEditMode;

        public ReadMd(string value, string path)
        {
            this.text = value;
            this.fullPath = path;
        }

        [ShowIf("IsEditMode")]
        [HorizontalGroup("Group1")]
        [Button(), PropertyOrder(-20)]
        public void Save()
        {
            File.WriteAllText(fullPath, text);
            IsEditMode = false;
        }

        [Button()]
        [HorizontalGroup("Group1")]
        public void Open()
        {
            UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(fullPath, 0);
        }

        [Button()]
        [HorizontalGroup("Group1")]
        [HideIf("IsEditMode")]
        public void Edit()
        {
            IsEditMode = !IsEditMode;
        }
    }

#endif
}