using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace ArioSoren.DocsUtility.DocsGenerator.Editor
{
    public static class HttpServerManager
    {
        private static string _configFilePath = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Library", "servers.json");
        private static int _startPort = 36700;
        private static int _endPort = 36799;

        public static int StartServer(string rootDirectory)
        {

            ValidateAndCleanConfigs();

            var config = GetServerConfigByRootDirectory(rootDirectory);

            if (config != null)
            {
                UnityEngine.Debug.Log($"Exist Port: {config.Port}");
                return config.Port;
            }

            int port = GetFreePort();

            UnityEngine.Debug.Log($"Port: {port}");


            if (port == -1)
            {
                UnityEngine.Debug.LogError("No free port found in the specified range.");
                return -1;
            }

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = Path.GetFullPath("Packages/com.ariosoren.docs-utility/DocsGenerator/ExternalTools~/SimpleHTTPServer/SimpleHTTPServer.exe"),
                // Arguments = $"-i --nocache --port {port} \"{rootDirectory}\"",
                Arguments = $"\"{rootDirectory}\" {port}",
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            try
            {

                // string commend =
                //     $"{startInfo.FileName} {startInfo.Arguments}";


                //     UnityEngine.Debug.Log($"commend: {commend}");

                //  Process   process = Process.Start("powershell.exe", commend);

                Process process = new Process
                {
                    StartInfo = startInfo
                };
                process.Start();

                UnityEngine.Debug.Log($"Started process ID: {process.Id}");

                int processId = process.Id;
                SaveConfig(rootDirectory, port, processId);

                return port;
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"Failed to start process: {ex.Message}");
                return -1;
            }
        }

        public static void StopServerById(int processId)
        {
            try
            {
                var process = Process.GetProcessById(processId);
                if (process != null)
                {
                    process.Kill();
                    RemoveConfigById(processId);
                }
            }
            catch (ArgumentException ex)
            {
                UnityEngine.Debug.LogError($"Process with ID {processId} not found: {ex.Message}");
            }
        }

        public static void StopServerByPort(int port)
        {
            var serverConfig = GetServerConfigByPort(port);
            if (serverConfig != null)
            {
                StopServerById(serverConfig.ProcessId);
            }
            else
            {
                UnityEngine.Debug.LogError($"No server found running on port {port}");
            }
        }

        private static int GetFreePort()
        {
            for (int port = _startPort; port <= _endPort; port++)
            {
                if (IsPortAvailable(port))
                {
                    return port;
                }
            }
            return -1; // No available port found in the range
        }

        private static bool IsPortAvailable(int port)
        {
            try
            {
                using (var client = new TcpClient())
                {
                    client.Connect(IPAddress.Loopback, port);
                    return false;
                }
            }
            catch (SocketException)
            {
                return true;
            }
        }

        private static void SaveConfig(string rootDirectory, int port, int processId)
        {
            var serverConfigs = LoadConfig();
            serverConfigs.Servers.Add(new ServerConfig { RootDirectory = rootDirectory, Port = port, ProcessId = processId });
            File.WriteAllText(_configFilePath, JsonUtility.ToJson(serverConfigs));
        }

        private static void RemoveConfigById(int processId)
        {
            var serverConfigs = LoadConfig();
            serverConfigs.Servers.RemoveAll(s => s.ProcessId == processId);
            File.WriteAllText(_configFilePath, JsonUtility.ToJson(serverConfigs));
        }

        private static ServerConfig GetServerConfigByPort(int port)
        {
            var serverConfigs = LoadConfig();
            return serverConfigs.Servers.Find(s => s.Port == port);
        }

        private static ServerConfig GetServerConfigByRootDirectory(string rootDirectory)
        {
            var serverConfigs = LoadConfig();
            return serverConfigs.Servers.Find(s => s.RootDirectory == rootDirectory);
        }

        private static ServerConfigList LoadConfig()
        {
            if (File.Exists(_configFilePath))
            {
                return JsonUtility.FromJson<ServerConfigList>(File.ReadAllText(_configFilePath));
            }
            else
            {
                var emptyConfigList = new ServerConfigList();
                File.WriteAllText(_configFilePath, JsonUtility.ToJson(emptyConfigList));
                return emptyConfigList;
            }
        }

        public static void ValidateAndCleanConfigs()
        {
            var serverConfigs = LoadConfig();
            bool configChanged = false;

            for (int i = serverConfigs.Servers.Count - 1; i >= 0; i--)
            {
                var config = serverConfigs.Servers[i];
                bool isProcessValid = true;
                bool isPortAvailable = IsPortAvailable(config.Port);

                try
                {
                    var process = Process.GetProcessById(config.ProcessId);
                    if (process == null || process.HasExited)
                    {
                        isProcessValid = false;
                    }
                }
                catch (ArgumentException)
                {
                    isProcessValid = false;
                }

                if (!isProcessValid || isPortAvailable)
                {
                    serverConfigs.Servers.RemoveAt(i);
                    configChanged = true;
                }
            }

            if (configChanged)
            {
                File.WriteAllText(_configFilePath, JsonUtility.ToJson(serverConfigs));
                UnityEngine.Debug.Log("Invalid server configurations have been cleaned up.");
            }
            else
            {
                UnityEngine.Debug.Log("All server configurations are valid.");
            }
        }


        [Serializable]
        public class ServerConfig
        {
            public string RootDirectory;
            public int Port;
            public int ProcessId;
        }

        [Serializable]
        public class ServerConfigList
        {
            public List<ServerConfig> Servers = new List<ServerConfig>();
        }
    }
}