using System.Collections.Generic;
using Newtonsoft.Json;

namespace ArioSoren.PackageTemplateUtility.Editor.Models
{
    public class PackageAssemblyDefinitionManifest
    {
        [JsonProperty("name")] public string Name;
        [JsonProperty("rootNamespace")] public string RootNamespace;
        [JsonProperty("references")] public List<string> References;
        [JsonProperty("excludePlatforms")] public List<string> ExcludePlatforms;
        [JsonProperty("allowUnsafeCode")] public bool AllowUnsafeCode;
        [JsonProperty("overrideReferences")] public bool OverrideReferences;
        [JsonProperty("precompiledReferences")]
        public List<string> PrecompiledReferences;
        [JsonProperty("autoReferenced")] public bool AutoReferenced;
        [JsonProperty("defineConstraints")] public List<string> DefineConstraints;
        [JsonProperty("versionDefines")] public List<string> VersionDefines;
        [JsonProperty("includePlatforms")] public List<string> IncludePlatforms;
        [JsonProperty("noEngineReferences")] public bool NoEngineReferences;
    }
}