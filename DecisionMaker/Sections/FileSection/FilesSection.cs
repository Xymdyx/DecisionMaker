/*
* author: Sam Ford
* desc: Section for deleting files made by the program
* date started: approx 5/30/2023
*/

namespace DecisionMaker
{
    internal class FilesSection:IDecisionMakerSection
    {
        private DecisionsSection decSect;
        private ProfileSection profSect;

        internal FilesSection(DecisionsSection ds, ProfileSection ps)
        {
            this.decSect = ds;
            this.profSect = ps;
            checkAndInitDir();
        }

        internal static bool checkAndInitDir()
        {
            return MU.checkAndInitADir(FSC.DEFAULT_FILES_DIR);
        }

        internal int doMenuLoop()
        {
            int opt = MenuUtils.INVALID_OPT;
            Console.WriteLine(FSC.FS_GREETING);
            checkAndInitDir();
            do
            {
                writeMenu();
                opt = MenuUtils.promptUserAndReturnOpt();
                processMenuInput(opt);
            } while (!MenuUtils.isChoiceMenuExit(opt));
            return opt;
        }

        private void writeMenu()
        {
            TextUtils.writeListAsNumberMenu(FSC.fileTypes.ToList());
            MenuUtils.printExitChoice();
        }

        private void processMenuInput(int opt)
        {
            switch(opt)
            {
                case (int) FSC.FileTypeCodes.DCFiles:
                    doDcsLoop();
                    break;
                case (int) FSC.FileTypeCodes.ProfileFiles:
                    doProfileLoop();
                    break;
                case (int) FSC.FileTypeCodes.WipFile:
                    doFileMenuLoop(FSC.DEFAULT_WIP_FILE);
                    break;
                case MenuUtils.EXIT_CODE:
                    MenuUtils.printToPreviousMenu();
                    break;
                default:
                    MenuUtils.writeInvalidMsg();
                    break;
            }
        }

        private int doDcsLoop()
        {
            int opt = MenuUtils.INVALID_OPT;
            do
            {
                writeManageDcsMenu();
                opt = MenuUtils.promptUserAndReturnOpt();
                processDcsMenuInput(opt);
            } while (!MenuUtils.isChoiceMenuExit(opt));
            return opt;
        }

        private void writeManageDcsMenu()
        {
            decSect.printSavedDcs();
            MenuUtils.printExitChoice();
            Console.WriteLine($"{(int)FSC.FileActionCodes.DeleteAll}. Delete all decision categories");
        }

        private void processDcsMenuInput(int opt)
        {
            if (decSect.isChoiceExistingDc(opt))
                manageChosenDc(opt);
            else if(isOptDeleteAllFiles(opt))
                deleteAllDcs();
            else
                MenuUtils.writeInvalidMsg();
        }

        private void manageChosenDc(int opt)
        {
            DecisionCategory dc = decSect.getDcFromMenuChoice(opt);
            int dcOpt = doFileMenuLoop(dc.CatPath);
            if (dcOpt == (int)FSC.FileActionCodes.DeleteFile && !dc.checkFileExists())
                this.decSect.DcMap.Remove(dc.CatName);
        }

        private bool isOptDeleteAllFiles(int opt)
        {
            return opt == (int)FSC.FileActionCodes.DeleteAll;
        }

        private void deleteAllDcs()
        {
            deleteDirAndContents(DSC.DEFAULT_DC_DIRECTORY);
            this.decSect.removeDcsFromMapNotInDir();
        }

        internal static void deleteDirAndContents(string dir)
        {
            try
            {
                Directory.Delete(dir, true);
            }
            catch(Exception e)
            {
                Console.WriteLine($"{FSC.FS_INFO_INTRO}: Failed to delete entire {dir} directory...");
                TU.logErrorMsg(e);
            }
        }

        private int doProfileLoop()
        {
            int opt = MenuUtils.INVALID_OPT;
            do
            {
                writeManageProfileMenu();
                opt = MenuUtils.promptUserAndReturnOpt();
                processProfileMenuInput(opt);
                profSect.scanForProfileUpdates();
            } while (!MenuUtils.isChoiceMenuExit(opt));
            return opt;
        }

        private void writeManageProfileMenu()
        {
            TextUtils.writeListAsNumberMenu(FSC.manageProfileActions.ToList());
            MenuUtils.printExitChoice();
            Console.WriteLine($"{(int)FSC.FileActionCodes.DeleteAll}. Delete entire profile");
        }

