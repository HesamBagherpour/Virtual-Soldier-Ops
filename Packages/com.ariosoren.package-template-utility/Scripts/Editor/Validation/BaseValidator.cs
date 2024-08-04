using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using LogType = ArioSoren.PackageTemplateUtility.Editor.Validation.Logger.LogType;

namespace ArioSoren.PackageTemplateUtility.Editor.Validation
{
    public abstract class BaseValidator
    {
        protected static ShowPackageWindow _showPackageWindow;
        #region CORE

        public abstract bool PackageIsValid(string packageName, ShowPackageWindow showPackageWindow);
        public abstract bool AllRequiredFilesAndFoldersExist(string packageName);
        #endregion CORE
    
    
        #region FILES_AND_FOLDERS_EXISTEMCE
        public static bool PackageDirectoryExists(string packageName)
        {
            if (!Directory.Exists(PackageDirectoryPath(packageName)))
            {
                _showPackageWindow.logWindow.AddNewLog(LogType.ERROR, "Package directory does not Exist.");
                return false;
            }
            
            _showPackageWindow.logWindow.AddNewLog(LogType.PASSED, "Package directory Exists.");
            return true;
        }
        
        public static bool PackageJsonExists(string packageName)
        {
            if (!File.Exists(Path.Combine(PackageJsonPath(packageName))))
            {
                _showPackageWindow.logWindow.AddNewLog(LogType.ERROR, "Package json file does not exist.");
                return false;
            }

            _showPackageWindow.logWindow.AddNewLog(LogType.PASSED, "Package json file exists");
            return true;
        }
        #endregion FILES_AND_FOLDERS_EXISTEMCE
        
        
        #region STRING_SETTER
        public static string PackageDirectoryPath(string packageName)
        {
            return Path.Combine(Application.dataPath, "ArioSoren", packageName);
        }
        
        public static string PackageJsonPath(string packageName)
        {
            return Path.Combine(PackageDirectoryPath(packageName), "package.json");
        }
        #endregion STRING_SETTER


        #region UTILITIES
        protected static bool ObjectIsNotNull<T>(T targetObject, string objectName)
        {
            if (targetObject == null)
            {
                _showPackageWindow.logWindow.AddNewLog(LogType.ERROR, $"{objectName} is null!");
                return false;
            }

            _showPackageWindow.logWindow.AddNewLog(LogType.PASSED, $"{objectName} is not null.");
            return true;
        }

        protected static bool StringIsNotNullOrEmpty(string value, string valueName)
        {
            if (string.IsNullOrEmpty(value))
            {
                _showPackageWindow.logWindow.AddNewLog(LogType.ERROR, $"{valueName} is null or empty!");
                return false;
            }

            _showPackageWindow.logWindow.AddNewLog(LogType.PASSED, $"{valueName} is not null or empty.");
            return true;
        }

        protected static bool StringDoesNotContainCharacter(string value, string valueName, Char targetCharacter)
        {
            if (value.Contains(targetCharacter))
            {
                _showPackageWindow.logWindow.AddNewLog(LogType.ERROR, $"{valueName} contains `{targetCharacter}` char.");
                return false;
            }

            _showPackageWindow.logWindow.AddNewLog(LogType.PASSED, $"{valueName} does not contain `{targetCharacter}` char.");
            return true;
        }

        protected static bool StringStartsWithUpperLetter(string value, string valueName)
        {
            if (!Char.IsUpper(value[0]))
            {
                _showPackageWindow.logWindow.AddNewLog(LogType.ERROR,
                    $"{valueName} does not start with an upper letter.");
                return false;
            }
                
            _showPackageWindow.logWindow.AddNewLog(LogType.PASSED, $"{valueName} starts with an upper letter.");
            return true;
        }

        protected static bool TwoStringsAreSame(string intendedString, string intendedStringName, string aimString)
        {
            if (intendedString != aimString)
            {
                _showPackageWindow.logWindow.AddNewLog(LogType.ERROR, $"{intendedStringName} is not equal to {aimString}");
                return false;
            }
            
            _showPackageWindow.logWindow.AddNewLog(LogType.PASSED, $"{intendedStringName} is equal to {aimString}");
            return true;
        }

        protected static List<T> GetAllFiles<T>(string sourceDirectory, string format, string listName, string pathMustInclude = null) where T: UnityEngine.Object
        {
            var allFiles = new List<T>();
            var allFilesPath = Directory.GetFiles(sourceDirectory, format);

            foreach (var path in allFilesPath)
            {
                if (!string.IsNullOrEmpty(pathMustInclude))
                {
                    if (!path.Contains(pathMustInclude)) continue;
                }
                
                var relativePath = PathRelatedToProject(path);
                allFiles.Add(AssetDatabase.LoadAssetAtPath<T>(relativePath));
            }
            
            var allDirectories = Directory.GetDirectories(sourceDirectory);
            
            foreach (var directory in allDirectories)
            {
                allFiles.AddRange(GetAllFiles<T>(directory, format, listName, pathMustInclude));
            }

            return allFiles;
        }

        protected static List<string> GetAllFilesPath(string sourceDirectory, string format)
        {
            var allFilesPath = Directory.GetFiles(sourceDirectory, format).ToList();
            var allDirectories = Directory.GetDirectories(sourceDirectory);
            
            foreach (var directory in allDirectories)
            {
                allFilesPath.AddRange(GetAllFilesPath(directory, format));
            }

            return allFilesPath;
        }

        protected static string ShortestStringInList(List<string> allStrings)
        {
            var shortestString = allStrings[0];
            foreach (var str in allStrings)
            {
                if (str.Length < shortestString.Length)
                {
                    shortestString = str;
                }
            }

            return shortestString;
        }

        protected static string LongestStringInList(List<string> allStrings)
        {
            var longestString = allStrings[0];
            foreach (var str in allStrings)
            {
                if (str.Length > longestString.Length)
                {
                    longestString = str;
                }
            }

            return longestString;
        }

        protected static string RemoveLastPartOfThePath(string path)
        {
            var splitedPath = path.Split("/");
            var lastPartLength = splitedPath[splitedPath.Length - 1].Length;
            return path.Remove(path.Length - lastPartLength, lastPartLength);
        }

        protected static bool DirectoryExists(string rootDirectory, string targetDirectory, bool searchJustInRoot = false)
        {
            var allDirectories = Directory.GetDirectories(rootDirectory);
            
            foreach (var directory in allDirectories)
            {
                if (directory.Contains(targetDirectory)) return true;
            }

            if (searchJustInRoot) return false;

            foreach (var directory in allDirectories)
            {
                if (DirectoryExists(directory, targetDirectory)) return true;
            }

            return false;
        }

        protected static string PathRelatedToProject(string path)
        {
            return Path.GetRelativePath(Application.dataPath.Remove(Application.dataPath.Length-6,6), path);
        }

        protected static bool StringHasNoUpperCharacter(string targetString, string stringName)
        {
            foreach (var letter in targetString)
            {
                if (Char.IsUpper(letter))
                {
                    _showPackageWindow.logWindow.AddNewLog(LogType.ERROR, $"{stringName} => {targetString} has some upper letters.");
                    return false;
                }
            }

            _showPackageWindow.logWindow.AddNewLog(LogType.PASSED, $"{stringName} => {targetString} has no upper letter.");
            return true;
        }
        #endregion UTILITIES
    }
}