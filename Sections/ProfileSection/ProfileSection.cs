/*
* author: Sam Ford
* desc: Section for customizing certain messages said by the program
* date started: approx 5/15/2023
*/
namespace DecisionMaker
{
    public class ProfileSection:IDecisionMakerSection
    {
        public const string PROFILE_DEFAULT_DIR = ".\\ProfileStorage\\";
        public const string PROFILE_GREETING_PATH = PROFILE_DEFAULT_DIR + "greeting.txt";
        public const string PROFILE_EXITING_PATH = PROFILE_DEFAULT_DIR + "exiting.txt";
        public const string PROFILE_DISPLAY_NAME_PATH = PROFILE_DEFAULT_DIR + "displayname.txt";
        private const string PROFILE_NO_SAVE_MSG = "Exited without saving any data";

        private const string PROFILE_MENU_GREETING = "Welcome to the Profile Menu. This is where you can customize this program's configurable messages!";
        private const string CHANGE_GREETING_MSG = "Please type a custom greeting message:";
        private const string CHANGE_EXITING_MSG = "Please type a custom exit message:";
        private const string CHANGE_DISPLAY_NAME_MSG = "Please type what you would like us to call you:";
        private const string PS_ERR_INTRO = "ProfileSect.cs: ";

        private const int CHANGE_GREETING_CODE = 1;
        private const int CHANGE_EXITING_CODE = 2;
        private const int CHANGE_DISPLAY_NAME_CODE = 3;

        private readonly string[] profileOptions = { "Change app greeting message", "Change app exit message", "Change display name" };
        public Personality appPersonality { get; private set; }

        public ProfileSection()
        {
            checkAndInitProfileDir();
            this.appPersonality = new();
        }

        private void checkAndInitProfileDir()
        {
            if(!Directory.Exists(PROFILE_DEFAULT_DIR))
                Directory.CreateDirectory(PROFILE_DEFAULT_DIR);
        }

        public int doMenuLoop()
        {
            Console.WriteLine(PROFILE_MENU_GREETING);
            int opt = MenuUtils.INVALID_OPT;
            do
            {
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
                case CHANGE_GREETING_CODE:
                    changeGreeting();
                    break;
                case CHANGE_EXITING_CODE:
                    changeExitMsg();
                    break;
                case CHANGE_DISPLAY_NAME_CODE:
                    changeUsername();
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
            trySaveAnswerToProfile(PROFILE_GREETING_PATH, CHANGE_GREETING_MSG);
        }

        private void changeExitMsg()
        {
            trySaveAnswerToProfile(PROFILE_EXITING_PATH, CHANGE_EXITING_MSG);
        }

        private void changeUsername()
        {
            trySaveAnswerToProfile(PROFILE_DISPLAY_NAME_PATH, CHANGE_DISPLAY_NAME_MSG);
        }

        private void trySaveAnswerToProfile(string path, string prompt)
        {
            string ans = "";
            int opt = MenuUtils.INVALID_OPT;
            do
            {
                ans = promptAndGetInput(prompt);
                opt = promptUserConfirmation(ans);
            } while(!MenuUtils.isBinaryChoiceExit(opt));

            bool saved = MenuUtils.isChoiceYes(opt) ? trySaveProfilePart(path, ans) : false;
            writeProfilePartExitMsg(path, ans, saved);
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

        private string promptAndGetInput(string prompt)
        {
            Console.WriteLine(prompt);
            return Console.ReadLine()!;
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
                Console.WriteLine($"{PS_ERR_INTRO}: Failed to save \"{ans}\" to {path}... {e}");
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

        private void decideForUser(List<string> choices){}
        private int runRNG(){return 0;}
    }
}