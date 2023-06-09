namespace DecisionMaker
{
    public class ProfileSection:IDecisionMakerSection
    {
        public const string PROFILE_DEFAULT_DIR = ".\\ProfileStorage\\";
        public const string PROFILE_GREETING_PATH = PROFILE_DEFAULT_DIR + "greeting.txt";
        public const string PROFILE_EXITING_PATH = PROFILE_DEFAULT_DIR + "exiting.txt";
        public const string PROFILE_DISPLAY_NAME_PATH = PROFILE_DEFAULT_DIR + "displayname.txt";
        private const string PROFILE_NO_SAVE_MSG = "Exited without saving any data";

        private const string DEFAULT_GREETING = "Hello there!";
        private const string PROFILE_MENU_GREETING = "Welcome to the Profile Menu. This is where you can customize this program's configurable messages!";
        private const string CHANGE_GREETING_MSG = "Please type a custom greeting message:";
        private const string CHANGE_EXITING_MSG = "Please type a custom exit message:";
        private const string CHANGE_DISPLAY_NAME_MSG = "Please type what you would like us to call you:";
        private const string DEFAULT_EXIT_MSG = "Goodbye, friend. We hope you found what you were looking for!";
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

        private Personality scanForConfigurations()
        {
            string greeting = "";
            string exiting = ""; 
            string displayName = "";

            if(File.Exists(PROFILE_GREETING_PATH))
                greeting = File.ReadAllText(PROFILE_GREETING_PATH);
            if(File.Exists(PROFILE_EXITING_PATH))
                exiting = File.ReadAllText(PROFILE_EXITING_PATH);
            if(File.Exists(PROFILE_DISPLAY_NAME_PATH))
                displayName = File.ReadAllText(PROFILE_DISPLAY_NAME_PATH);

            return new Personality(greeting, exiting, displayName);
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

        private void readExistingLists(){}
        private void addItemToList(){}
        private void decideForUser(List<string> choices){}
        private int runRNG(){return 0;}
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
    }
}