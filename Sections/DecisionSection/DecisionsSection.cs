/*
* author: Sam Ford
* desc: Section for decision categories and making decisions from them
* note that DC == "Decision Category"
* date started: approx 4/23/2023
*/

using System.Collections.Specialized;

using MU = DecisionMaker.MenuUtils;
using TU = DecisionMaker.TextUtils;
using DC = DecisionMaker.DecisionCategory;
namespace DecisionMaker
{
    public class DecisionsSection:IDecisionMakerSection
    {
        // STRING CONSTANTS
        public const string DEFAULT_DC_DIRECTORY = @".\Decisions\Categories\";
        public const string DECISION_DELIMITER = "\n";
        private const string NO_DC_DIR_MSG = "No decisions directory detected in the desired location...Creating";
        private const string HAS_DCS_MSG = "What would you like us to choose today?";
        private const string NO_DCS_MSG = "Hmm. There appear to be no decision categories for us to choose from.";
        private const string ADD_1ST_DC_CONFIRM_MSG = "Let's add a decision category shall we? Please confirm that you would like to do so.";
        private const string STOP_INFO_MSG = "to stop adding";
        private const string DNE_DC_MSG = "This decision category doesn't exist";
        private const string NO_CHOICES_MSG = "No choices to choose from! Please add some...";
        private const string DECISIONS_WELCOME_MSG = "Welcome to the Decisions menu. This is where the magic happens!";
        private const string ADD_CHOICE_INTRO_MSG = "Please provide an alphanumeric string for a choice that hasn't already been added";
        private const string REMOVE_CHOICES_MENU_MSG = "Please select the number of the item you'd like to remove (can remove until nothing remains)...";
        private const string CREATE_DC_MSG = "Please help us create a new decision category...";
        private const string SHOW_DCS_MSG = "Here are the existing decision categories:";
        private const string NAME_DC_MSG = "Please name this new decision category (no duplicates allowed)";
        private const string DESCRIBE_DC_MSG = "Please give a description for this category:";
        private const string READ_DC_MSG = "Feel free to decide on your own if this list inspires you:";
        private const string ADD_CHOICE_REJECT_MSG = "What you inputted was simply unaceeptable";
        private const string REMOVE_CHOICE_REJECT_MSG = "What you inputted was invalid. Therefore, nothing was removed...";
        private const string DS_ERR_INTRO = "DecisionSect.cs: ";

        // INT CONSTANTS
        private const int DELETE_ALL_CHOICES_CODE = -1;
        private const int DESC_LINE_IDX = 1;
        private const int INFO_LEN = 2;

        // private map for category actions of form <Action Name, terminateLoop>
        private readonly OrderedDictionary dcActions = new()
        {
            {"Make a decision", true},
            {"View decisions", false},
            {"Read category description", false},
            {"Add choices", false},
            {"Remove choices", false},
            {"Delete entire category", true}
        };

        private enum DcActionCodes
        {
            Decide = 1,
            ReadChoices,
            ReadDesc,
            AddChoices,
            RemoveChoices,
            DeleteDc
        }        

        // FIELDS
        private readonly Random rng;
        // private map of categories with matching objs
        private Dictionary<string, DC> _dcMap;

        public Dictionary<string, DC> DcMap{ get => _dcMap; }

        // CONSTRUCTOR
        public DecisionsSection()
        {
            this.rng = new();
            this._dcMap = new();
            this._dcMap = new();
            checkAndInitDCDir();
            addNewCategoriesToMap();
        }

        private void fullyUpdateStoredDCs()
        {
            syncDcMapToDcDir();
            addNewCategoriesToMap();
        }

        public void syncDcMapToDcDir()
        {
            if(checkAndInitDCDir())
                removeOldCategoriesFromMap();
        }

