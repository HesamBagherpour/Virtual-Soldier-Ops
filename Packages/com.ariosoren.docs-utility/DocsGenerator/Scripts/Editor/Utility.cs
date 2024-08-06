using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using Unity.Plastic.Newtonsoft.Json;
using System.Threading.Tasks;
using System.Text;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using System;

namespace ArioSoren.DocsUtility.DocsGenerator.Editor
{
    public class Utility
    {
        public async static void ExecuteCommand(string docfxPath)
        {
            int exitCode;
            ProcessStartInfo processInfo;
            Process process;

            // UnityEngine.Debug.unityLogger.Log(Application.dataPath);

            string fxToolPath = "Packages/com.ariosoren.docs-utility/DocsGenerator/ExternalTools~/docfx.console/tools/docfx.exe";

            UnityEngine.Debug.unityLogger.Log(fxToolPath);

            string commend =
                $"{Path.GetFullPath(fxToolPath)} {Path.GetFullPath(docfxPath)} --serve";

            // UnityEngine.Debug.unityLogger.Log(commend);


            // processInfo = new ProcessStartInfo("powershell.exe" + commend);
            // // processInfo = new ProcessStartInfo("cmd.exe", commend);
            // processInfo.CreateNoWindow = false;
            // processInfo.WindowStyle = ProcessWindowStyle.Normal;
            // processInfo.UseShellExecute = true;
            // // *** Redirect the output ***
            // processInfo.RedirectStandardError = true;
            // processInfo.RedirectStandardOutput = true;

            process = Process.Start("powershell.exe", commend);
            // await Task.Delay(3000);
            Application.OpenURL("http://localhost:8080/");
            process.WaitForExit();

            exitCode = process.ExitCode;

            process.Close();
            AssetDatabase.Refresh();
        }

        public async static void ExecuteCommandServe(string docfxPath)
        {
            int exitCode;
            ProcessStartInfo processInfo;
            Process process;

            // UnityEngine.Debug.unityLogger.Log(Application.dataPath);

            string fxToolPath = "Packages/com.ariosoren.docs-utility/DocsGenerator/ExternalTools~/docfx.console/tools/docfx.exe";

            // UnityEngine.Debug.unityLogger.Log(fxToolPath);

            string commend =
                $"{Path.GetFullPath(fxToolPath)} serve {Path.GetFullPath(docfxPath)}";

            // UnityEngine.Debug.unityLogger.Log(commend);


            process = Process.Start("powershell.exe", commend);
            await Task.Delay(3000);
            Application.OpenURL("http://localhost:8080/");
            process.WaitForExit();

            exitCode = process.ExitCode;

            process.Close();
            AssetDatabase.Refresh();
        }

        public async static Task ExecuteCommandBuild(string docfxPath)
        {
            int exitCode;
            ProcessStartInfo processInfo;
            Process process;

            // UnityEngine.Debug.unityLogger.Log(Application.dataPath);

            string fxToolPath = "Packages/com.ariosoren.docs-utility/DocsGenerator/ExternalTools~/docfx.console/tools/docfx.exe";

            // UnityEngine.Debug.unityLogger.Log(fxToolPath);

            string commend =
                $"{Path.GetFullPath(fxToolPath)} build {Path.GetFullPath(docfxPath)}";

            process = Process.Start("powershell.exe", commend);
            // await Task.Delay(3000);
            // Application.OpenURL("http://localhost:8080/");
            process.WaitForExit();

            exitCode = process.ExitCode;

            process.Close();
            AssetDatabase.Refresh();
        }

        public async static void ExecuteCommandBuildAndServe(string docfxPath)
        {
            int exitCode;
            ProcessStartInfo processInfo;
            Process process;

            // UnityEngine.Debug.unityLogger.Log(Application.dataPath);

            string fxToolPath = "Packages/com.ariosoren.docs-utility/DocsGenerator/ExternalTools~/docfx.console/tools/docfx.exe";

            // UnityEngine.Debug.unityLogger.Log(fxToolPath);

            string commend =
                $"{Path.GetFullPath(fxToolPath)} build {Path.GetFullPath(docfxPath)} --serve";

            process = Process.Start("powershell.exe", commend);
            // await Task.Delay(3000);
            Application.OpenURL("http://localhost:8080/");
            process.WaitForExit();

            exitCode = process.ExitCode;

            process.Close();
            AssetDatabase.Refresh();
        }

