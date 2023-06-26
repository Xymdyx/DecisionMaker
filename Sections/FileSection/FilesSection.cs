/*
* author: Sam Ford
* desc: Section for deleting files made by the program
* date started: approx 5/30/2023
*/

using FSC = DecisionMaker.FileSectConstants;
using TU = DecisionMaker.TextUtils;
namespace DecisionMaker
{
    using PSC = DecisionMaker.ProfileSectConstants;
    using DSC = DecisionMaker.DecisionSectConstants;
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
            try
            {
                Directory.CreateDirectory(FSC.DEFAULT_FILES_DIR);
            }
            catch (Exception e)
            {
                Console.WriteLine($"{FSC.FS_ERR_HEADER} failed to initialize {FSC.DEFAULT_FILES_DIR} directory...\n{e}");
            }
            return Directory.Exists(FSC.DEFAULT_FILES_DIR);
        }        

        internal int doMenuLoop()
        {
            int opt = MenuUtils.INVALID_OPT;
            Console.WriteLine(FSC.FM_GREETING);
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
                    doDCsLoop();
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

        private int doDCsLoop()
        {
            int opt = MenuUtils.INVALID_OPT;
            do
            {
                writeManageDCsMenu();
                opt = MenuUtils.promptUserAndReturnOpt();
                processDCsMenuInput(opt);
            } while (!MenuUtils.isChoiceMenuExit(opt));
            return opt;
        }

        private void writeManageDCsMenu()
        {
            decSect.printSavedDCs();
            MenuUtils.printExitChoice();
            Console.WriteLine($"{(int)FSC.FileActionCodes.DeleteAll}. Delete all decision categories");
        }

        private void processDCsMenuInput(int opt)
        {
            if (decSect.isChoiceInChoiceRange(opt))
                manageChosenDC(opt);
            else if(isOptDeleteAll(opt))
                deleteAllDCs();
            else
                MenuUtils.writeInvalidMsg();
        }

        private void manageChosenDC(int opt)
        {
            DecisionCategory dc = decSect.getDCFromMenuChoice(opt);
            int dcOpt = doFileMenuLoop(dc.CatPath);
            if (dcOpt == (int)FSC.FileActionCodes.DeleteFile && !dc.checkFileExists())
                this.decSect.DcMap.Remove(dc.CatName);
        }

        private bool isOptDeleteAll(int opt)
        {
            return opt == (int)FSC.FileActionCodes.DeleteAll;
        }

        private void deleteAllDCs()
        {
            deleteDirAndContents(DSC.DEFAULT_DC_DIRECTORY);
            this.decSect.syncDcMapToDcDir();
        }

        private void deleteDirAndContents(string dir)
        {
            try
            {
                Directory.Delete(dir, true);
            }
            catch(Exception e)
            {
                Console.WriteLine($"{FSC.FS_ERR_HEADER}: Failed to delete entire {dir} directory...");
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

        private int doFileMenuLoop(string fName)
        {
            int opt = MenuUtils.INVALID_OPT;
            do
            {
                writeFileMenu();
                opt = MenuUtils.promptUserAndReturnOpt();
                processFileActions(opt, fName);
            } while (continueFileMenuLoop(opt));
            return opt;
        }

        private bool continueFileMenuLoop(int opt)
        {
            return (!MenuUtils.isChoiceMenuExit(opt)) && (opt != (int)FSC.FileActionCodes.DeleteFile);
        }

        private void writeFileMenu()
        {
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

        internal static string viewFileContents(string fName)
        {
            string fileLines = "";
            try
            {
                if (File.Exists(fName))
                {
                    string info = $"Contents of {fName}:\n";
                    fileLines = string.Join("\n", File.ReadAllLines(fName));
                    Console.WriteLine(info + fileLines);
                }
                else
                    Console.WriteLine($"{fName} doesn't exist therefore cannot read!");
            }
            catch(Exception e)
            {
                Console.WriteLine($"{FSC.FS_ERR_HEADER} failed to read contents of {fName}...\n{e.Message}\n") ;
            }
            return fileLines;
        }

        internal static bool deleteManageableFile(string fName)
        {
            try
            {
                if (File.Exists(fName))
                {
                    Console.WriteLine($"Trying to delete {fName}...");
                    File.Delete(fName);
                    Console.WriteLine(FSC.DELETE_SUCCESS);
                }
                else
                    Console.WriteLine($"{fName} doesn't exist therefore cannot delete!");
            }
            catch(Exception e)
            {
                Console.WriteLine($"{FSC.FS_ERR_HEADER} failed to delete {fName}...");
                TU.logErrorMsg(e);
            }
            return !File.Exists(fName);
        }
    }
}