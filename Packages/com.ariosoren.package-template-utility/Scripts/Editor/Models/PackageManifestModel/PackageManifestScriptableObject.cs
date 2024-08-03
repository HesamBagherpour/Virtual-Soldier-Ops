using System;
using UnityEngine;

namespace ArioSoren.PackageTemplateUtility.Editor.Models.PackageManifestModel
{
    // ToDo: I think we should move this script to somewhere else. It's not a model.
    [CreateAssetMenu(fileName = "ProjectDataScriptable", menuName = "ProjectDataScriptable/ProjectDataScriptableObject", order = 1)]
    [Serializable]
    public class PackageManifestScriptableObject : ScriptableObject
    {
        public PackageManifestData packageManifestData;

    }
}