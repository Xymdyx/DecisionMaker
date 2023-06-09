/*
* author: Sam Ford
* desc: Section for deleting files made by the program
* date started: approx 5/30/2023
*/
namespace DecisionMaker
{
    using ps = DecisionMaker.ProfileSection;
    public class FilesSection:IDecisionMakerSection
    {
        private enum FileTypeCodes
        {
            DCFiles = 1,
            ProfileFiles,
            WipFile
        }

        private enum FileActionCodes 
        {
            DeleteAll = -1,
            ViewFile = 1,
            DeleteFile
        }

        private enum ProfileActionCodes
        {
            GREETING = 1,
            DEPARTING,
            DISPLAYNAME
        }

        public const string DEFAULT_FILES_DIR = ".\\FileManagement\\";
        public const string DEFAULT_WIP_FILE = DEFAULT_FILES_DIR + "wipcat";
        private const string FS_ERR_HEADER = "FilesSection.cs: ";
        private readonly string[] fileTypes = { "Decision Categories", "Custom Profile", "WIP File" };
        private readonly string[] manageFileActions = {"View contents", "Delete file"};
        private readonly string[] manageProfileActions = {"Greeting", "Exiting", "Display Name"};
        private DecisionsSection decSect;
        private ProfileSection profSect;

        public FilesSection(DecisionsSection ds, ProfileSection ps)
        {
            this.decSect = ds;
            this.profSect = ps;
        }

        public int doMenuLoop()
        {
            int opt = MenuUtils.INVALID_OPT;
            do
            {
                writeMenu();
                opt = MenuUtils.promptUser();
                processMenuInput(opt);
            } while (!MenuUtils.isChoiceMenuExit(opt));
            return opt;
        }

        private void writeMenu()
        {
            TextUtils.writeListAsNumberMenu(fileTypes.ToList());
            MenuUtils.printExitChoice();
        }

        private void processMenuInput(int opt)
        {
            switch(opt)
            {
                case (int) FileTypeCodes.DCFiles:
                    doDCsLoop();
                    break;
                case (int) FileTypeCodes.ProfileFiles:
                    doProfileLoop();
                    break;
                case (int) FileTypeCodes.WipFile:
                    doFileMenuLoop(DEFAULT_WIP_FILE);
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
                opt = MenuUtils.promptUser();
                processDCsMenuInput(opt);
            } while (!MenuUtils.isChoiceMenuExit(opt));
            return opt;
        }

        private void writeManageDCsMenu()
        {
            decSect.printSavedDCs();
            MenuUtils.printExitChoice();
            Console.WriteLine($"{(int)FileActionCodes.DeleteAll}. Delete all decision categories");
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
            string dcName = decSect.getDCNameFromMenuChoice(opt);
            string dcPath = decSect.formatDCPath(dcName);
            int dcOpt = doFileMenuLoop(dcPath);

            if(dcOpt == (int) FileActionCodes.DeleteFile)
                this.decSect.fullyUpdateStoredDCs();
        }

        private bool isOptDeleteAll(int opt)
        {
            return opt == (int)FileActionCodes.DeleteAll;
        }

        private void deleteAllDCs()
        {
            deleteDirAndContents(DecisionsSection.DEFAULT_DC_DIRECTORY);
            this.decSect.fullyUpdateStoredDCs();
        }

        private void deleteDirAndContents(string dir)
        {
            try
            {
                Directory.Delete(dir, true);
            }
            catch(Exception e)
            {
                Console.WriteLine($"{FS_ERR_HEADER}: Failed to delete entire {dir} directory...\n{e}");
            }
        }

        private int doProfileLoop()
        {
            int opt = MenuUtils.INVALID_OPT;
            do
            {
                writeManageProfileMenu();
                opt = MenuUtils.promptUser();
                processProfileMenuInput(opt);
                profSect.scanForProfileUpdates();
            } while (!MenuUtils.isChoiceMenuExit(opt));
            return opt;
        }

        private void writeManageProfileMenu()
        {
            TextUtils.writeListAsNumberMenu(this.manageProfileActions.ToList());
            MenuUtils.printExitChoice();
            Console.WriteLine($"{(int)FileActionCodes.DeleteAll}. Delete entire profile");
        }

        private void processProfileMenuInput(int opt)
        {
            switch(opt)
            {
                case (int) ProfileActionCodes.GREETING:
                    doFileMenuLoop(ps.PROFILE_GREETING_PATH);
                    break;
                case (int) ProfileActionCodes.DEPARTING:
                    doFileMenuLoop(ps.PROFILE_EXITING_PATH);
                    break;
                case (int) ProfileActionCodes.DISPLAYNAME:
                    doFileMenuLoop(ps.PROFILE_DISPLAY_NAME_PATH);
                    break;
                case (int)FileActionCodes.DeleteAll:
                    deleteDirAndContents(ps.PROFILE_DEFAULT_DIR);
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
                opt = MenuUtils.promptUser();
                processFileActions(opt, fName);
            } while (continueFileMenuLoop(opt));
            return opt;
        }

        private bool continueFileMenuLoop(int opt)
        {
            return (!MenuUtils.isChoiceMenuExit(opt)) && (opt != (int)FileActionCodes.DeleteFile);
        }

        private void writeFileMenu()
        {
            TextUtils.writeListAsNumberMenu(this.manageFileActions.ToList());
            MenuUtils.printExitChoice();
        }  

        private void processFileActions(int opt, string fName)
        {
            switch(opt)
            {
                case (int) FileActionCodes.ViewFile:
                    viewFileContents(fName);
                    break;
                case (int) FileActionCodes.DeleteFile:
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

        private void viewFileContents(string fName)
        {
            try
            {
                if (File.Exists(fName))
                {
                    string info = $"Contents of {fName}:\n";
                    string fileLines = string.Join("\n", File.ReadAllLines(fName));
                    Console.WriteLine(info + fileLines);
                }
                else
                    Console.WriteLine($"{fName} doesn't exist therefore cannot read!");
            }
            catch(Exception e)
            {
                Console.WriteLine($"{FS_ERR_HEADER} failed to read contents of {fName}... {e}") ;
            }
        }

        private void deleteManageableFile(string fName)
        {
            try
            {
                if (File.Exists(fName))
                {
                    Console.WriteLine($"Trying to delete {fName}...");
                    File.Delete(fName);
                    Console.WriteLine("Delete successful!");
                }
                else
                    Console.WriteLine($"{fName} doesn't exist therefore cannot delete!");
            }
            catch(Exception e)
            {
                Console.WriteLine($"{FS_ERR_HEADER} failed to delete {fName}... {e}");
            }
        }

        private void decideForUser(List<string> choices){}
        private int runRNG(){return 0;}
    }
}