        /// <summary>
        /// initialize categories directory on startup if it doesn't exist already
        /// </summary>
        public static bool checkAndInitDCDir()
        {
            if (!Directory.Exists(DEFAULT_DC_DIRECTORY))
            {
                try
                {
                    Console.WriteLine(NO_DC_DIR_MSG);
                    Directory.CreateDirectory(DEFAULT_DC_DIRECTORY);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{DS_ERR_INTRO} failed to initialize {DEFAULT_DC_DIRECTORY} directory...\n{e}");
                }
            }
            return Directory.Exists(DEFAULT_DC_DIRECTORY);
        }

        // initialize the category map by reading files in Categories directory
        private void addNewCategoriesToMap()
        {
            List<string> existing = scanForDCs();
            foreach(string cat in existing.Where(c => !_dcMap.ContainsKey(c)))
            {
                string catPath = formatDCPath(cat);
                string[] catLines = File.ReadAllLines(catPath);
                string catDesc = catLines[DESC_LINE_IDX];
                List<string> catChoices = getChoicesFromDcFile(cat);

                DC dc = new(cat, catDesc, catChoices);
                _dcMap.TryAdd(cat, dc);
            }
        }

        // remove map categories no longer in Categories directory
        private void removeOldCategoriesFromMap()
        {
            List<string> existing = scanForDCs();
            foreach(string cat in _dcMap.Keys.Where(c => !existing.Contains(c)).ToList())
                _dcMap.Remove(cat);
        }

        public static string formatDCPath(string dc)
        {
            return DEFAULT_DC_DIRECTORY + dc + TU.TXT;
        }

        /// <summary>
        /// scans the \Decisions\Categories directory for txts and returns them as strings
        /// </summary>
        /// <returns>a list of the decision category filenames </returns>
        private List<string> scanForDCs()
        {
            try
            {
                List<string> files = Directory.GetFiles(DEFAULT_DC_DIRECTORY, $"*{TU.TXT}").ToList();
                int i = 0;
                foreach (string path in files.ToList())
                {
                    int catLen = path.Length - DEFAULT_DC_DIRECTORY.Length - TU.TXT.Length;
                    files[i] = path.Substring(DEFAULT_DC_DIRECTORY.Length, catLen);
                    ++i;
                }
                return files;
            }
            catch(Exception e)
            {
                Console.WriteLine(DS_ERR_INTRO + $"Error scanning for categories... {e}");
            }
            return new();
        }

        private List<string> getChoicesFromDcFile(string dc)
        {
            return File.ReadAllLines(formatDCPath(dc)).Skip(INFO_LEN).ToList();
        }

        /// <summary>
        /// the entry loop for the Decision Section
        /// </summary>
        /// <returns></returns>
        public int doMenuLoop()
        {
            Console.WriteLine(DECISIONS_WELCOME_MSG);
            int opt = MU.INVALID_OPT;
            do
            {
                fullyUpdateStoredDCs();
                writeStartMenu();
                opt = MU.promptUser();
                processStartMenuInput(opt);
            }while(!wantsToExit(opt));
            return opt;
        }

        private bool wantsToExit(int opt)
        {
            return MU.isChoiceMenuExit(opt) || (MU.isChoiceNo(opt) && !hasDCs());
        }

        private void writeStartMenu()
        {
            if(hasDCs())
                writeDCsMenu();
            else
                add1stDC();
        }

        private bool hasDCs()
        {
            return _dcMap.Count > 0;
        }

        private void writeDCsMenu()
        {
            Console.WriteLine(HAS_DCS_MSG);
            printSavedDCs();
            printAddDC();
            MU.printExitChoice();
        }

        public void printSavedDCs()
        {
            int totalCategories = _dcMap.Count;
            for(int i = 0; i < totalCategories; i++)
            {
                DC dc = _dcMap.ElementAt(i).Value;
                Console.WriteLine($"{i+1}. {dc}");
            }
        }

        private void printAddDC()
        {
            Console.WriteLine($"{_dcMap.Count + 1}. Add a whole new Decision Category");
        }

