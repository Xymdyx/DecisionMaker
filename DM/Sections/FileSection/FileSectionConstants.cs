/*
* author: Xymdyx
* desc: Static class purely for storing File Section constants
* date started: approx 6/18/2023
*/

namespace DecisionMaker
{
    internal static class FileSectConstants
    {
        internal const string DEFAULT_FILES_DIR = ".\\FileManagement\\";
        internal const string DEFAULT_WIP_FILE = DEFAULT_FILES_DIR + "wipcat" + TU.TXT;
        internal const string FS_GREETING = "Welcome to File Management. This is where you can delete any files generated by this program.";
        internal const string FS_INFO_INTRO = "FilesSection.cs: ";
        internal const string DELETE_SUCCESS = "Delete successful!";
        internal const string HOW_FIND_WIP = "The wip file is located in the FileManagement section and you can choose what to do with it there.\n";
        internal static readonly string[] fileTypes = { "Decision Categories", "Custom Profile", "WIP File" };
        internal static readonly string[] manageFileActions = {"View contents", "Delete file"};
        internal static readonly string[] manageProfileActions = { "Greeting", "Exiting", "Display Name" };

        internal enum FileTypeCodes
        {
            DCFiles = 1,
            ProfileFiles,
            WipFile
        }

        internal enum FileActionCodes 
        {
            DeleteAll = -1,
            ViewFile = 1,
            DeleteFile
        }

        internal enum ProfileActionCodes
        {
            GREETING = 1,
            DEPARTING,
            DISPLAYNAME
        }        
    }
}