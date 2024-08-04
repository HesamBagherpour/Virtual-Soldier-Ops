using Newtonsoft.Json;

namespace ArioSoren.PackageTemplateUtility.Editor.Models.PackageManifestModel
{
    /// <summary>
    /// The model of the package manifest's repository.
    /// </summary>
    [System.Serializable]
    public class PackageManifestRepository
    {
        /// <summary>
        /// The URL of the repository.
        /// </summary>
        [JsonProperty("url")]
        public string Url;

        /// <summary>
        /// The type of the repository e.g. GitHub.
        /// </summary>
        [JsonProperty("type")]
        public string Type;

        /// <summary>
        /// ?????????????????
        /// </summary>
        [JsonProperty("revision")]
        public string Revision;
    }
}