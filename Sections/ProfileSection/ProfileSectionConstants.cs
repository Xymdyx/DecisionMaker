/*
* author: Sam Ford
* desc: Static class purely for storing Profile Section constants
* date started: approx 6/18/2023
*/
namespace DecisionMaker
{
    public static class ProfileSectConstants
    {
        public const string DEFAULT_PROFILE_DIR = ".\\ProfileStorage\\";
        public const string PROFILE_GREETING_PATH = DEFAULT_PROFILE_DIR + "greeting.txt";
        public const string PROFILE_EXITING_PATH = DEFAULT_PROFILE_DIR + "exiting.txt";
        public const string PROFILE_DISPLAY_NAME_PATH = DEFAULT_PROFILE_DIR + "displayname.txt";
        public const string PROFILE_NO_SAVE_MSG = "Exited without saving any data";

        public const string PROFILE_MENU_GREETING = "Welcome to the Profile Menu. This is where you can customize this program's configurable messages!";
        public const string CHANGE_GREETING_MSG = "Please type a custom greeting message:";
        public const string CHANGE_EXITING_MSG = "Please type a custom exit message:";
        public const string CHANGE_DISPLAY_NAME_MSG = "Please type what you would like us to call you:";
        public const string PS_ERR_INTRO = "ProfileSect.cs: ";

        public enum ProfileParts
        {
            Greeting = 1,
            Exiting,
            DisplayName
        }
        public static readonly string[] profileOptions = { "Change app greeting message", "Change app exit message", "Change display name" };        
    }
}