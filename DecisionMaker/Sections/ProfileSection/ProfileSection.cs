/*
* author: Sam Ford
* desc: Section for customizing certain messages said by the program
* date started: approx 5/15/2023
*/

namespace DecisionMaker
{
    internal class ProfileSection:IDecisionMakerSection
    {
        internal Personality appPersonality { get; private set; }

        internal ProfileSection()
        {
            checkAndInitDir();
            this.appPersonality = new();
        }

        internal static bool checkAndInitDir()
        {
            try
            {
                Directory.CreateDirectory(PSC.DEFAULT_PROFILE_DIR);
            }
            catch(Exception e)
            {
                Console.WriteLine($"{PSC.PS_INFO_INTRO} failed to initialize {PSC.DEFAULT_PROFILE_DIR} directory...");
                TU.logErrorMsg(e);
            }
            return Directory.Exists(PSC.DEFAULT_PROFILE_DIR);
        }

        internal int doMenuLoop()
        {
            Console.WriteLine(PSC.PROFILE_MENU_GREETING);
            int opt = MenuUtils.INVALID_OPT;
            do
            {
                checkAndInitDir();
                writeMenu();
                opt = MenuUtils.promptUserAndReturnOpt();
                processMenuInput(opt);
            } while(!MenuUtils.isChoiceMenuExit(opt));
            scanForProfileUpdates();
            return opt;
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
                case (int) PSC.ProfileParts.Greeting:
                    changeGreeting();
                    break;
                case (int) PSC.ProfileParts.Exiting:
                    changeExitMsg();
                    break;
                case (int) PSC.ProfileParts.DisplayName:
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
            trySaveAnswerToProfile(PSC.PROFILE_GREETING_PATH, PSC.CHANGE_GREETING_MSG, PSC.ProfileParts.Greeting);
        }

        private void changeExitMsg()
        {
            trySaveAnswerToProfile(PSC.PROFILE_EXITING_PATH, PSC.CHANGE_EXITING_MSG, PSC.ProfileParts.Exiting);
        }

        private void changeDisplayName()
        {
            trySaveAnswerToProfile(PSC.PROFILE_DISPLAY_NAME_PATH, PSC.CHANGE_DISPLAY_NAME_MSG, PSC.ProfileParts.DisplayName);
        }

        private void trySaveAnswerToProfile(string path, string prompt, PSC.ProfileParts part)
        {
            string ans = TU.BLANK;
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

        private void displayProfilePart(PSC.ProfileParts part)
        {
            string partName = "unknown partname";
            string partVal = "invalid val";
            switch(part)
            {
                case PSC.ProfileParts.Greeting:
                    partName = "greeting";
                    partVal = appPersonality.mainGreeting!;
                    break;
                case PSC.ProfileParts.Exiting:
                    partName = "exiting";
                    partVal = appPersonality.mainExit!;
                    break;
                case PSC.ProfileParts.DisplayName:
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
                opt = MenuUtils.promptUserAndReturnOpt();
            }
            return opt;
        }

        internal bool trySaveProfilePart(string path, string ans)
        {
            try
            {
                if (isPathProfilePart(path))
                {
                    checkAndInitDir();
                    File.WriteAllText(path, ans);
                }
                else
                    Console.WriteLine($"{PSC.PS_INFO_INTRO} {path} doesn't belong in {PSC.DEFAULT_PROFILE_DIR} directory!");
            }
            catch(Exception e)
            {
                Console.WriteLine($"{PSC.PS_INFO_INTRO} Failed to save \"{ans}\" to {path}...");
                TU.logErrorMsg(e);
            }
            return File.Exists(path);
        }

        private bool isPathProfilePart(string path)
        {
            return path == PSC.PROFILE_DISPLAY_NAME_PATH || path == PSC.PROFILE_EXITING_PATH || path == PSC.PROFILE_GREETING_PATH;
        }

        private void writeProfilePartExitMsg(string path, string ans, bool saved)
        {
            string exitConfirmMsg = saved ? $"Saving \"{ans}\" to {path}" : PSC.PROFILE_NO_SAVE_MSG;
            Console.WriteLine(exitConfirmMsg);
        }

        internal void scanForProfileUpdates()
        {
            appPersonality.applyFileChangesToPersonality();
        }

        internal bool saveEntireProfile()
        {
            bool success = true;
            success &= trySaveProfilePart(PSC.PROFILE_DISPLAY_NAME_PATH, appPersonality.displayName!);
            success &= trySaveProfilePart(PSC.PROFILE_GREETING_PATH, appPersonality.mainGreeting!);
            success &= trySaveProfilePart(PSC.PROFILE_EXITING_PATH, appPersonality.mainExit!);

            if(!success)
                Console.WriteLine($"{PSC.PS_INFO_INTRO} Failed to save all personality files!");
            return success;
        }        
    }
}