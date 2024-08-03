using System.Collections.Generic;
using System.IO;
using System.Linq;
using ArioSoren.PackageTemplateUtility.Editor.Models.PackageManifestModel;
using ArioSoren.PackageTemplateUtility.Editor.Validation.Models;
using UnityEditorInternal;
using UnityEngine;
using LogType = ArioSoren.PackageTemplateUtility.Editor.Validation.Logger.LogType;
using PackageManifest = ArioSoren.PackageTemplateUtility.Editor.Models.PackageManifestModel.PackageManifest;

namespace ArioSoren.PackageTemplateUtility.Editor.Validation
{
    public abstract class StandardValidator : BaseValidator
    {
        protected static List<AssemblyDefinitionAsset> _allAssemblyDefinitionAssets;
        
        #region CORE
        public override bool PackageIsValid(string packageName, ShowPackageWindow showPackageWindow)
        {
            _showPackageWindow = showPackageWindow;
        
            if (!AllRequiredFilesAndFoldersExist(packageName))
            {
                _showPackageWindow.logWindow.AddNewLog(LogType.ERROR, "Some required files and folders do not exist.");
                return false;
            }
        
            _showPackageWindow.logWindow.AddNewLog(LogType.PASSED,"All required files and folders exist.");

            if (!AllAssemblyDefinitionsAreValid(packageName)) return false;

            if (!PackageJsonIsValid(packageName)) return false;

            return true;
        }

        public override bool AllRequiredFilesAndFoldersExist(string packageName)
        {
            var scriptsFolder = Path.Combine(Application.dataPath, "ArioSoren", packageName, "Scripts");
            
            if (!PackageDirectoryExists(packageName)) return false;
            if (!PackageJsonExists(packageName)) return false;
            if (DirectoryExists(PackageDirectoryPath(packageName), "Scripts", true))
            {
                return ScriptsRequirementsAreValid(scriptsFolder);
            }
            
            return !ScriptsRequirementsAreValid(scriptsFolder);
        }
        
        public static bool AllAssemblyDefinitionsAreValid(string packageName)
        {
            var packageDirectory = PackageDirectoryPath(packageName);

            if (AssemblyDefinitionsAreGenerallyValid(_allAssemblyDefinitionAssets)
            && EditorAssemblyDefinitionsAreValid(packageDirectory))
            {
                _showPackageWindow.logWindow.AddNewLog(LogType.PASSED, "All assembly definitions are valid.");
                return true;
            }
            
            _showPackageWindow.logWindow.AddNewLog(LogType.ERROR, "Something went wrong in assembly definitions.");
            return false;
        }
        
        public bool PackageJsonIsValid(string packageName)
        {
            var packageJsonPath = PackageJsonPath(packageName);
            var packageJsonString = LoadPackageJson(packageJsonPath);
            var deserializedPackageJson = DeserializePackageJson(packageJsonString);

            if (AllRequiredFieldsAreSet(deserializedPackageJson) && PackageFullNameIsValid(deserializedPackageJson.Name))
            {
                _showPackageWindow.logWindow.AddNewLog(LogType.PASSED, "PackageJson file is valid.");
                return true;
            }
            
            _showPackageWindow.logWindow.AddNewLog(LogType.ERROR, "PackageJson file is not valid.");
            return false;
        }
        #endregion CORE
        
        
        #region FILES_AND_FOLDERS_EXISTENCE
        public static bool ScriptsRequirementsAreValid(string scriptsFolder)
        {
            _allAssemblyDefinitionAssets =
                GetAllFiles<AssemblyDefinitionAsset>(scriptsFolder, "*.asmdef", "All assembly definitions");

            if (_allAssemblyDefinitionAssets.Count != 0
                && ThereAreCsFiles(scriptsFolder)
                && AllCSharpFilesAreIncludedWithinTheAssemblyDefinition(scriptsFolder)
                && (DirectoryExists(scriptsFolder, "Editor", false)
                    || DirectoryExists(scriptsFolder, "Runtime", false)))
            {
                _showPackageWindow.logWindow.AddNewLog(LogType.PASSED, "All `Scripts` requirements are valid.");
                return true;
            }
            
            _showPackageWindow.logWindow.AddNewLog(LogType.ERROR, "Something went wrong in `Scripts` requirements.");
            return false;
        }

