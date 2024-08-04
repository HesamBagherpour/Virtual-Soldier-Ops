using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using ArioSoren.GeneralUtility.Command.Models;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace ArioSoren.GeneralUtility.Command
{
    public static class Executor
    {
        /// <summary>
        /// A string that has consisted of all of the intended command models and the execution option.
        /// </summary>
        private static string _command = "";
        
        /// <summary>
        /// The name and the format of the command executor application.
        /// </summary>
        private static string _functorApp = "";
        
        /// <summary>
        /// Executes intended commands.
        /// </summary>
        /// <param name="commands">The list of intended command models.
        /// any element of this list includes a command and a path to where the command should execute.</param>
        /// <param name="executeCommandOption">The option of command execution.</param>
        public static void ExecuteCommand(List<CommandModel> commands, ExecuteCommandOption executeCommandOption)
        {
            // Puts all of the command models together in a single string (_command) and also sets the command executor application (_functorApp).
            // These are determined according to the OS.
            switch (SystemInfo.operatingSystemFamily)
            {
                case OperatingSystemFamily.Windows:
                    _command = CreateWindowsCommand(commands, executeCommandOption);
                    _functorApp = "cmd.exe";
                    break;
                case OperatingSystemFamily.Linux:
                    _command = CreateLinuxCommand(commands, executeCommandOption);
                    _functorApp = "Terminal";
                    break;
                case OperatingSystemFamily.MacOSX:
                    _command = CreateMacOsCommand(commands, executeCommandOption);
                    _functorApp = "Terminal";
                    break;
                case OperatingSystemFamily.Other:
                    Debug.unityLogger.LogError("Platform","No supported OS!");
                    break;
                default:
                    Debug.unityLogger.LogError("Platform","No supported OS!");
                    return;
            }

            RunCommands(_functorApp, _command);
        }

        /// <summary>
        /// Puts all of the command models together in a single string.
        /// This method produces a string of commands for cmd (the Windows command application).
        /// </summary>
        /// <param name="commands">This is the list of intended command models.
        /// any element of this list includes a command and a path to where the command should execute.</param>
        /// <param name="executeCommandOption">The option of command execution.</param>
        /// <returns>A string that has consisted of all of the intended command models and the execution option.
        /// This string could be run in the Microsoft Windows command executor app (cmd).</returns>
        private static string CreateWindowsCommand(List<CommandModel> commands, ExecuteCommandOption executeCommandOption)
        {
            var command = new StringBuilder();
            command.Append(executeCommandOption.AutoCloseCommandWindow ? "/C " : "/K ");

            foreach (var commandModel in commands)
            {
                if (!string.IsNullOrEmpty(commandModel.RootFolder))
                {
                    command.Append($"{commandModel.RootFolder[0]}: ");
                    command.Append($"&cd {commandModel.RootFolder} ");
                }
                    
                command.Append($"&{commandModel.Command} ");
            }

            return command.ToString();
        }

        /// <summary>
        /// Puts all of the command models together in a single string.
        /// This method produces a string of commands to run on the Linux command executor application.
        /// ToDo: Write the body. Method doesn't have a body yet.
        /// </summary>
        /// <param name="commands">This is the list of intended command models.
        /// any element of this list includes a command and a path to where the command should execute.</param>
        /// <param name="executeCommandOption">The option of command execution.</param>
        /// <returns>A string that has consisted of all of the intended command models and the execution option.
        /// This string could be run on the Linux command executor application.</returns>
        private static string CreateLinuxCommand(List<CommandModel> commands, ExecuteCommandOption executeCommandOption)
        {
            // todo
            return "";
        }

        /// <summary>
        /// Puts all of the command models together in a single string.
        /// This method produces a string of commands to run on the MacOs command executor application.
        /// ToDo: Write the body. Method doesn't have a body yet.
        /// </summary>
        /// <param name="commands">This is the list of intended command models.
        /// any element of this list includes a command and a path to where the command should execute.</param>
        /// <param name="executeCommandOption">The option of command execution.</param>
        /// <returns>A string that has consisted of all of the intended command models and the execution option.
        /// This string could be run on the MacOs command executor application.</returns>
        private static string CreateMacOsCommand(List<CommandModel> commands, ExecuteCommandOption executeCommandOption)
        {
            // todo
            return "";
        }

        /// <summary>
        /// Runs the intended commands on the intended command executor application. 
        /// </summary>
        /// <param name="functorApp">The intended command executor application.</param>
        /// <param name="command">The intended commands</param>
        private static void RunCommands(string functorApp, string command)
        {
            Process.Start(functorApp, command);
        }
    }
}