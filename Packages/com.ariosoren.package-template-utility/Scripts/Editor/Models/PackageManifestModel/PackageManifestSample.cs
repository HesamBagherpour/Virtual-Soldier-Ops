using Newtonsoft.Json;

namespace ArioSoren.PackageTemplateUtility.Editor.Models.PackageManifestModel
{
    /// <summary>
    /// The model of the package manifest's samples.
    /// </summary>
    [System.Serializable]
    public class PackageManifestSample
    {
        /// <summary>
        /// The display name of the sample.
        /// </summary>
        [JsonProperty("displayName")]
        public string DisplayName;

        /// <summary>
        /// The description of the sample.
        /// </summary>
        [JsonProperty("description")]
        public string Description;

        /// <summary>
        /// the path of the sample.
        /// </summary>
        [JsonProperty("path")]
        public string Path;
    }
}