        public static bool AllCSharpFilesAreIncludedWithinTheAssemblyDefinition(string scriptsFolder)
        {
            var allCSharpFilesPath = GetAllFilesPath(scriptsFolder, "*.cs");
            var allAssemblyDefinitionsPath = GetAllFilesPath(scriptsFolder, "*asmdef");

            var shortestAssemblyDefinitionPath = ShortestStringInList(allAssemblyDefinitionsPath);
            shortestAssemblyDefinitionPath = RemoveLastPartOfThePath(shortestAssemblyDefinitionPath);

            var shortestCSharpFilePath = ShortestStringInList(allCSharpFilesPath);
            shortestCSharpFilePath = RemoveLastPartOfThePath(shortestCSharpFilePath);

            if (shortestCSharpFilePath.Length > shortestAssemblyDefinitionPath.Length)
            {
                _showPackageWindow.logWindow.AddNewLog(LogType.ERROR, "There are some C# scripts that are not included in any assembly definition.");
                return false;
            }

            _showPackageWindow.logWindow.AddNewLog(LogType.PASSED, "All C# scripts are included in assembly definitions.");
            return true;
        }
        
        public static bool ThereAreCsFiles(string sourceDirectory)
        {
            var allCsFiles = Directory.GetFiles(sourceDirectory, "*.cs");

            if (allCsFiles.Length != 0)
            {
                _showPackageWindow.logWindow.AddNewLog(LogType.PASSED, "There are some c# scripts.");
                return true;
            }
            
            var allDirectories = Directory.GetDirectories(sourceDirectory);

            foreach (var directory in allDirectories)
            {
                if (ThereAreCsFiles(directory)) return true;
            }

            _showPackageWindow.logWindow.AddNewLog(LogType.ERROR, "There is no c# file.");
            return false;
        }
        #endregion FILES_AND_FOLDERS_EXISTENCE

        
        #region ASSEMBLY_DEFINITION
        public static bool AssemblyDefinitionsAreGenerallyValid(
            List<AssemblyDefinitionAsset> allAssemblyDefinitionAssets, bool areEditorAssemblies = false)
        {
            var allDeserializedAssemblyDefinitions = DeserializeAssemblyDefinitionAssets(allAssemblyDefinitionAssets);
            
            foreach (var deserializedAssemblyDefinition in allDeserializedAssemblyDefinitions)
            {
                if (!AssemblyDefinitionIsValid(deserializedAssemblyDefinition)) return false;
                if (!areEditorAssemblies) continue;
                if (!JustEditorHasCheckedInPlatforms(deserializedAssemblyDefinition)) return false;
            }

            _showPackageWindow.logWindow.AddNewLog(LogType.PASSED, "All the assembly definitions are generally valid");
            return true;
        }

        public static bool EditorAssemblyDefinitionsAreValid(string packagePath)
        {
            var allEditorAssemblyDefinitionAssets = 
                GetAllFiles<AssemblyDefinitionAsset>(packagePath, "*.asmdef", "All assembly definitions", "/Editor");
            return AssemblyDefinitionsAreGenerallyValid(allEditorAssemblyDefinitionAssets, true);
        }
        
        public static List<AssemblyDefinition> DeserializeAssemblyDefinitionAssets(
            List<AssemblyDefinitionAsset> assemblyDefinitionAssets)
        {
            return assemblyDefinitionAssets.Select(assemblyDefinitionAsset => 
                DeserializeAssemblyDefinitionAsset(assemblyDefinitionAsset)).ToList();
        }
        
        public static AssemblyDefinition DeserializeAssemblyDefinitionAsset(
            AssemblyDefinitionAsset assemblyDefinitionAsset)
        {
            return Newtonsoft.Json.JsonConvert
                .DeserializeObject<AssemblyDefinition>(assemblyDefinitionAsset.ToString());
        }
        
