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
            return MU.checkAndInitADir(PSC.DEFAULT_PROF_DIR);
        }

        internal int doMenuLoop()
        {
            Console.WriteLine(PSC.PROFILE_MENU_GREETING);
            int opt = MU.INVALID_OPT;
            do
            {
                checkAndInitDir();
                writeMenu();
                opt = MU.promptUserAndReturnOpt();
                processMenuInput(opt);
            } while(!MU.isChoiceMenuExit(opt));
            scanForProfileUpdates();
            return opt;
        }

        private void writeMenu()
        {
            TU.writeListAsNumberMenu(PSC.profileOptions.ToList());
            MU.printExitChoice();
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
                case MU.EXIT_CODE:
                    MU.printToPreviousMenu();
                    break;
                default:
                    MU.writeInvalidMsg();
                    break;
            }
        }

        private void changeGreeting()
        {
            trySaveAnswerToProfile(PSC.PROF_GREETING_PATH, PSC.CHANGE_GREETING_MSG, PSC.ProfileParts.Greeting);
        }

        private void changeExitMsg()
        {
            trySaveAnswerToProfile(PSC.PROF_EXITING_PATH, PSC.CHANGE_EXITING_MSG, PSC.ProfileParts.Exiting);
        }

        private void changeDisplayName()
        {
            trySaveAnswerToProfile(PSC.PROF_DISPLAY_NAME_PATH, PSC.CHANGE_DISPLAY_NAME_MSG, PSC.ProfileParts.DisplayName);
        }

        /// <summary>
        /// helper method that tries to save a text input by user to a corresponding profile part
        /// </summary>
        /// <param name="path">a constant path given another method in this file</param>
        /// <param name="prompt">a constant prompt given by another method in this file</param>
        /// <param name="part">the ProfileParts enum that corresponds to the prompt and path</param>
        /// <returns>bool- whether the user's answer was saved to the desired profile path</returns>
        private bool trySaveAnswerToProfile(string path, string prompt, PSC.ProfileParts part)
        {
            string ans = TU.BLANK;
            int opt = MU.INVALID_OPT;
            do
            {
                printProfilePart(part);
                ans = promptAndGetText(prompt);
                opt = promptUserConfirmation(ans);
            } while(!MU.isBinaryInputExit(opt));

            bool saved = MU.isChoiceYes(opt) ? trySaveProfilePart(path, ans) : false;
            writeProfilePartExitMsg(path, ans, saved);
            scanForProfileUpdates();
            return saved;
        }

        private void printProfilePart(PSC.ProfileParts part)
        {
            string partName = PSC.PS_UNKNOWN_PROF_PART;
            string partVal = TU.BLANK;
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
            Console.WriteLine($"Current profile {partName}: {partVal}");
        }

        private string promptAndGetText(string prompt)
        {
            Console.WriteLine(prompt);
            return Console.ReadLine()!;
        }

        private int promptUserConfirmation(string ans)
        {
            int opt = MU.INVALID_OPT;
            if (TU.isInputAcceptable(ans))
            {
                Console.WriteLine($"{PSC.PROF_CONFIRM_PART} {ans}");
                MU.writeBinaryMenu();
                opt = MU.promptUserAndReturnOpt();
            }
            return opt;
        }

        /// <summary>
        /// helper method for trySaveAnswerToProfile
        /// </summary>
        /// <param name="path">a constant path given another method in this file</param>
        /// <param name="ans">validated text answer given by user</param>
        /// <returns>bool- whether the user's answer was saved to the desired profile path</returns>
        internal bool trySaveProfilePart(string path, string ans)
        {
            bool saved = false;
            try
            {
                if (isPathProfilePart(path) && TU.isInputAcceptable(ans))
                {
                    checkAndInitDir();
                    File.WriteAllText(path, ans);
                    saved = true;
                }
                else
                    Console.WriteLine($"{PSC.PS_INFO_INTRO} {path} doesn't belong in {PSC.DEFAULT_PROF_DIR} directory!");
            }
            catch(Exception e)
            {
                Console.WriteLine($"{PSC.PS_INFO_INTRO} Failed to save \"{ans}\" to {path}...");
                TU.logErrorMsg(e);
            }
            return saved;
        }

        private bool isPathProfilePart(string path)
        {
            return path == PSC.PROF_DISPLAY_NAME_PATH || path == PSC.PROF_EXITING_PATH || path == PSC.PROF_GREETING_PATH;
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
            success &= trySaveProfilePart(PSC.PROF_DISPLAY_NAME_PATH, appPersonality.displayName!);
            success &= trySaveProfilePart(PSC.PROF_GREETING_PATH, appPersonality.mainGreeting!);
            success &= trySaveProfilePart(PSC.PROF_EXITING_PATH, appPersonality.mainExit!);

            if(!success)
                Console.WriteLine($"{PSC.PS_INFO_INTRO} Failed to save all personality files!");
            return success;
        }        
    }
}