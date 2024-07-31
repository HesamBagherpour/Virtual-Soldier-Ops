using Newtonsoft.Json;

namespace ArioSoren.PackageTemplateUtility.Editor.Models.PackageManifestModel
{
    /// <summary>
    /// The model of the package manifest's author.
    /// </summary>
    [System.Serializable]
    public class PackageManifestAuthor
    {
        /// <summary>
        /// The name of the author.
        /// </summary>
        [JsonProperty("name")]
        public string Name;

        /// <summary>
        /// The email of the author.
        /// </summary>
        [JsonProperty("email")]
        public string Email;

        /// <summary>
        /// The URL of the author.
        /// </summary>
        [JsonProperty("url")]
        public string Url;
    }
}