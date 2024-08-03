using System.IO;
using UnityEngine;

namespace ArioSoren.PackageTemplateUtility.Editor.Validation
{
    public class LegacyValidator : BaseValidator
    {
        private static LegacyValidator _instance;
        
        #region CORE
        public static LegacyValidator Instance()
        {
            return _instance ??= new LegacyValidator();
        }

        public override bool PackageIsValid(string packageName, ShowPackageWindow showPackageWindow)
        {
            return AllRequiredFilesAndFoldersExist(packageName);
        }

        public override bool AllRequiredFilesAndFoldersExist(string packageName)
        {
            return PackageDirectoryExists(packageName) && (PackageJsonExists(packageName) 
                    && (ThereIsInstallerJsonFile(packageName) && ThereIsUnityPackageFile(packageName)));
        }
        #endregion CORE

        
        #region FILES_AND_FOLDERS_EXISTENCE
        public bool ThereIsInstallerJsonFile(string packageName)
        {
            return File.Exists(Path.Combine(PackageDirectoryPath(packageName), "ArioSoren", packageName,
                "install-remove-package.json"));
        }

        public bool ThereIsUnityPackageFile(string packageName)
        {
            var unityPackageFiles = Directory.GetFiles("*.unitypackage");
            return unityPackageFiles.Length != 0;
        }
        #endregion FILES_AND_FOLDERS_EXISTENCE
    }
}