        private void add1stDC()
        {
            Console.WriteLine(NO_DCS_MSG);
            Console.WriteLine(ADD_1ST_DC_CONFIRM_MSG);
            MU.writeBinaryMenu();
        }

        // this is for processing the entry point menu
        private void processStartMenuInput(int opt)
        {
            if(hasDCs())
                processDCsMenuInput(opt);
            else
                process1stDCConfirmation(opt);
        }

        // processes existing categories menu input
        private void processDCsMenuInput(int opt)
        {
            if(isChoiceInChoiceRange(opt))
                enterDCActionsMenu(opt);
            else if(MU.isChoiceMenuExit(opt))
                Console.WriteLine(MU.MENU_EXIT_MSG);
            else if(isChoiceAddNewDC(opt))
                createPermanentDC();
            else
                MU.writeInvalidMsg();
        }

        public string getDCNameFromMenuChoice(int opt)
        {
            return this._dcMap.ElementAt(opt - 1).Key;
        }

        public DC getDCFromMenuChoice(int opt)
        {
            return _dcMap.ElementAt(opt - 1).Value;
        }

        /// <summary>
        /// processes no categories menu input,
        /// aka adding the first category to the categories directory
        /// </summary>
        /// <param name="opt">- the processed option from processStartMenuInput </param>
        private void process1stDCConfirmation(int opt)
        {
            switch(opt)
            {
                case MU.YES_CODE:
                    createPermanentDC();
                    break;
                case MU.NO_CODE:
                    Console.WriteLine(MU.MENU_EXIT_MSG);
                    break;
                case MU.EXIT_CODE:
                    break;
                default:
                    MU.writeInvalidMsg();
                    break;
            }
        }

        // determine if the input is for an existing category
        public bool isChoiceInChoiceRange(int opt)
        {
            return(hasDCs()) && ((opt >= MU.MENU_START) && (opt <= _dcMap.Count));
        }

        private bool isChoiceAddNewDC(int opt)
        {
            return opt == (_dcMap.Count + 1);
        }

        // loop for choosing what to do with a selected decision category
        private void enterDCActionsMenu(int dcChoice)
        {
            string selected = getDCNameFromMenuChoice(dcChoice);
            DC selectedDc = getDCFromMenuChoice(dcChoice);
            int dcOpt = MU.INVALID_OPT;
            bool doesTerminate = false;
            do
            {
                writeDCActionsMenu(selectedDc.CatName);
                dcOpt = MU.promptUser();
                doesTerminate = processDCActionsMenuInput(dcOpt, selected, selectedDc);
            }while(!MU.isChoiceMenuExit(dcOpt) && !doesTerminate);
        }

        private void writeDCActionsMenu(string dc)
        {
            Console.WriteLine($"Here are the choices for the {dc} decision category: ");
            List<string> actionNames = getDCActionKeys().ToList();
            TU.writeListAsNumberMenu(actionNames);
            MU.printExitChoice();
        }

        /// <summary>
        /// responds to what the user inputted in the category actions menu
        /// </summary>
        /// <param name="opt">- the valid/invalid option a user inputted </param>
        /// <param name="dc">- the category we're currently in </param>
        /// <returns>whether the categoryActions loop should terminate</returns>
        private bool processDCActionsMenuInput(int opt, string dc, DC selectedDC)
        {
            bool doesTerminate = false;
            if(isChoiceDCAction(opt))
                doesTerminate = processDCAction(opt, dc, selectedDC);
            else if (MU.isChoiceMenuExit(opt))
            {
                Console.WriteLine(MU.MENU_EXIT_MSG);
                doesTerminate = true;
            }
            else
                MU.writeInvalidMsg();

            Console.WriteLine();
            return doesTerminate;
        }

        /// <summary>
        /// Attempt to create a new decision category.
        /// </summary>
        /// <returns>
        /// bool- true if a user's new decision category was successfully configured
        /// false otherwise
        /// </returns>
        private bool createPermanentDC()
        {
            printExistingDCs();
            Console.WriteLine(CREATE_DC_MSG);
            DC newDc = inputDC();
            return newDc.IsValidDc() && newDc.saveFile() && _dcMap.TryAdd(newDc.CatName, newDc);
        }

