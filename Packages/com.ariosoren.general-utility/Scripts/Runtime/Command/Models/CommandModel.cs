namespace ArioSoren.GeneralUtility.Command.Models
{
    /// <summary>
    /// The model to represent command.
    /// </summary>
    public class CommandModel
    {
        /// <summary>
        /// The command to be executed on the command line, terminal, etc.
        /// </summary>
        public string Command;
        
        /// <summary>
        /// The path to the folder where the command will be executed. 
        /// </summary>
        public string RootFolder;
    }
}
