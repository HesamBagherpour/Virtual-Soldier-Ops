using System;
using ArioSoren.PackageTemplateUtility.Editor.Models.PackageManifestModel;
using Newtonsoft.Json;

namespace ArioSoren.PackageTemplateUtility.Editor.Models
{
    /// <summary>
    /// A model which represents package data. 
    /// </summary>
    [Serializable]
    public class PackageData
    {
        /// <summary>
        /// The manifest of the package.
        /// It's an instance of a model which represents `package.json` file of the package.  
        /// </summary>
        [JsonProperty("packageManifest")] public PackageManifest PackageManifest;
        
        /// <summary>
        /// The path of the package in the project. 
        /// </summary>
        [JsonProperty("path")] public string Path;
    }
}