        // Read all saved categories.
        private void printExistingDCs()
        {
            if(hasDCs())
            {
                Console.WriteLine(SHOW_DCS_MSG);
                printSavedDCs();
            }
        }

        // accept user input for a new decision category step-by-step
        private DC inputDC()
        {
            string dcName = "";
            string dcDesc = "";
            List<string> dcChoices = new();
            try
            {
                dcName = nameDC();
                dcDesc = describeDC();
                DC dc = new(dcName, dcDesc);
                dc.CatChoices = addChoicesToDC(dc);
                return dc;
            }
            catch(Exception e)
            {
                Console.WriteLine($"{e} Failed to add new decision category. Saving any made progress");
                saveUnfinishedDC(dcName, dcDesc, dcChoices);
            }
            return DC.EmptyDc;
        }

        private void saveUnfinishedDC(string name, string desc, List<string> choices)
        {
            string dcPath = formatDCPath(FilesSection.DEFAULT_WIP_FILE);
            File.WriteAllText(dcPath, name + DECISION_DELIMITER);
            File.AppendAllText(dcPath, desc + DECISION_DELIMITER);
            File.AppendAllLines(dcPath, choices);
        }

        private string nameDC()
        {
            string dcName = "";
            do
            {
                Console.WriteLine(NAME_DC_MSG);
                dcName = Console.ReadLine()!;
            }while(String.IsNullOrWhiteSpace(dcName) || _dcMap.Keys.Contains(dcName.Trim()));

            return dcName;
        }

        private string describeDC()
        {
            string dcDesc = "";
            do
            {
                Console.WriteLine(DESCRIBE_DC_MSG);
                dcDesc = Console.ReadLine()!;
            }while(!TU.isInputAcceptable(dcDesc));
            return dcDesc;
        }

        private List<string> addChoicesToDC(DC selectedDc)
        {
            List<string> acceptedChoices = selectedDc.CatChoices;
            string choiceInput = "";
            bool stopWanted = false;
            do
            {
                printAddChoiceLoopInstructions(acceptedChoices);
                choiceInput = Console.ReadLine()!;
                stopWanted = TU.isInputStopCommand(choiceInput);
                bool accepted = false;
                if(!stopWanted)
                    accepted = tryAcceptNewDCChoice(choiceInput, acceptedChoices); // choose to accept or reject into choiceInputs

                printAddChoiceLoopMsg(accepted, choiceInput, acceptedChoices);
            }while(TU.isStringListEmpty(acceptedChoices) || !stopWanted);

            Console.WriteLine($"Choices for {selectedDc.CatName}: {TU.prettyStringifyList(acceptedChoices)}\n");
            return acceptedChoices;
        }

        private void printAddChoiceLoopInstructions(List<string> acceptedChoices)
        {
            string introEnd = (TU.isStringListEmpty(acceptedChoices)) ? ":" : $" {getAddChoicesStopMsg()}:";
            Console.WriteLine(ADD_CHOICE_INTRO_MSG + introEnd);
        }

        private string getAddChoicesStopMsg()
        {
            string stops = TU.prettyStringifyList(TU.stopWords.ToList());
            return $"({STOP_INFO_MSG}, type any positive number or any of the following in lowercase: {stops})";
        }

        private void printAddChoiceLoopMsg(bool wasAccepted, string candidate, List<string> acceptedChoices)
        {
            string outputMsg = "";
            if(wasAccepted)
                outputMsg = $"{candidate} accepted!";
            else if (isItemAlreadyAccepted(candidate, acceptedChoices))
                outputMsg = $"{candidate} was already accepted";
            else if (!TU.isInputAcceptable(candidate))
                outputMsg = ADD_CHOICE_REJECT_MSG;

            if(outputMsg != "")
                Console.WriteLine(outputMsg + "\n");
        }

