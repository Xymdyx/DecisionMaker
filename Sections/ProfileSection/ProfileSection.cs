namespace DecisionMaker
{
    public class ProfileSection:IDecisionMakerSection
    {
         private const string PROFILE_DEFAULT_DIR = ".\\ProfileStorage\\";
        private const string PROFILE_DEFAULT_PATH = ".\\ProfileStorage\\profile.txt";
        private const string PROFILE_GREETING_PATH = ".\\ProfileStorage\\greeting.txt";
        private const string PROFILE_EXITING_PATH = ".\\ProfileStorage\\exiting.txt";
        private const string PROFILE_DISPLAY_NAME_PATH = ".\\ProfileStorage\\displayname.txt";

        private const string DEFAULT_GREETING = "Hello there, friend!";
        private const string PROFILE_MENU_GREETING = "Welcome to the Profile Menu. This is where you can customize this program's configurable messages!";
        private const string CHANGE_GREETING_MSG = "Please type a custom greeting message:";
        private const string CHANGE_EXITING_MSG = "Please type a custom exit message:";
        private const string CHANGE_DISPLAY_NAME_MSG = "Please type what you would like us to call you:";
        private const string DEFAULT_EXIT_MSG = "Goodbye, friend. We hope you found what you were looking for!";
        private const int CHANGE_GREETING_CODE = 1;
        private const int CHANGE_EXITING_CODE = 2;
        private const int CHANGE_DISPLAY_NAME_CODE = 3;

        private const string PS_ERR_INTRO = "ProfileSect.cs: ";

        public ProfileSection()
        {
            scanForConfigurations();
            checkAndInitProfileDir();
        }

        private void checkAndInitProfileDir()
        {
            if(!Directory.Exists(PROFILE_DEFAULT_DIR))
            {
                Directory.CreateDirectory(PROFILE_DEFAULT_DIR);
            }
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
            return 0;
        }
        private void writeMenu()
        {
            /* TODO: 5/24/23
            3. Confirm change with a Binary confirmation
            4. Store customizations in txts and read from them at initialization
            */
            Console.WriteLine("1. Change app greeting message\n" +
                            "2. Change app exit message\n" +
                            "3. Change display name");
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
                    Console.WriteLine("Exiting");
                    break;
                default:
                    Console.WriteLine(MenuUtils.INVALID_CHOICE_MSG);
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
            string exitConfirmMsg = saved ? $"Saving \"{ans}\" to {path}" :  "Exited without saving any data";
            Console.WriteLine(exitConfirmMsg);
        }

        private void scanForConfigurations(){}
    }
}