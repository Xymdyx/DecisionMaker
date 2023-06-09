using System;
using System.Text.RegularExpressions;
using System.Collections.Specialized;


namespace DecisionMaker
{
    // NOTES: DC == "Decision Category"
    public class DecisionsSection : IDecisionMakerSection
    {
        // STRING CONSTANTS
        public const string DEFAULT_DC_DIRECTORY = @".\Decisions\Categories\";
        private const string DECISION_DELIMITER = "\n";
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

        public Dictionary<string, string> CategoryMap { get => categoryMap; }

        // CONSTRUCTOR
        public DecisionsSection()
        {
            this.rng = new();
            this.categoryMap = new();
            checkAndInitDCDir();
            addNewCategoriesToMap();
        }

        public void fullyUpdateStoredDCs()
        {
            checkAndInitDCDir();
            addNewCategoriesToMap();
            removeOldCategoriesFromMap();
        }

        // initialize the category map by reading files in Categories directory
        private void addNewCategoriesToMap()
        {
            List<string> existing = scanForDCs();
            foreach(string cat in existing.Where(c => !categoryMap.ContainsKey(c)))
            {
                string[] catLines = File.ReadAllLines(formatDCPath(cat));
                categoryMap.Add(cat, catLines[DESC_LINE_IDX]);
            }
        }

        // remove map categories no longer in Categories directory
        private void removeOldCategoriesFromMap()
        {
            List<string> existing = scanForDCs();
            foreach(string cat in categoryMap.Keys.Where(c => !existing.Contains(c)).ToList())
                categoryMap.Remove(cat);
        }

        public string formatDCPath(string category)
        {
            return DEFAULT_DC_DIRECTORY + category + TextUtils.TXT;
        }

        // used by FileSection.cs to print the exisiting categories.
        private List<string> scanForDCs()
        {
            try
            {
                List<string> files = Directory.GetFiles(DEFAULT_DC_DIRECTORY, $"*{TextUtils.TXT}").ToList();
                int i = 0;
                foreach (string path in files.ToList())
                {
                    int catLen = path.Length - DEFAULT_DC_DIRECTORY.Length - TextUtils.TXT.Length;
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

        /// <summary>
        /// the entry loop for the Decision Section
        /// </summary>
        /// <returns></returns>
        public int doMenuLoop()
        {
            Console.WriteLine(DECISIONS_WELCOME_MSG);
            int opt = MenuUtils.INVALID_OPT;
            do
            {
                writeStartMenu();
                opt = MenuUtils.promptUser();
                processStartMenuInput(opt);
                fullyUpdateStoredDCs();
            }while(!wantsToExit(opt));
            return 0;
        }

        private bool wantsToExit(int opt)
        {
            return MenuUtils.isChoiceMenuExit(opt) || (MenuUtils.isChoiceNo(opt) && !hasDCs());
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
            MenuUtils.printExitChoice();
        }

        public void printSavedDCs()
        {
            int totalCategories = categoryMap.Count;            
            for(int i = 0; i < totalCategories; i++)
            {
                KeyValuePair<string,string> category = categoryMap.ElementAt(i);
                Console.WriteLine($"{i+1}. {category.Key}: {category.Value}");
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
            MenuUtils.writeBinaryMenu();
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
            else if(MenuUtils.isChoiceMenuExit(opt))
                Console.WriteLine(MenuUtils.MENU_EXIT_MSG);
            else if(isChoiceAddNewDC(opt))
                createDC();
            else
                MenuUtils.writeInvalidMsg();
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
                case MenuUtils.YES_CODE:
                    createDC();
                    break;
                case MenuUtils.NO_CODE:
                    Console.WriteLine(MenuUtils.MENU_EXIT_MSG);
                    break;
                case MenuUtils.EXIT_CODE:
                    break;
                default:
                    MenuUtils.writeInvalidMsg();
                    break;
            }
        }

        // determine if the input is for an existing category
        public bool isChoiceInChoiceRange(int opt)
        {
            return(hasDCs()) && ((opt >= MenuUtils.MENU_START) && (opt <= categoryMap.Count));
        }

        private bool isChoiceAddNewDC(int opt)
        {
            return opt == (categoryMap.Count + 1);
        }

        // loop for choosing what to do with a selected decision category
        private void enterDCActionsMenu(int dcChoice)
        {
            string selected = getDCNameFromMenuChoice(dcChoice);
            int categoryOpt = MenuUtils.INVALID_OPT;
            bool doesTerminate = false;
            do
            {
                writeDCActionsMenu(selected);
                categoryOpt = MenuUtils.promptUser();
                doesTerminate = processDCActionsMenuInput(categoryOpt, selected);
            }while(!MenuUtils.isChoiceMenuExit(categoryOpt) && !doesTerminate);
        }

        private void writeDCActionsMenu(string category)
        {
            Console.WriteLine($"Here are the choices for the {category} decision category: ");
            List<string> actionNames = getDCActionKeys().ToList();
            TextUtils.writeListAsNumberMenu(actionNames);
            MenuUtils.printExitChoice();
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
            else if (MenuUtils.isChoiceMenuExit(opt))
            {
                Console.WriteLine(MenuUtils.MENU_EXIT_MSG);
                doesTerminate = true;
            }
            else
                MenuUtils.writeInvalidMsg();

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
            }while(!TextUtils.isInputAcceptable(categoryDesc));
            return categoryDesc;
        }

        private List<string> addChoicesToDC(string category)
        {
            List<string> acceptedChoices = checkDCExists(category) ? getChoicesDC(category) : new();
            string choiceInput = "";
            bool stopWanted = false;
            do
            {
                printAddChoiceLoopInstructions(acceptedChoices);
                choiceInput = Console.ReadLine()!;
                stopWanted = TextUtils.isInputStopCommand(choiceInput);
                bool accepted = false;
                if(!stopWanted)
                    accepted = tryAcceptNewDCChoice(choiceInput, acceptedChoices); // choose to accept or reject into choiceInputs

                printAddChoiceLoopMsg(accepted, choiceInput, acceptedChoices);
            }while(TextUtils.isStringListEmpty(acceptedChoices) || !stopWanted);

            Console.WriteLine($"Choices for {category}: {TextUtils.prettyStringifyList(acceptedChoices)}\n");
            return acceptedChoices;
        }

        private bool checkDCExists(string category)
        {
            return File.Exists(formatDCPath(category));
        }

        private void printAddChoiceLoopInstructions(List<string> acceptedChoices)
        {
            string introEnd = (TextUtils.isStringListEmpty(acceptedChoices)) ? ":" : $" {getAddChoicesStopMsg()}:";
            Console.WriteLine(ADD_CHOICE_INTRO_MSG + introEnd);
        }

        private string getAddChoicesStopMsg()
        {
            string stops = TextUtils.prettyStringifyList(TextUtils.stopWords.ToList());
            return $"({STOP_INFO_MSG}, type any positive number or any of the following in lowercase: {stops})";
        }

        private void printAddChoiceLoopMsg(bool wasAccepted, string candidate, List<string> acceptedChoices)
        {
            string outputMsg = "";
            if(wasAccepted)
                outputMsg = $"{candidate} accepted!";
            else if (isItemAlreadyAccepted(candidate, acceptedChoices))
                outputMsg = $"{candidate} was already accepted";
            else if (!TextUtils.isInputAcceptable(candidate))
                outputMsg = ADD_CHOICE_REJECT_MSG;

            if(outputMsg != "")
                Console.WriteLine(outputMsg);
        }

        bool tryAcceptNewDCChoice(string candidate, List<string> acceptedChoices)
        {
            if(TextUtils.isInputAcceptable(candidate) && !isItemAlreadyAccepted(candidate, acceptedChoices))
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
            return (opt >= MenuUtils.MENU_START) && (opt <= categoryActions.Count);
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
                case 1:
                    decideForUser(category);
                    break;
                case 2:
                    readExistingDC(category);
                    break;
                case 3:
                    readDescDC(category);
                    break;
                case 4:
                    addChoicesToExistingDC(category);
                    break;
                case 5:
                    removeChoicesFromDC(category);
                    break;
                case 6:
                    deleteDC(category);
                    break;
                default:
                    Console.WriteLine(DS_ERR_INTRO + "Invalid Category Action in process action. Something's up");
                    break;
            }
            return getDCActionTerminateVals()[actionNum-1];
        }

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

            List<string> choices = getChoicesDC(category);
            int chosen = runRNG(choices);
            Console.WriteLine($"For {category}, we've decided upon: {choices[chosen]}");
        }

