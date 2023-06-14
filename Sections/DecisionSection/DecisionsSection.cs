/*
* author: Sam Ford
* desc: Section for decision categories and making decisions from them
* date started: approx 4/23/2023
*/
// NOTES: DC == "Decision Category"

using System;
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

        // FIELDS
        private readonly Random rng;
        // private map of categories with matching prompts
        private Dictionary<string, string> categoryMap;
        // private map for category actions of form <Action Name, terminateLoop>
        private readonly OrderedDictionary categoryActions = new()
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

        public Dictionary<string, string> CategoryMap { get => categoryMap; }

        private Dictionary<string, DC> _dcMap; // TODO: 6/14/23

        // CONSTRUCTOR
        public DecisionsSection()
        {
            this.rng = new();
            this.categoryMap = new();
            this._dcMap = new();
            checkAndInitDCDir();
            addNewCategoriesToMap();
        }

        public void fullyUpdateStoredDCs()
        {
            checkAndInitDCDir();
            addNewCategoriesToMap();
            removeOldCategoriesFromMap();
        }
        
        /// <summary>
        /// initialize categories directory on startup if it doesn't exist already
        /// </summary>
        private void checkAndInitDCDir()
        {
            if(!Directory.Exists(DEFAULT_DC_DIRECTORY))
            {
                Console.WriteLine(NO_DC_DIR_MSG);
                Directory.CreateDirectory(DEFAULT_DC_DIRECTORY);
            }
        }


        // initialize the category map by reading files in Categories directory
        private void addNewCategoriesToMap()
        {
            List<string> existing = scanForDCs();
            foreach(string cat in existing.Where(c => !categoryMap.ContainsKey(c)))
            {
                string catPath = formatDCPath(cat);
                string[] catLines = File.ReadAllLines(catPath);
                string catDesc = catLines[DESC_LINE_IDX];
                List<string> catChoices = getChoicesFromDcFile(cat);

                DC dc = new(cat, catDesc, catChoices); // TODO: replace as map value 6/14/23
                Console.WriteLine(dc);

                categoryMap.Add(cat, catLines[DESC_LINE_IDX]);
                _dcMap.Add(cat, dc);
            }
        }

        // remove map categories no longer in Categories directory
        private void removeOldCategoriesFromMap()
        {
            List<string> existing = scanForDCs();
            foreach(string cat in categoryMap.Keys.Where(c => !existing.Contains(c)).ToList())
                categoryMap.Remove(cat);
        }

        public static string formatDCPath(string category)
        {
            return DEFAULT_DC_DIRECTORY + category + TU.TXT;
        }

        // used by FileSection.cs to print the exisiting categories.
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
                writeStartMenu();
                opt = MU.promptUser();
                processStartMenuInput(opt);
                fullyUpdateStoredDCs();
            }while(!wantsToExit(opt));
            return 0;
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
            return categoryMap.Count > 0;
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
            int totalCategories = categoryMap.Count;            
            for(int i = 0; i < totalCategories; i++)
            {
                KeyValuePair<string,string> category = categoryMap.ElementAt(i);
                Console.WriteLine($"{i+1}. {category.Key}: {category.Value}"); //TODO change to use to string
            }
        }

        private void printAddDC()
        {
            Console.WriteLine($"{categoryMap.Count + 1}. Add a whole new Decision Category");
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
            {
                Console.WriteLine($"Going to {getDCNameFromMenuChoice(opt)} menu...");
                enterDCActionsMenu(opt);
            }
            else if(MU.isChoiceMenuExit(opt))
                Console.WriteLine(MU.MENU_EXIT_MSG);
            else if(isChoiceAddNewDC(opt))
                createDC();
            else
                MU.writeInvalidMsg();
        }


        public string getDCNameFromMenuChoice(int opt)
        {
            return this.categoryMap.ElementAt(opt - 1).Key;
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
                    createDC();
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
            return(hasDCs()) && ((opt >= MU.MENU_START) && (opt <= categoryMap.Count));
        }

        private bool isChoiceAddNewDC(int opt)
        {
            return opt == (categoryMap.Count + 1);
        }

        // loop for choosing what to do with a selected decision category
        private void enterDCActionsMenu(int dcChoice)
        {
            string selected = getDCNameFromMenuChoice(dcChoice);
            int categoryOpt = MU.INVALID_OPT;
            bool doesTerminate = false;
            do
            {
                writeDCActionsMenu(selected);
                categoryOpt = MU.promptUser();
                doesTerminate = processDCActionsMenuInput(categoryOpt, selected);
            }while(!MU.isChoiceMenuExit(categoryOpt) && !doesTerminate);
        }

        private void writeDCActionsMenu(string category)
        {
            Console.WriteLine($"Here are the choices for the {category} decision category: ");
            List<string> actionNames = getDCActionKeys().ToList();
            TU.writeListAsNumberMenu(actionNames);
            MU.printExitChoice();
        }

        /// <summary>
        /// responds to what the user inputted in the category actions menu
        /// </summary>
        /// <param name="opt">- the valid/invalid option a user inputted </param>
        /// <param name="category">- the category we're currently in </param>
        /// <returns>whether the categoryActions loop should terminate</returns>
        private bool processDCActionsMenuInput(int opt, string category)
        {
            bool doesTerminate = false;
            if(isChoiceDCAction(opt))
                doesTerminate = processDCAction(opt, category);
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
        private bool createDC()
        {
            printExistingDCs();
            Console.WriteLine(CREATE_DC_MSG);
            return inputDC();
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
        private bool inputDC()
        {
            string categoryName = "";
            string categoryDesc = "";
            List<string> categoryChoices = new();
            try
            {
                categoryName = nameDC();
                categoryDesc = describeDC();
                categoryChoices = addChoicesToDC(categoryName);
                saveDCFile(categoryName, categoryDesc, categoryChoices);

                DC dc = new(categoryName, categoryDesc, categoryChoices); // TODO: 6/14/23
                Console.WriteLine(dc);
            }
            catch(Exception e)
            {
                Console.WriteLine($"{e} Failed to add new decision category. Saving any made progress");
                saveUnfinishedDC(categoryName, categoryDesc, categoryChoices);
                return false;
            }
            return true;
        }

        private void saveDCFile(string name, string desc, List<string> choices)
        {
            string categoryPath = formatDCPath(name);
            File.WriteAllText(categoryPath, name + DECISION_DELIMITER);
            File.AppendAllText(categoryPath, desc + DECISION_DELIMITER);
            File.AppendAllLines(categoryPath, choices);
        }

        private void saveUnfinishedDC(string name, string desc, List<string> choices)
        {
            string categoryPath = formatDCPath(FilesSection.DEFAULT_WIP_FILE);
            File.WriteAllText(categoryPath, name + DECISION_DELIMITER);
            File.AppendAllText(categoryPath, desc + DECISION_DELIMITER);
            File.AppendAllLines(categoryPath, choices);
        }

        private string nameDC()
        {
            string categoryName = "";
            do
            {
                Console.WriteLine(NAME_DC_MSG);
                categoryName = Console.ReadLine()!;
            }while(String.IsNullOrWhiteSpace(categoryName) || categoryMap.Keys.Contains(categoryName));

            return categoryName;
        }

        private string describeDC()
        {
            string categoryDesc = "";
            do
            {
                Console.WriteLine(DESCRIBE_DC_MSG);
                categoryDesc = Console.ReadLine()!;
            }while(!TU.isInputAcceptable(categoryDesc));
            return categoryDesc;
        }

        // TODO: convert to use DC obj
        private List<string> addChoicesToDC(string category)
        {
            List<string> acceptedChoices = checkDCExists(category) ? getChoicesFromDcFile(category) : new();
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

            Console.WriteLine($"Choices for {category}: {TU.prettyStringifyList(acceptedChoices)}\n");
            return acceptedChoices;
        }

        // TODO: to use dc objs
        private bool checkDCExists(string category)
        {
            //return  categoryMap.TryGetValue(category, out string dc);
            return File.Exists(formatDCPath(category));
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
                Console.WriteLine(outputMsg);
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
            return (opt >= MU.MENU_START) && (opt <= categoryActions.Count);
        }
 
        /// <summary>
        /// process actions after choosing an existing category
        /// </summary>
        /// <param name="actionNum">- the number the user inputted...</param>
        /// <param name="category">- the existing chosen category... </param>
        /// <returns>- whether the chosen action should terminate the category menu loop</returns>
        private bool processDCAction(int actionNum, string category)
        {
            switch(actionNum)
            {
                case (int) DcActionCodes.Decide:
                    decideForUser(category);
                    break;
                case (int) DcActionCodes.ReadChoices:
                    readExistingDC(category);
                    break;
                case (int) DcActionCodes.ReadDesc:
                    readDescDC(category);
                    break;
                case (int) DcActionCodes.AddChoices:
                    addChoicesToExistingDC(category);
                    break;
                case (int) DcActionCodes.RemoveChoices:
                    removeChoicesFromDC(category);
                    break;
                case (int) DcActionCodes.DeleteDc:
                    confirmDeleteDC(category);
                    break;
                default:
                    Console.WriteLine(DS_ERR_INTRO + "Invalid Category Action in process action. Something's up");
                    break;
            }
            return getDCActionTerminateVals()[actionNum-1];
        }

        // TODO: convert to use DC obj
        /// <summary>
        /// given a decision category, choose a random item from that decision category for the user to commit to
        /// </summary>
        /// <param name="category">- the category to pull a choice from </param>
        private void decideForUser(string category)
        {
            if(!doesDCHaveChoices(category))
            {
                Console.WriteLine(NO_CHOICES_MSG);
                return;
            }

            List<string> choices = getChoicesFromDcFile(category);
            int chosen = runRNG(choices);
            Console.WriteLine($"For {category}, we've decided upon: {choices[chosen]}");
        }

        // TODO: convert to use DC obj
        // print all options in a categories file line-by-line
        private void readExistingDC(string category)
        {
            if(!doesDCHaveChoices(category))
            {
                Console.WriteLine(NO_CHOICES_MSG);
                return;
            }
            
            Console.WriteLine(READ_DC_MSG);
            List<string> choices = getChoicesFromDcFile(category);
            foreach(string c in choices)
                Console.WriteLine(c);
        }

        // TODO: convert to use DC obj
        private void readDescDC(string category)
        {
            if(checkDCExists(category))
                Console.WriteLine($"Description for {category}: {getDescDC(category)}");
        }

        // TODO: convert to use DC obj
        private string getDescDC(string category)
        {
            string catDesc = categoryMap[category];
            return (catDesc != null) ? catDesc : DNE_DC_MSG;
        }

        // TODO: convert to use DC obj
        private void addChoicesToExistingDC(string category)
        {
            List<string> addedChoices = addChoicesToDC(category);
            saveDCFile(category, getDescDC(category), addedChoices);
        }

        // TODO: convert to use DC obj
        private void removeChoicesFromDC(string category)
        {
            if (!doesDCHaveChoices(category))
            {
                Console.WriteLine(NO_CHOICES_MSG);
                return;
            }

            List<string> remainingChoices = removeChoicesFromDCLoop(category);
            saveDCFile(category, getDescDC(category), remainingChoices);
        }

        private void confirmDeleteDC(string category)
        {
            int opt = MU.INVALID_OPT;
            do
            {
                Console.WriteLine($"Please confirm you want to delete the {category} decision category:");
                MU.writeBinaryMenu();
                opt = MU.promptUser();
                processDeleteDCOpt(opt, category);
            } while (!MU.isBinaryChoice(opt));
        }

        private void processDeleteDCOpt(int opt, string category)
        {
            
            switch(opt)
            {
                case MU.YES_CODE:
                    deleteDC(category);
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

        // TODO: convert to use DC obj
        private void deleteDC(string category)
        {
            if(checkDCExists(category))
            {
                try
                {
                    string catPath = formatDCPath(category);
                    File.Delete(catPath);
                    Console.WriteLine($"Successfully deleted {category} and its file");
                }
                catch
                {
                    Console.WriteLine($"Failed to delete {category} file...");
                }
            }
            else
                Console.WriteLine($"{category} does not exist and therefore can't be deleted...");
        }        

        // TODO: convert to use DC obj
        private bool doesDCHaveChoices(string category)
        {
            return checkDCExists(category) && !TU.isStringListEmpty(getChoicesFromDcFile(category));
        }

        /// <summary>
        /// loop for removing choices until no choices remain
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        private List<string> removeChoicesFromDCLoop(string category)
        {
            List<string> remainingChoices = getChoicesFromDcFile(category);
            int opt = MU.INVALID_OPT;
            bool isExit = false;
            while(!TU.isStringListEmpty(remainingChoices) && !isExit)
            {
                writeRemoveChoicesMenu(remainingChoices);
                opt = MU.promptUser();
                isExit = MU.isChoiceMenuExit(opt);
                string removed = processRemoveDecisionChoice(opt, remainingChoices);
                if(!isExit) printRemoveChoicesLoopMsg(removed, remainingChoices, category);
                Console.WriteLine();
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

        private void printRemoveChoicesLoopMsg(string removed, List<string> remainingChoices, string category)
        {
            if (TU.isStringListEmpty(remainingChoices))
            {
                Console.WriteLine($"All choices removed from {category} category");
                return;
            }
            else if (removed == "")
                Console.WriteLine(REMOVE_CHOICE_REJECT_MSG);
            else
                Console.WriteLine($"Successfully removed {removed} option!");

            Console.WriteLine($"{category} choices remaining: {TU.prettyStringifyList(remainingChoices)}");
        }

        private int runRNG(List<string> choices)
        {
            int endIdx = choices.Count - 1;
            return rng.Next(0, endIdx);
        }        

        private string[] getDCActionKeys()
        {
            string[] actionKeys = new string[this.categoryActions.Count];
            this.categoryActions.Keys.CopyTo(actionKeys, 0);
            return actionKeys;
        }

        private bool[] getDCActionTerminateVals()
        {
            bool[] terminateVals = new bool[this.categoryActions.Count];
            this.categoryActions.Values.CopyTo(terminateVals, 0);
            return terminateVals;
        }

        private List<string> getChoicesFromDcFile(string category)
        {
            return File.ReadAllLines(formatDCPath(category)).Skip(INFO_LEN).ToList();
        }

        // TODO: convert to initialize DC obj
        private List<string> getChoicesFromDcObj(string category)
        {
           if(getDcObj(category,  out DC catObj))
                return catObj.CatChoices;
            return new();
        }

        private bool getDcObj(string category, out DC dc)
        {
            return _dcMap.TryGetValue(category, out dc!);
        }
    }
}