        public static bool AssemblyDefinitionIsValid(AssemblyDefinition deserializedAssemblyDefinition)
        {
            if (AutoReferencedHasNotChecked(deserializedAssemblyDefinition)
                && TwoStringsAreSame(deserializedAssemblyDefinition.RootNamespace, "AssemblyDefinition-RootNamespace",deserializedAssemblyDefinition.Nmae)
                && NameOfAssemblyDefinitionIsValid(deserializedAssemblyDefinition.Nmae))
            {
                _showPackageWindow.logWindow.AddNewLog(LogType.PASSED,
                    $"The name of {deserializedAssemblyDefinition} assembly definition is Valid.");
                return true;
            }
            
            _showPackageWindow.logWindow.AddNewLog(LogType.ERROR,
                $"The name of {deserializedAssemblyDefinition} assembly definition is not Valid.");
            return false;
        }
        
        public static bool NameOfAssemblyDefinitionIsValid(string nameOfAssemblyDefinition)
        {
            if (StringIsNotNullOrEmpty(nameOfAssemblyDefinition, "AssemblyDefinition-Name")
                && StringDoesNotContainCharacter(nameOfAssemblyDefinition, "AssemblyDefinition-Name", '_')
                && StringDoesNotContainCharacter(nameOfAssemblyDefinition, "AssemblyDefinition-Name", '-')
                && AllNamePartsAreValid(nameOfAssemblyDefinition))
            {
                _showPackageWindow.logWindow.AddNewLog(LogType.PASSED,
                    $"The name of assembly definition ({nameOfAssemblyDefinition}) is Valid.");
                return true;
            }
            
            _showPackageWindow.logWindow.AddNewLog(LogType.ERROR,
                $"The name of assembly definition ({nameOfAssemblyDefinition}) is not Valid.");
            return false;
        }
        
        public static bool JustEditorHasCheckedInPlatforms(AssemblyDefinition deserializedAssemblyDefinition)
        {
            var platforms = deserializedAssemblyDefinition.IncludePlatforms;

            if (platforms.Count == 1 && platforms[0] == "Editor")
            {
                _showPackageWindow.logWindow.AddNewLog(LogType.PASSED,
                    $"Platforms are valid in {deserializedAssemblyDefinition.Nmae} assembly definition.");
                return true;
            }

            _showPackageWindow.logWindow.AddNewLog(LogType.PASSED,
                $"{deserializedAssemblyDefinition.Nmae} is an Editor assembly definition but the editor is not included in platforms or there are some platforms other than editor.");
            return false;
        }

        protected static bool AutoReferencedHasNotChecked(AssemblyDefinition deserializedAssemblyDefinition)
        {
            if (deserializedAssemblyDefinition.References.Count == 0)
            {
                return true;
            }
            
            if (deserializedAssemblyDefinition.AutoReferenced)
            {
                _showPackageWindow.logWindow.AddNewLog(LogType.ERROR, 
                    $"`Auto referenced` has checked in {deserializedAssemblyDefinition.Nmae} assembly definition.");
                return false;
            }

            _showPackageWindow.logWindow.AddNewLog(LogType.PASSED, 
                $"`Auto referenced` has not checked in {deserializedAssemblyDefinition.Nmae} assembly definition.");
            return true;
        }

        protected static bool AllNamePartsAreValid(string name)
        {
            var splitedName = name.Split(".");

            if (TwoStringsAreSame(splitedName[0],"AssemblyDefinition-Name-FirstPart", "ArioSoren")
                && AllNamePartsStartWithUpperLetters(splitedName))
            {
                _showPackageWindow.logWindow.AddNewLog(LogType.PASSED, $"The name of assembly definition ({name}) is valid.");
                return true;
            }
            
            _showPackageWindow.logWindow.AddNewLog(LogType.ERROR, $"`Something is wrong in {name}.");
            return true;
        }

        protected static bool AllNamePartsStartWithUpperLetters(string[] nameParts)
        {
            foreach (var namePart in nameParts)
            {
                if (!StringIsNotNullOrEmpty(namePart, "AssemblyDefinition-Name-SomePart")) return false;
                if (!StringStartsWithUpperLetter(namePart, $"AssemblyDefinition-Name-{namePart}")) return false;
            }

            _showPackageWindow.logWindow.AddNewLog(LogType.PASSED, "All name parts are valid.");
            return true;
        }
        #endregion ASSEMBLY_DEFINITION