        // print all options in a categories file line-by-line
        private void readExistingDC(string category)
        {
            if(!doesDCHaveChoices(category))
            {
                Console.WriteLine(NO_CHOICES_MSG);
                return;
            }
            
            Console.WriteLine(READ_DC_MSG);
            List<string> choices = getChoicesDC(category);
            foreach(string c in choices)
                Console.WriteLine(c);
        }

        private void readDescDC(string category)
        {
            if(checkDCExists(category))
                Console.WriteLine($"Description for {category}: {getDescDC(category)}");
        }

        private string getDescDC(string category)
        {
            string catDesc = categoryMap[category];
            return (catDesc != null) ? catDesc : DNE_DC_MSG;
        }

        private void addChoicesToExistingDC(string category)
        {
            List<string> addedChoices = addChoicesToDC(category);
            saveDCFile(category, getDescDC(category), addedChoices);
        }

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

        private bool doesDCHaveChoices(string category)
        {
            return checkDCExists(category) && !TextUtils.isStringListEmpty(getChoicesDC(category));
        }

        /// <summary>
        /// loop for removing choices until no choices remain
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        private List<string> removeChoicesFromDCLoop(string category)
        {
            List<string> remainingChoices = getChoicesDC(category);
            int opt = MenuUtils.INVALID_OPT;
            bool isExit = false;
            while(!TextUtils.isStringListEmpty(remainingChoices) && !isExit)
            {
                writeRemoveChoicesMenu(remainingChoices);
                opt = MenuUtils.promptUser();
                isExit = MenuUtils.isChoiceMenuExit(opt);
                string removed = processRemoveDecisionChoice(opt, remainingChoices);
                if(!isExit) printRemoveChoicesLoopMsg(removed, remainingChoices, category);
                Console.WriteLine();
            }
            return remainingChoices;
        }

         private void writeRemoveChoicesMenu(List<string> remaining)
         {
            Console.WriteLine(REMOVE_CHOICES_MENU_MSG);
            TextUtils.writeListAsNumberMenu(remaining);
            MenuUtils.printExitChoice();
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
            if((MenuUtils.MENU_START <= choiceOpt) && (choiceOpt <= remainingChoices.Count))
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
            if (TextUtils.isStringListEmpty(remainingChoices))
            {
                Console.WriteLine($"All choices removed from {category} category");
                return;
            }
            else if (removed == "")
                Console.WriteLine(REMOVE_CHOICE_REJECT_MSG);
            else
                Console.WriteLine($"Successfully removed {removed} option!");

            Console.WriteLine($"{category} choices remaining: {TextUtils.prettyStringifyList(remainingChoices)}");
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

        private List<string> getChoicesDC(string category)
        {
            return File.ReadAllLines(formatDCPath(category)).Skip(INFO_LEN).ToList();
        }
    }
}