        private void processProfileMenuInput(int opt)
        {
            switch(opt)
            {
                case (int) FSC.ProfileActionCodes.GREETING:
                    doFileMenuLoop(PSC.PROFILE_GREETING_PATH);
                    break;
                case (int) FSC.ProfileActionCodes.DEPARTING:
                    doFileMenuLoop(PSC.PROFILE_EXITING_PATH);
                    break;
                case (int) FSC.ProfileActionCodes.DISPLAYNAME:
                    doFileMenuLoop(PSC.PROFILE_DISPLAY_NAME_PATH);
                    break;
                case (int)FSC.FileActionCodes.DeleteAll:
                    deleteDirAndContents(PSC.DEFAULT_PROFILE_DIR);
                    break;
                case MenuUtils.EXIT_CODE:
                    MenuUtils.printToPreviousMenu();
                    break;
                default:
                    MenuUtils.writeInvalidMsg();
                    break;
            }
        }

        /// <summary>
        /// menu for acting on a selected fName
        /// </summary>
        /// <param name="fName">a file name chosen previously by user</param>
        /// <returns>int- the action chosen on fName</returns>
        private int doFileMenuLoop(string fName)
        {
            int opt = MenuUtils.INVALID_OPT;
            do
            {
                writeFileMenu(fName);
                opt = MenuUtils.promptUserAndReturnOpt();
                processFileActions(opt, fName);
                Console.WriteLine();
            } while (continueFileMenuLoop(opt));
            return opt;
        }

        private bool continueFileMenuLoop(int opt)
        {
            return (!MenuUtils.isChoiceMenuExit(opt)) && (opt != (int)FSC.FileActionCodes.DeleteFile);
        }

        private void writeFileMenu(string fName)
        {
            Console.WriteLine($"Please select what you would like to do with the {fName} file:");
            TextUtils.writeListAsNumberMenu(FSC.manageFileActions.ToList());
            MenuUtils.printExitChoice();
        }

        private void processFileActions(int opt, string fName)
        {
            switch(opt)
            {
                case (int) FSC.FileActionCodes.ViewFile:
                    viewFileContents(fName);
                    break;
                case (int) FSC.FileActionCodes.DeleteFile:
                    deleteManageableFile(fName);
                    break;
                case MenuUtils.EXIT_CODE:
                    MenuUtils.printToPreviousMenu();
                    break;
                default:
                    MenuUtils.writeInvalidMsg();
                    break;
            }
        }

        /// <summary>
        /// print out a files contents to console and return its string
        /// </summary>
        /// <param name="fPath">a file path</param>
        /// <returns> string contents of a file or empty string if error</returns>
        internal static string viewFileContents(string fPath)
        {
            string fileLines = TU.BLANK;
            try
            {
                if (File.Exists(fPath))
                {
                    string info = $"Contents of {fPath}:\n";
                    fileLines = string.Join("\n", File.ReadAllLines(fPath));
                    Console.WriteLine(info + fileLines);
                }
                else
                    Console.WriteLine($"{fPath} doesn't exist therefore cannot read!");
            }
            catch(Exception e)
            {
                Console.WriteLine($"{FSC.FS_INFO_INTRO} failed to read contents of {fPath}...\n{e.Message}\n") ;
            }
            return fileLines;
        }

        /// <summary>
        /// try to delete a file
        /// </summary>
        /// <param name="fPath">a file path</param>
        /// <returns>bool - whether file exists after this operation</returns>
        internal static bool deleteManageableFile(string fPath)
        {
            try
            {
                if (File.Exists(fPath))
                {
                    Console.WriteLine($"Trying to delete {fPath}...");
                    File.Delete(fPath);
                    Console.WriteLine(FSC.DELETE_SUCCESS);
                }
                else
                    Console.WriteLine($"{fPath} doesn't exist therefore cannot delete!");
            }
            catch(Exception e)
            {
                Console.WriteLine($"{FSC.FS_INFO_INTRO} failed to delete {fPath}...");
                TU.logErrorMsg(e);
            }
            return !File.Exists(fPath);
        }

        /// <summary>
        /// save all files on application close
        /// </summary>
        /// <returns> bool - whether all DC and Profile files were saved before exiting</returns>
        internal bool saveFilesBeforeExit()
        {
            bool savedDcs = this.decSect.saveAllDcsInMap();
            bool savedProfile = this.profSect.saveEntireProfile();
            return savedDcs && savedProfile;
        }
    }
}