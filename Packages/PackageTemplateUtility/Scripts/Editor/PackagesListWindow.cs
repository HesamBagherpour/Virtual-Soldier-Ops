using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace ArioSoren.PackageTemplateUtility.Editor
{
    public class PackagesListWindow : OdinMenuEditorWindow
    {
        [MenuItem("ArioSoren/Package Template Utility Editor")]
        private static void Open()
        {
            var window = GetWindow<PackagesListWindow>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 500);
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            OdinMenuTree tree = new OdinMenuTree(supportsMultiSelect: true);
            tree.DefaultMenuStyle.IconSize = 28.00f;
            tree.Config.DrawSearchToolbar = true;
            tree.DefaultMenuStyle.DrawFoldoutTriangle = true;
            tree.Config.DefaultMenuStyle.IconSize = 20;
            tree.Add("Create new", new MainPage());
            
            Texture2D checkedImage = (Texture2D) AssetDatabase.LoadAssetAtPath(
                "Packages/ae.ariosoren.package-template-utility/PackageTemplateUtility/Art/Icons/checked.png",
                typeof(Texture2D));
            Texture2D updateImage = (Texture2D) AssetDatabase.LoadAssetAtPath(
                "Packages/ae.ariosoren.package-template-utility/PackageTemplateUtility/Art/Icons/update.png",
                typeof(Texture2D));
            Texture2D packageImage = (Texture2D) AssetDatabase.LoadAssetAtPath(
                "Packages/ae.ariosoren.package-template-utility/PackageTemplateUtility/Art/Icons/package.png",
                typeof(Texture2D));
            Texture2D addPackageImage = (Texture2D) AssetDatabase.LoadAssetAtPath(
                "Packages/ae.ariosoren.package-template-utility/PackageTemplateUtility/Art/Icons/add-package.png",
                typeof(Texture2D));

            AddPackagesToMenuTree(tree, PackageTemplateUtilityManager.PackagesListPath());
            tree.EnumerateTree().AddIcons<ShowPackageWindow>((x) => packageImage);
            return tree;
        }

        private void AddPackagesToMenuTree(OdinMenuTree tree, List<string> packagesPath)
        {
            foreach (var packagePath in packagesPath)
            {
                ShowPackageWindow showPackageWindow = CreateInstance<ShowPackageWindow>();
                showPackageWindow.Init(packagePath);
                tree.Add(showPackageWindow.packageManifestDataForEditor.author.Name.Trim() + "/" + showPackageWindow.packageManifestDataForEditor.displayName.Trim(), showPackageWindow);
            }
        }
    }
}