        bool tryAcceptNewDCChoice(string candidate, List<string> acceptedChoices)
        {
            if(TU.isInputAcceptable(candidate) && !isItemAlreadyAccepted(candidate, acceptedChoices))
            {
                acceptedChoices.Add(candidate);
                return true;
            }
            return false;
        }

        private bool isItemAlreadyAccepted(string candidate, List<string> accepted)
        {
            return accepted.Exists(choice => choice.ToLower() == candidate.ToLower());
        }

        private bool isChoiceDCAction(int opt)
        {
            return (opt >= MU.MENU_START) && (opt <= dcActions.Count);
        }

        /// <summary>
        /// process actions after choosing an existing category
        /// </summary>
        /// <param name="actionNum">- the number the user inputted...</param>
        /// <param name="dc">- the existing chosen category... </param>
        /// <returns>- whether the chosen action should terminate the category menu loop</returns>
        private bool processDCAction(int actionNum, string dc, DC selectedDc)
        {
            bool confirmHalt = true;
            switch(actionNum)
            {
                case (int) DcActionCodes.Decide:
                    confirmHalt = decideForUser(selectedDc);
                    break;
                case (int) DcActionCodes.ReadChoices:
                    confirmHalt = readExistingDC(selectedDc);
                    break;
                case (int) DcActionCodes.ReadDesc:
                    confirmHalt = readDescDC(selectedDc);
                    break;
                case (int) DcActionCodes.AddChoices:
                    confirmHalt = addChoicesToExistingDC(selectedDc);
                    break;
                case (int) DcActionCodes.RemoveChoices:
                    confirmHalt = removeChoicesFromDC(selectedDc);
                    break;
                case (int) DcActionCodes.DeleteDc:
                    confirmHalt = confirmDeleteDC(selectedDc);
                    break;
                default:
                    Console.WriteLine(DS_ERR_INTRO + "Invalid Category Action in process action. Something's up");
                    break;
            }
            return getDCActionTerminateVals()[actionNum-1] && confirmHalt;
        }

        /// <summary>
        /// given a decision category, choose a random item from that decision category for the user to commit to
        /// </summary>
        /// <param name="dc">- the category to pull a choice from </param>
        private bool decideForUser(DC dc)
        {
            if(!dc.hasChoices())
            {
                Console.WriteLine(NO_CHOICES_MSG);
                return false;
            }

            int chosen = runRNG(dc.CatChoices);
            Console.WriteLine($"For {dc.CatName}, we've decided upon: {dc.CatChoices[chosen]}");
            return true;
        }

        // print all options in a categories file line-by-line
        private bool readExistingDC(DC dc)
        {
            if(!dc.hasChoices())
            {
                Console.WriteLine(NO_CHOICES_MSG);
                return false;
            }

            Console.WriteLine(READ_DC_MSG);
            Console.WriteLine(dc.stringifyChoices());
            return false;
        }

        private bool readDescDC(DC dc)
        {
            bool exists = dc.checkFileExists();
            if(exists)
                Console.WriteLine($"Description for {dc.CatName}: {dc.CatDesc}");
            return !exists;
        }

        private bool addChoicesToExistingDC(DC dc)
        {
            List<string> added = addChoicesToDC(dc);
            dc.CatChoices = added;
            return dc.saveFile();
        }

        private bool removeChoicesFromDC(DC dc)
        {
            if (!dc.hasChoices())
            {
                Console.WriteLine(NO_CHOICES_MSG);
                return false;
            }

            List<string> remaining = removeChoicesFromDCLoop(dc);
            dc.CatChoices = remaining;
            return dc.saveFile();
        }

