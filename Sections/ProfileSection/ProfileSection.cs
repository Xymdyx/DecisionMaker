/*
* author: Sam Ford
* desc: Section for customizing certain messages said by the program
* date started: approx 5/15/2023
*/
using PSC = DecisionMaker.ProfileSectConstants;
namespace DecisionMaker
{
    public class ProfileSection:IDecisionMakerSection
    {
        private enum ProfileParts
        {
            Greeting = 1,
            Exiting,
            DisplayName
        }
        private static readonly string[] profileOptions = { "Change app greeting message", "Change app exit message", "Change display name" };
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
                Directory.CreateDirectory(PSC.DEFAULT_PROFILE_DIR);
            }
            catch(Exception e)
            {
                Console.WriteLine($"{PSC.PS_ERR_INTRO} failed to initialize {PSC.DEFAULT_PROFILE_DIR} directory...\n{e}");
            }
            return Directory.Exists(PSC.DEFAULT_PROFILE_DIR);
        }

        public int doMenuLoop()
        {
            Console.WriteLine(PSC.PROFILE_MENU_GREETING);
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
            TextUtils.writeListAsNumberMenu(PSC.profileOptions.ToList());
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
            trySaveAnswerToProfile(PSC.PROFILE_GREETING_PATH, PSC.CHANGE_GREETING_MSG, ProfileParts.Greeting);
        }

        private void changeExitMsg()
        {
            trySaveAnswerToProfile(PSC.PROFILE_EXITING_PATH, PSC.CHANGE_EXITING_MSG, ProfileParts.Exiting);
        }

        private void changeDisplayName()
        {
            trySaveAnswerToProfile(PSC.PROFILE_DISPLAY_NAME_PATH, PSC.CHANGE_DISPLAY_NAME_MSG, ProfileParts.DisplayName);
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
                Console.WriteLine($"{PSC.PS_ERR_INTRO}: Failed to save \"{ans}\" to {path}...\n{e.Message}\n");
                return false;
            }
        }

        private void writeProfilePartExitMsg(string path, string ans, bool saved)
        {
            string exitConfirmMsg = saved ? $"Saving \"{ans}\" to {path}" : PSC.PROFILE_NO_SAVE_MSG;
            Console.WriteLine(exitConfirmMsg);
        }

        public void scanForProfileUpdates()
        {
            appPersonality.applyFileChangesToPersonality();
        }
    }
}