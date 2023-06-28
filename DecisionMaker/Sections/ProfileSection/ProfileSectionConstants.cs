/*
* author: Sam Ford
* desc: Static class purely for storing Profile Section constants
* date started: approx 6/18/2023
*/

namespace DecisionMaker
{
    internal static class ProfileSectConstants
    {
        internal const string DEFAULT_PROFILE_DIR = ".\\ProfileStorage\\";
        internal const string PROFILE_GREETING_PATH = DEFAULT_PROFILE_DIR + "greeting.txt";
        internal const string PROFILE_EXITING_PATH = DEFAULT_PROFILE_DIR + "exiting.txt";
        internal const string PROFILE_DISPLAY_NAME_PATH = DEFAULT_PROFILE_DIR + "displayname.txt";
        internal const string PROFILE_NO_SAVE_MSG = "Exited without saving any data";

        internal const string PROFILE_MENU_GREETING = "Welcome to the Profile Menu. This is where you can customize this program's configurable messages!";
        internal const string CHANGE_GREETING_MSG = "Please type a custom greeting message:";
        internal const string CHANGE_EXITING_MSG = "Please type a custom exit message:";
        internal const string CHANGE_DISPLAY_NAME_MSG = "Please type what you would like us to call you:";
        internal const string PS_INFO_INTRO = "ProfileSect.cs: ";
        internal enum ProfileParts
        {
            Greeting = 1,
            Exiting,
            DisplayName
        }
        internal static readonly string[] profileOptions = { "Change app greeting message", "Change app exit message", "Change display name" };        
    }
}