        public async static Task ExecuteCommandMetadata(string docfxPath)
        {
            int exitCode;
            ProcessStartInfo processInfo;
            Process process;

            // UnityEngine.Debug.unityLogger.Log(Application.dataPath);

            string fxToolPath = "Packages/com.ariosoren.docs-utility/DocsGenerator/ExternalTools~/docfx.console/tools/docfx.exe";

            // UnityEngine.Debug.unityLogger.Log(fxToolPath);

            string commend =
                $"{Path.GetFullPath(fxToolPath)} metadata {Path.GetFullPath(docfxPath)}";

            process = Process.Start("powershell.exe", commend);
            process.WaitForExit();

            exitCode = process.ExitCode;

            process.Close();
            AssetDatabase.Refresh();
        }

        public async static void ExecuteCommandAllStage(string docfxPath)
        {
            string siteFolder = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(docfxPath)), "_site~");

            if (!Directory.Exists(siteFolder))
            {
                await ExecuteCommandMetadata(docfxPath);
                await ExecuteCommandBuild(docfxPath);
            }

            int port = HttpServerManager.StartServer(Path.Combine(Path.GetDirectoryName(Path.GetFullPath(docfxPath)), "_site~"));
            Application.OpenURL($"http://localhost:{port}/");
        }

        public static void DeleteDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                try
                {
                    foreach (string file in Directory.GetFiles(path))
                    {
                        File.Delete(file);
                    }

                    foreach (string subDir in Directory.GetDirectories(path))
                    {
                        DeleteDirectory(subDir);
                    }

                    Directory.Delete(path, true);
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.unityLogger.Log($"Error occurred while deleting directory '{path}': {ex.Message}");
                }
            }
            else
            {
                UnityEngine.Debug.unityLogger.Log($"Directory '{path}' does not exist.");
            }
        }


        public static string[] GetAllAssetPaths(string rootYMLPath)
        {
            string yml = LoadFile(rootYMLPath);
            TOC newTOC = Deserializer(yml);

            string relativePath = RemoveLastDirSlash(rootYMLPath);
            // var absolutePath = new DirectoryInfo(relativePath).Root;
            return newTOC.Extract(relativePath);
        }

        public static string RemoveLastDirSlash(string rootYMLPath)
        {
            int lastSlash = rootYMLPath.LastIndexOf('/');
            var relativePath = (lastSlash > -1) ? rootYMLPath.Substring(0, lastSlash) : rootYMLPath;
            return relativePath;
        }

        public static DocFxModel ExtractDocFxJson(string jsonPath)
        {
            var jsonString = File.ReadAllText(jsonPath, Encoding.UTF8);
            // DocFxModel docFxModel = JsonConvert.DeserializeObject<DocFxModel>(jsonString);
            DocFxModel docFxModel = JsonUtility.FromJson<DocFxModel>(jsonString);
            return docFxModel;
        }

        public static TOC Deserializer(string yml)
        {
            var toc = new TOC();

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance) // see height_in_inches in sample yml 
                .Build();

            toc.content = deserializer.Deserialize<List<TOCContent>>(yml);

            return toc;
        }

        public static string LoadFile(string path)
        {
            if (!File.Exists(path))
            {
                return string.Empty;
            }

            using (StreamReader reader = new StreamReader(path))
            {
                return reader.ReadToEnd();
            }
        }


        public static bool IsFile(string path)
        {
            if (File.Exists(path))
            {
                return true;
            }

            if (Directory.Exists(path))
            {
                return false;
            }

            return false;
        }

        public static bool IsYaml(string path)
        {
            string ext = Path.GetExtension(path);

            if (ext == ".yml") return true;

            return false;
        }

        public static string RemoveLastFileExtension(string value)
        {
            int lastDot = value.LastIndexOf('.');
            return (lastDot > -1) ? value.Substring(0, lastDot) : value;
        }

        public static bool TryExtractJson<T>(string jsonPath, out T toExtract)
        {
            if (!File.Exists(jsonPath))
            {
                toExtract = default;
                return false;
            }

            toExtract = JsonConvert.DeserializeObject<T>(File.ReadAllText(jsonPath));
            return toExtract != null;
        }

        public static void GenerateDocFX(string templateType, string dest)
        {
            string mSource =
                "Packages/com.ariosoren.docs-utility/Templates/" +
                templateType;

            FileUtil.CopyFileOrDirectory(mSource, dest);
            AssetDatabase.Refresh();
        }

        public static void TryCreateJson(DocfxUtilityJson model, string file)
        {
            File.WriteAllText(file, JsonConvert.SerializeObject(model));
        }
    }

    [System.Serializable]
    public class DocFxModel
    {
        public Metadata[] metadata;
        public Build build;
        public TemplateData templateData;
    }

    [System.Serializable]
    public class Metadata
    {
        public Src[] src;
        public string filter;
        public string dest;
    }

    [System.Serializable]
    public class Build
    {
        public Content[] content;
        public Resource[] resource;
        public string[] xrefService;
        public string[] postProcessors;
        public GlobalMetadata globalMetadata;
        public string dest;
        public string markdownEngineName;
        public string[] template;
    }

    [System.Serializable]
    public class Content
    {
        public string[] files;
        public string src;
        public string dest;
    }

    [System.Serializable]
    public class Resource
    {
        public string[] files;
        public string[] exclude;
    }

    [System.Serializable]
    public class GlobalMetadata
    {
        public string _appTitle;
        public string _appFooter;
        public bool _enableSearch;
    }

    [System.Serializable]
    public class Src
    {
        public string[] files;
        public string[] exclude;
        public string src;
    }

    public class TOC
    {
        public List<TOCContent> content;

        public List<string> relativePaths = new List<string>();

        internal string[] Extract(string path)
        {
            for (int index = 0; index < content.Count; index++)
            {
                bool isFile = Utility.IsFile(path + "/" + content[index].href);

                if (isFile)
                {
                    if (Utility.IsYaml(path + "/" + content[index].href))
                    {
                        string yml = Utility.LoadFile(path + "/" + Utility.RemoveLastDirSlash(content[index].href) +
                                                      "/" + "toc.yml");

                        if (yml != string.Empty)
                        {
                            TOC newTOC = Utility.Deserializer(yml);

                            string path1 = path + "/" + Utility.RemoveLastDirSlash(content[index].href);
                            string[] collection = newTOC.Extract(path1);
                            relativePaths.AddRange(collection);
                        }
                        else
                        {
                            // UnityEngine.Debug.Log(itemPath);
                            string itemPath = path + "/" + Utility.RemoveLastDirSlash(content[index].href) + "/";
                            relativePaths.Add(itemPath);
                        }
                    }
                    else
                    {
                        string itemPath = path + "/" + content[index].href;
                        relativePaths.Add(itemPath);

                        string parentFolder = Utility.RemoveLastDirSlash(itemPath);
                        if (!AssetDatabase.IsValidFolder(parentFolder + "/images"))
                        {
                            AssetDatabase.CreateFolder(parentFolder, "images");
                        }
                    }
                }
                else
                {
                    string yml = Utility.LoadFile(path + "/" + content[index].href + "toc.yml");

                    if (yml != string.Empty && !content[index].href.Contains("_api~"))
                    {
                        TOC newTOC = Utility.Deserializer(yml);

                        string itemPath = path + "/" + Utility.RemoveLastDirSlash(content[index].href);
                        if (!AssetDatabase.IsValidFolder(itemPath + "/images"))
                        {
                            AssetDatabase.CreateFolder(itemPath, "images");
                        }

                        string[] collection = newTOC.Extract(itemPath);
                        relativePaths.AddRange(collection);
                    }
                    else
                    {
                        string itemPath = path + "/" + Utility.RemoveLastDirSlash(content[index].href) + "/";
                        relativePaths.Add(itemPath);
                        if (!AssetDatabase.IsValidFolder(itemPath + "images"))
                        {
                            AssetDatabase.CreateFolder(Utility.RemoveLastDirSlash(itemPath), "images");
                        }
                    }
                }
            }

            if (relativePaths.Count == 0) relativePaths.Add(path + "/");
            return relativePaths.ToArray();
        }
    }

    public struct TOCContent
    {
        public string name;
        public string href;
    }
}