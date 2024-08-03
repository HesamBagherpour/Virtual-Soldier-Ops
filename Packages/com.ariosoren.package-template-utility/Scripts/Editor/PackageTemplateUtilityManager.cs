using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using ArioSoren.PackageTemplateUtility.Editor.Models;
using ArioSoren.PackageTemplateUtility.Editor.Models.PackageManifestModel;
using Newtonsoft.Json.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ArioSoren.PackageTemplateUtility.Editor
{
    public static class PackageTemplateUtilityManager
    {
        [ReadOnly] public static string PackageNameSpace = "ArioSoren";
        [ReadOnly] public static string assemblyName = "ArioSoren";

        public static List<PackageData> GetPackages()
        {
            List<PackageData> packageDatas = new List<PackageData>();
            foreach (var packagePath in PackagesListPath())
            {
                if (!File.Exists(packagePath + "/Package.json")) continue;
                PackageData packageData = new PackageData();
                packageData.PackageManifest = new PackageManifest();
                packageData.PackageManifest = GetPackageManifestData(packagePath);
                packageData.Path = packagePath;
                packageDatas.Add(packageData);
            }

            return packageDatas;
        }

        /// <summary>
        /// return instance PackageManifest from given package path
        /// </summary>
        /// <param name="path"></param>
        /// <returns> PackageManifest instance of given package path</returns>
        public static PackageManifest GetPackageManifestData(string path)
        {
            if (File.Exists(path))
            {
                JObject o1 = JObject.Parse(File.ReadAllText(path));
                PackageManifest packageManifest = o1.ToObject<PackageManifest>();
                return packageManifest;
            }

            return null;
        }

        /// <summary>
        /// return list of all the packages paths
        /// </summary>
        /// <returns> a list containing of all the packages path </returns>
        public static List<string> PackagesListPath()
        {
            List<string> lsts = new List<string>();
            string[] files = Directory.GetFiles(Application.dataPath, "*package.json", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                lsts.Add(file);
            }

            return lsts;
        }

        /// <summary>
        /// Check package exist in the given path
        /// </summary>
        /// <param name="path"> string path </param>
        /// <returns>A bool returns true package exist , false if package path doesn't exists. </returns>
        public static bool CheckPackageExists(string path)
        {
            if (Directory.Exists(path))
            {
                return true;
            }

            return false;
        }

        public static string GetNameSpaceDirPath(string packageNameSpace, string name)
        {
            return Path.Combine("Assets/ariosoren/Packages",
                string.Join(".", packageNameSpace, name));
        }

        public static string GetPascalCase(string name, string separator)
        {
            var pascalCase = Regex.Replace(name, @"((^\w)|(\.|\p{P})\w)", match => match.Value.ToUpper())
                .Replace("-", separator);
            return pascalCase;
        }
    }
}