        /// <summary>
        /// loop for removing choices until no choices remain
        /// </summary>
        /// <param name="selectedDc"></param>
        /// <returns></returns>
        private List<string> removeChoicesFromDCLoop(DC selectedDc)
        {
            List<string> remainingChoices = selectedDc.CatChoices;
            int opt = MU.INVALID_OPT;
            bool isExit = false;
            while(!TU.isStringListEmpty(remainingChoices) && !isExit)
            {
                writeRemoveChoicesMenu(remainingChoices);
                opt = MU.promptUser();
                isExit = MU.isChoiceMenuExit(opt);
                string removed = processRemoveDecisionChoice(opt, remainingChoices);
                if(!isExit) printRemoveChoicesLoopMsg(removed, remainingChoices, selectedDc.CatName);
            }
            return remainingChoices;
        }

         private void writeRemoveChoicesMenu(List<string> remaining)
         {
            Console.WriteLine(REMOVE_CHOICES_MENU_MSG);
            TU.writeListAsNumberMenu(remaining);
            MU.printExitChoice();
            printDeleteAllChoices();
        }

        private string processRemoveDecisionChoice(int opt, List<string> remainingChoices)
        {
            string removedEl = "";
            if (opt == DELETE_ALL_CHOICES_CODE)
                remainingChoices.Clear();
            else
               removedEl = tryRemoveChoice(opt, remainingChoices);

            return removedEl;
        }

        private void printDeleteAllChoices()
        {
            Console.WriteLine($"{DELETE_ALL_CHOICES_CODE}. To remove all choices");
        }

        private string tryRemoveChoice(int choiceOpt, List<string> remainingChoices)
        {
            string removed = "";
            if((MU.MENU_START <= choiceOpt) && (choiceOpt <= remainingChoices.Count))
            {
                try
                {
                    int choiceIdx = choiceOpt - 1;
                    removed = remainingChoices.ElementAt(choiceIdx);
                    remainingChoices.RemoveAt(choiceIdx);
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                    Console.WriteLine($"DecisionSect: Failed to remove decision choice from {remainingChoices} given option {choiceOpt}");
                }
            }
            return removed;
        }

        private void printRemoveChoicesLoopMsg(string removed, List<string> remainingChoices, string dc)
        {
            if (TU.isStringListEmpty(remainingChoices))
            {
                Console.WriteLine($"All choices removed from {dc} category\n");
                return;
            }
            else if (removed == "")
                Console.WriteLine(REMOVE_CHOICE_REJECT_MSG);
            else
                Console.WriteLine($"Successfully removed {removed} option!");

            Console.WriteLine($"{dc} choices remaining: {TU.prettyStringifyList(remainingChoices)}\n");
        }

        private bool confirmDeleteDC(DC dc)
        {
            int opt = MU.INVALID_OPT;
            bool terminateConfirm = false;
            do
            {
                Console.WriteLine($"Please confirm you want to delete the {dc} decision category:");
                MU.writeBinaryMenu();
                opt = MU.promptUser();
                terminateConfirm = processDeleteDCOpt(opt, dc);
            } while (!MU.isBinaryChoice(opt));
            return terminateConfirm;
        }

        private bool processDeleteDCOpt(int opt, DC dc)
        {
            switch(opt)
            {
                case MU.YES_CODE:
                    return dc.deleteFile() && _dcMap.Remove(dc.CatName);
                case MU.NO_CODE:
                    Console.WriteLine(MU.MENU_EXIT_MSG);
                    return false;
                case MU.EXIT_CODE:
                    return false;
                default:
                    MU.writeInvalidMsg();
                    return false;
            }
        }

        private int runRNG(List<string> choices)
        {
            int endIdx = choices.Count - 1;
            return rng.Next(0, endIdx);
        }

        private string[] getDCActionKeys()
        {
            string[] actionKeys = new string[this.dcActions.Count];
            this.dcActions.Keys.CopyTo(actionKeys, 0);
            return actionKeys;
        }

        private bool[] getDCActionTerminateVals()
        {
            bool[] terminateVals = new bool[this.dcActions.Count];
            this.dcActions.Values.CopyTo(terminateVals, 0);
            return terminateVals;
        }
    }
}