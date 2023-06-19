/*
* author: Sam Ford
* desc: Section for customizing certain messages said by the program
* date started: approx 5/15/2023
*/
namespace DecisionMaker
{
    public class ProfileSection:IDecisionMakerSection
    {
        public const string DEFAULT_PROFILE_DIR = ".\\ProfileStorage\\";
        public const string PROFILE_GREETING_PATH = DEFAULT_PROFILE_DIR + "greeting.txt";
        public const string PROFILE_EXITING_PATH = DEFAULT_PROFILE_DIR + "exiting.txt";
        public const string PROFILE_DISPLAY_NAME_PATH = DEFAULT_PROFILE_DIR + "displayname.txt";
        private const string PROFILE_NO_SAVE_MSG = "Exited without saving any data";

        private const string PROFILE_MENU_GREETING = "Welcome to the Profile Menu. This is where you can customize this program's configurable messages!";
        private const string CHANGE_GREETING_MSG = "Please type a custom greeting message:";
        private const string CHANGE_EXITING_MSG = "Please type a custom exit message:";
        private const string CHANGE_DISPLAY_NAME_MSG = "Please type what you would like us to call you:";
        private const string PS_ERR_INTRO = "ProfileSect.cs: ";

        private enum ProfileParts
        {
            Greeting = 1,
            Exiting,
            DisplayName
        }
        private readonly string[] profileOptions = { "Change app greeting message", "Change app exit message", "Change display name" };
        public Personality appPersonality { get; private set; }

        public ProfileSection()
        {
            checkAndInitDir();
            this.appPersonality = new();
        }

        public static bool checkAndInitDir()
        {
            try
            {
                Directory.CreateDirectory(DEFAULT_PROFILE_DIR);
            }
            catch(Exception e)
            {
                Console.WriteLine($"{PS_ERR_INTRO} failed to initialize {DEFAULT_PROFILE_DIR} directory...\n{e}");
            }
            return Directory.Exists(DEFAULT_PROFILE_DIR);
        }

        public int doMenuLoop()
        {
            Console.WriteLine(PROFILE_MENU_GREETING);
            int opt = MenuUtils.INVALID_OPT;
            do
            {
                checkAndInitDir();
                writeMenu();
                opt = MenuUtils.promptUser();
                processMenuInput(opt);
            } while(!MenuUtils.isChoiceMenuExit(opt));
            scanForProfileUpdates();
            return 0;
        }

        private void writeMenu()
        {
            TextUtils.writeListAsNumberMenu(this.profileOptions.ToList());
            MenuUtils.printExitChoice();
        }

        private void processMenuInput(int opt)
        {
            switch(opt)
            {
                case (int) ProfileParts.Greeting:
                    changeGreeting();
                    break;
                case (int) ProfileParts.Exiting:
                    changeExitMsg();
                    break;
                case (int) ProfileParts.DisplayName:
                    changeDisplayName();
                    break;
                case MenuUtils.EXIT_CODE:
                    MenuUtils.printToPreviousMenu();
                    break;
                default:
                    MenuUtils.writeInvalidMsg();
                    break;
            }
        }

        private void changeGreeting()
        {
            trySaveAnswerToProfile(PROFILE_GREETING_PATH, CHANGE_GREETING_MSG, ProfileParts.Greeting);
        }

        private void changeExitMsg()
        {
            trySaveAnswerToProfile(PROFILE_EXITING_PATH, CHANGE_EXITING_MSG, ProfileParts.Exiting);
        }

        private void changeDisplayName()
        {
            trySaveAnswerToProfile(PROFILE_DISPLAY_NAME_PATH, CHANGE_DISPLAY_NAME_MSG, ProfileParts.DisplayName);
        }

        private void trySaveAnswerToProfile(string path, string prompt, ProfileParts part)
        {
            string ans = "";
            int opt = MenuUtils.INVALID_OPT;
            do
            {
                displayProfilePart(part);
                ans = promptAndGetInput(prompt);
                opt = promptUserConfirmation(ans);
            } while(!MenuUtils.isBinaryInputExit(opt));

            bool saved = MenuUtils.isChoiceYes(opt) ? trySaveProfilePart(path, ans) : false;
            writeProfilePartExitMsg(path, ans, saved);
            scanForProfileUpdates();
        }

        private void displayProfilePart(ProfileParts part)
        {
            string partName = "unknown partname";
            string partVal = "invalid val";
            switch(part)
            {
                case ProfileParts.Greeting:
                    partName = "greeting";
                    partVal = appPersonality.mainGreeting!;
                    break;
                case ProfileParts.Exiting:
                    partName = "exiting";
                    partVal = appPersonality.mainExit!;
                    break;
                case ProfileParts.DisplayName:
                    partName = "display name";
                    partVal = appPersonality.displayName!;
                    break;
                default:
                    break;
            }
            Console.WriteLine($"Current {partName} is {partVal}");
        }

        private string promptAndGetInput(string prompt)
        {
            Console.WriteLine(prompt);
            return Console.ReadLine()!;
        }

        private int promptUserConfirmation(string ans)
        {
            int opt = MenuUtils.INVALID_OPT;
            if (TextUtils.isInputAcceptable(ans))
            {
                Console.WriteLine($"Please confirm you want: {ans}");
                MenuUtils.writeBinaryMenu();
                opt = MenuUtils.promptUser();
            }
            return opt;
        }

        private bool trySaveProfilePart(string path, string ans)
        {
            try
            {
                File.WriteAllText(path, ans);
                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine($"{PS_ERR_INTRO}: Failed to save \"{ans}\" to {path}...\n{e.Message}\n");
                return false;
            }
        }

        private void writeProfilePartExitMsg(string path, string ans, bool saved)
        {
            string exitConfirmMsg = saved ? $"Saving \"{ans}\" to {path}" : PROFILE_NO_SAVE_MSG;
            Console.WriteLine(exitConfirmMsg);
        }

        public void scanForProfileUpdates()
        {
            appPersonality.applyFileChangesToPersonality();
        }
    }
}