using System.Collections.Generic;
using Newtonsoft.Json;
using Sirenix.OdinInspector;

namespace ArioSoren.PackageTemplateUtility.Editor.Models.PackageManifestModel
{
    /// <summary>
    /// package.json data model.
    /// </summary>
    [System.Serializable]
    public class PackageManifest
    {
        /// <summary>
        /// The name of the package.
        /// </summary>
        [PreviewField(60)] [JsonProperty("name")]
        public string Name;

        /// <summary>
        /// The display name of the package. Display name shown in the `Package Manager` window.
        /// </summary>
        [JsonProperty("displayName")] 
        public string DisplayName;

        /// <summary>
        /// The version of the package.
        /// </summary>
        [JsonProperty("version")]
        public string Version;

        /// <summary>
        /// Just for external packages. It refers to the version of the original package.
        /// </summary>
        [JsonProperty("referenceVersion")] 
        public string ReferenceVersion;

        /// <summary>
        /// The type of the package. It could be "ariosoren", "external", "external-legacy".
        /// </summary>
        [JsonProperty("packageType")] 
        public string PackageType;

        /// <summary>
        /// ??????
        /// </summary>
        [JsonProperty("unity")] 
        public string Unity;

        /// <summary>
        /// The minimum version of the Unity Editor which supports the package.
        /// </summary>
        [JsonProperty("unityRelease")] 
        public string UnityRelease;

        /// <summary>
        /// The description of the package.
        /// </summary>
        [JsonProperty("description")] 
        public string Description;

        /// <summary>
        /// The host of the package e.g. `Unity Hub`.
        /// </summary>
        [JsonProperty("host")] 
        public string Host;

        /// <summary>
        /// The URL of the package's documentation.
        /// </summary>
        [JsonProperty("documentationUrl")] 
        public string DocumentationUrl;

        /// <summary>
        /// The URl of the package's change log.
        /// </summary>
        [JsonProperty("changelogUrl")] 
        public string ChangelogUrl;

        /// <summary>
        /// The license of the package.
        /// </summary>
        [JsonProperty("license")] 
        public string License;

        /// <summary>
        /// The URL of the package's license.
        /// </summary>
        [JsonProperty("licensesUrl")] 
        public string LicensesUrl;

        /// <summary>
        /// ?????
        /// </summary>
        [JsonProperty("hideInEditor")] 
        public bool HideInEditor;

        /// <summary>
        /// The type of the package e.g. `Tool`.
        /// </summary>
        [JsonProperty("type")] 
        public string Type;

        /// <summary>
        /// The dictionary of the package dependencies.
        /// Each element of this dictionary includes two strings:
        /// the key is the name of the dependency and the value is the version or repository of the dependency. 
        /// </summary>
        [JsonProperty("dependencies")] 
        public Dictionary<string, string> Dependencies;

        /// <summary>
        /// The keywords of the package.
        /// Keywords are useful for searching the package within the `Package Manager` window.
        /// </summary>
        [JsonProperty("keywords")] 
        public List<string> Keywords;

        /// <summary>
        /// The author of the package.
        /// </summary>
        [JsonProperty("author")] 
        public PackageManifestAuthor Author;

        /// <summary>
        /// The repository of the package.
        /// </summary>
        [JsonProperty("repository")] 
        public PackageManifestRepository Repository;

        /// <summary>
        /// A list of samples of the pacakage.
        /// </summary>
        [JsonProperty("samples")] 
        public List<PackageManifestSample> Samples;
    }
}