        #region PACKAGE_JSON
        public static string LoadPackageJson(string packageJsonPath)
        { 
            return File.ReadAllText(packageJsonPath);
        }
        
        public static PackageManifest DeserializePackageJson(string packageJsonString)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<PackageManifest>(packageJsonString);
        }
        
        public static bool PackageFullNameIsValid(string packageFullName)
        {
            var splitedFullName = packageFullName.Split(".");
            if (TwoStringsAreSame(splitedFullName[0], "PackageJson-Name-FirstPart (Company domain)", "ae")
                && TwoStringsAreSame(splitedFullName[1], "PackageJson-Name-SecondPart (Company name)", "ariosoren")
                && StringDoesNotContainCharacter(splitedFullName[2], "PackageJson-Name-ThirdPart (package name)", '_')
                && StringHasNoUpperCharacter(splitedFullName[2], "PackageJson-Name-ThirdPart (package name)"))
            {
                _showPackageWindow.logWindow.AddNewLog(LogType.PASSED, "PackageJson-Name is valid.");
                return true;
            }
            
            _showPackageWindow.logWindow.AddNewLog(LogType.ERROR, "PackageJson-Name is not valid.");
            return false;
        }

        public virtual bool AllRequiredFieldsAreSet(PackageManifest deserializedPackageJson)
        {
            if (AuthorIsSet(deserializedPackageJson.Author)
                && StringIsNotNullOrEmpty(deserializedPackageJson.Description, "PackageJson-Description")
                && StringIsNotNullOrEmpty(deserializedPackageJson.Name, "PackageJson-Name")
                && RepositoryIsSet(deserializedPackageJson.Repository)
                && RepositoryTypeIsValid(deserializedPackageJson.Repository)
                && StringIsNotNullOrEmpty(deserializedPackageJson.Version, "PackageJson-Version")
                && StringIsNotNullOrEmpty(deserializedPackageJson.DisplayName, "PackageJson-DisplayName")
                && StringIsNotNullOrEmpty(deserializedPackageJson.PackageType, "PackageJson-PackageType"))
            {
                _showPackageWindow.logWindow.AddNewLog(LogType.PASSED, "All required fields are set.");
                return true;
            }
            
            _showPackageWindow.logWindow.AddNewLog(LogType.ERROR, "PackageJson-Some required fields are not set.");
            return false;
        }

        private static bool AuthorIsSet(PackageManifestAuthor author)
        {
            if (ObjectIsNotNull(author, "PackageJson-Author")
                && StringIsNotNullOrEmpty(author.Name, "PackageJson-Author-Name"))
            {
                _showPackageWindow.logWindow.AddNewLog(LogType.PASSED, "PackageJson-Author is set.");
                return true;
            }
            
            _showPackageWindow.logWindow.AddNewLog(LogType.ERROR, "PackageJson-Something is wrong in Author.");
            return false;
        }

        private static bool RepositoryIsSet(PackageManifestRepository repository)
        {
            if (ObjectIsNotNull(repository, "PackageJson-Repository")
                && StringIsNotNullOrEmpty(repository.Type, "PackageJson-Repository-Type")
                && StringIsNotNullOrEmpty(repository.Url, "PackageJson-Repository-URL"))
            {
                _showPackageWindow.logWindow.AddNewLog(LogType.PASSED, "PackageJson-Repository is set.");
                return true;
            }
            
            _showPackageWindow.logWindow.AddNewLog(LogType.ERROR, "PackageJson-Something is wrong in Repository.");
            return false;
        }

        public static bool RepositoryTypeIsValid(PackageManifestRepository packageManifestRepository)
        {
            if (packageManifestRepository.Type is "github" or "gitlab" or "assetstore")
            {
                _showPackageWindow.logWindow.AddNewLog(LogType.PASSED, "PackageJson-Repository-Type is valid.");
                return true;
            }
            
            _showPackageWindow.logWindow.AddNewLog(LogType.ERROR, "PackageJson-Repository-Type is not valid.");
            return false;
        }
        #endregion PACKAGE_JSON
    }
}