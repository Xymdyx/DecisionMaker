/*
* author: Sam Ford
* desc: Static class purely for storing Profile Section constants
* date started: approx 6/18/2023
*/

namespace DecisionMaker
{
    internal static class ProfileSectConstants
    {
        internal const string DEFAULT_PROF_DIR = ".\\ProfileStorage\\";        
        internal const string PROFILE_MENU_GREETING = "Welcome to the Profile Menu. This is where you can customize this program's configurable messages!";
        internal const string PS_INFO_INTRO = "ProfileSect.cs: ";

        internal const string GREETING = "greeting";
        internal const string EXITING = "exiting";
        internal const string DISPLAY_NAME = "displayname";
        internal const string PROF_GREETING_PATH = DEFAULT_PROF_DIR + GREETING + TU.TXT;
        internal const string PROF_EXITING_PATH = DEFAULT_PROF_DIR + EXITING + TU.TXT;
        internal const string PROF_DISPLAY_NAME_PATH = DEFAULT_PROF_DIR + DISPLAY_NAME + TU.TXT;

        internal const string CHANGE_GREETING_MSG = "Please type a custom greeting message:";
        internal const string CHANGE_EXITING_MSG = "Please type a custom exit message:";
        internal const string CHANGE_DISPLAY_NAME_MSG = "Please type what you would like us to call you:";
        internal const string PROF_CONFIRM_PART = "Please confirm you want:";
        internal const string PROFILE_NO_SAVE_MSG = "Exited without saving any data";
        internal const string PS_UNKNOWN_PROF_PART = "unknown partname";
        internal enum ProfileParts
        {
            Greeting = 1,
            Exiting,
            DisplayName
        }
        internal static readonly string[] profileOptions = { "Change app greeting message", "Change app exit message", "Change display name" };
    }
}