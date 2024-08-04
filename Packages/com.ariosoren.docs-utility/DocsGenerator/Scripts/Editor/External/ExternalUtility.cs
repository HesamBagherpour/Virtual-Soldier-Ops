using System;
using System.Collections.Generic;
using ArioSoren.DocsGenerator.Editor.Models;

namespace ArioSoren.DocsGenerator.Editor.External
{
    public class ExternalUtility 
    {
        public static DocumentationInfo GetDocumentationInfo(string packageName)
        {
            List<AssetDataDocsUtility> assets = AssetDataDocsUtility.LoadAllAssets();
    
            foreach (var data in assets)
            {
                if (data.assetName == packageName && data.docValidationState == DocValidationState.Documented)
                {
                    // if (openEditor)
                    // {
                    //     DocsEditorWindow.Open(packageName);
                    // }
    
                    return new DocumentationInfo()
                    {
                        path = data.assetPath + "/Documentation/",
                        isDocumented = true
                    };
                }
            }
    
            return new DocumentationInfo()
            {
                path = String.Empty,
                isDocumented = false
            };
        }
    }
}


