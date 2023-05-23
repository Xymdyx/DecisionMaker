using System;
using System.Text.RegularExpressions;
using System.Collections.Specialized;

namespace DecisionMaker
{
    // NOTES: DC == "Decision Category"
    public class DecisionsSection : IDecisionMakerSection
    {
        // STRING CONSTANTS
        private const string DEFAULT_DC_DIRECTORY = @".\Decisions\Categories\";
        private const string DEFAULT_WIP_FILE = "wipcat";
        private const string TXT = ".txt";
        private const string DECISION_DELIMITER = "\n"; // DC files delimited by newlines
        private const string NO_DC_DIR_MSG = "No decisions directory detected in the desired location...Creating";
        private const string HAS_DCS_MSG = "What would you like us to choose today?";
        private const string NO_DCS_MSG = "Hmm. There appear to be no decision categories for us to choose from.";
        private const string INVALID_CHOICE_MSG = "What you inputted was not a valid Choice, please try again.";
        private const string MENU_EXIT_MSG = "Exiting to main menu";
        private const string STOP_INFO_MSG = "(type any positive number, \"stop\", or \"exit\" to stop adding)";
        private const string DNE_DC_MSG = "This decision category doesn't exist";
        private const string NO_CHOICES_MSG = "No choices to choose from! Please add some...";
        private const string BINARY_CHOICE_MSG ="1. Yes\n2. No\n";
        private const string DECISIONS_WELCOME_MSG = "Welcome to the Decisions menu. This is where the magic happens!";

        // INT CONSTANTS
        private const int INVALID_OPT = Int32.MinValue; //TODO: UTIL CLASS - 5/23/23
        private const int DELETE_ALL_CHOICES_CODE = -1;
        private const int EXIT_CODE = 0; //TODO: UTIL CLASS - 5/23/23
        private const int YES_CODE = 1; //TODO: UTIL CLASS - 5/23/23
        private const int NO_CODE = 2; //TODO: UTIL CLASS - 5/23/23
        private const int MAX_STRING_LEN = 360; //TODO: UTIL CLASS - 5/23/23
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

        // CONSTRUCTOR
        public DecisionsSection()
        {
            this.rng = new();
            this.categoryMap = new();
            checkAndInitDCDir();
            addNewCategoriesToMap();
        }

        private void fullyUpdateStoredDCs()
        {
            addNewCategoriesToMap();
            removeOldCategoriesFromMap();
        }

        // initialize the category map by reading files in the categories directory
        private void addNewCategoriesToMap()
        {
            List<string> existing = scanForDCs();
            Console.WriteLine(prettyStringifyList(existing));
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

        private string formatDCPath(string category)
        {
            return DEFAULT_DC_DIRECTORY + category + TXT;
        }

        private List<string> scanForDCs()
        {
            try
            {
                List<string> files = Directory.GetFiles(DEFAULT_DC_DIRECTORY, $"*{TXT}").ToList();
                int i = 0;
                foreach (string path in files.ToList())
                {
                    int catLen = path.Length - DEFAULT_DC_DIRECTORY.Length - TXT.Length;
                    files[i] = path.Substring(DEFAULT_DC_DIRECTORY.Length, catLen);
                    ++i;
                }
                return files;
            }
            catch(Exception e)
            {
                Console.WriteLine($"DecisionSect.cs: Error scanning for categories... {e}");
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
            int opt = INVALID_OPT;
            do
            {
                writeStartMenu();
                opt = promptUser();
                processStartMenuInput(opt);
                fullyUpdateStoredDCs();
            }while(!wantsToExit(opt));
            return 0;
        }

        private bool wantsToExit(int opt)
        {
            return isChoiceMenuExit(opt) || (isChoiceNo(opt) && !hasDCs());
        }

        private void writeStartMenu()
        {
            if(hasDCs())
            {
                writeDCsMenu();
                return;
            }
            Console.WriteLine(NO_DCS_MSG);
            add1stDC();
            return;
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
            printExitChoice();
        }

        private void printSavedDCs()
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

        private void printExitChoice()
        {
            Console.WriteLine($"{EXIT_CODE}. Exit");
        }

        private void add1stDC()
        {
            Console.WriteLine("Let's add a decision category shall we? Please confirm that you would like to do so.");
            Console.WriteLine(BINARY_CHOICE_MSG);
        }

        /// <summary>
        /// main user choice parsing method where they must choose from listed integers
        /// </summary>
        /// <returns>
        /// a processed integer... INVALID_OPT for invalid choice
        /// </returns>
        private int promptUser()
        {
            Console.WriteLine("Please choose a valid number: ");
            string input = Console.ReadLine()!;
            int opt = convertInputToInt(input);
            return opt;        
        }

        private int convertInputToInt(string input)
        {
            int opt = INVALID_OPT;
            try
            {
                opt = System.Int32.Parse(input);
            }
            catch(Exception e) 
            {
                Console.Error.WriteLine($"DecisionSect.cs: Cannot convert input to integer: {e}");
            }
            return opt;
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
                int catIdx = opt-1;
                Console.WriteLine($"Going to {categoryMap.Keys.ElementAt(catIdx)} menu...");
                enterDCActionsMenu(catIdx);
            }
            else if(isChoiceMenuExit(opt))
                Console.WriteLine(MENU_EXIT_MSG);
            else if(isChoiceAddNewDC(opt))
                createDC();
            else
                Console.WriteLine(INVALID_CHOICE_MSG);
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
                case YES_CODE:
                    createDC();
                    break;
                case NO_CODE:
                    Console.WriteLine(MENU_EXIT_MSG);
                    break;
                default:
                    Console.WriteLine(INVALID_CHOICE_MSG);
                    break;
            }
        }

        // determine if the input is for an existing category
        private bool isChoiceInChoiceRange(int opt)
        {
            return(hasDCs()) && ((opt >= 1) && (opt <= categoryMap.Count));
        }

        // determines if the inputted number is for exiting a menu.
        private bool isChoiceMenuExit(int opt)
        {
            return opt == EXIT_CODE;
        }

        private bool isChoiceNo(int opt)
        {
            return opt == NO_CODE;
        }

        private bool isChoiceAddNewDC(int opt)
        {
            return opt == (categoryMap.Count + 1);
        }

        // loop for choosing what to do with a selected decision category
        private void enterDCActionsMenu(int categoryIdx)
        {
            string selected = categoryMap.ElementAt(categoryIdx).Key;
            int categoryOpt = INVALID_OPT;
            bool doesTerminate = false;
            do
            {
                writeDCActionsMenu(selected);
                categoryOpt = promptUser();
                doesTerminate = processDCActionsMenuInput(categoryOpt, selected);
            }while(!isChoiceMenuExit(categoryOpt) && !doesTerminate);
        }

        private void writeDCActionsMenu(string category)
        {
            Console.WriteLine($"Here are the choices for the {category} decision category: ");
            List<string> actionNames = getDCActionKeys().ToList();
            writeListAsNumberMenu(actionNames);
            printExitChoice();
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
            else if (isChoiceMenuExit(opt))
            {
                Console.WriteLine(MENU_EXIT_MSG);
                doesTerminate = true;
            }
            else
                Console.WriteLine(INVALID_CHOICE_MSG);

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
            Console.WriteLine("Please help us create a new decision category...");
            return inputDC();
        }

        // Read all saved categories.
        private void printExistingDCs()
        {
            if(hasDCs())
            {
                Console.WriteLine("Here are the existing decision categories:");
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
            string categoryPath = formatDCPath(DEFAULT_WIP_FILE);
            File.WriteAllText(categoryPath, name + DECISION_DELIMITER);
            File.AppendAllText(categoryPath, desc + DECISION_DELIMITER);
            File.AppendAllLines(categoryPath, choices);
        }

        private string nameDC()
        {
            string categoryName = "";
            do
            {
                Console.WriteLine("Please name this new decision category (no duplicates allowed)");
                categoryName = Console.ReadLine()!;
            }while(String.IsNullOrWhiteSpace(categoryName) || categoryMap.Keys.Contains(categoryName));

            return categoryName;
        }

        private string describeDC()
        {
            string categoryDesc = "";
            do
            {
                Console.WriteLine("Please give a description for this category: ");
                categoryDesc = Console.ReadLine()!;
            }while(!isInputAcceptable(categoryDesc));
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
                stopWanted = isInputStopCommand(choiceInput);
                bool accepted = false;
                if(!stopWanted)
                    accepted = tryAcceptNewDCChoice(choiceInput, acceptedChoices); // choose to accept or reject into choiceInputs

                printAddChoiceLoopMsg(accepted, choiceInput, acceptedChoices);
            }while(isStringListEmpty(acceptedChoices) || !stopWanted);

            Console.WriteLine($"Choices for {category}: {prettyStringifyList(acceptedChoices)}\n");
            return acceptedChoices;
        }

        private bool checkDCExists(string category)
        {
            return File.Exists(formatDCPath(category));
        }

        private void printAddChoiceLoopInstructions(List<string> acceptedChoices)
        {
            const string introStart = "Please provide an alphanumeric string for a choice that hasn't already been added";
            string introEnd = (isStringListEmpty(acceptedChoices)) ? ":" : $" {STOP_INFO_MSG}:";
            Console.WriteLine(introStart + introEnd);
        }

        private void printAddChoiceLoopMsg(bool wasAccepted, string candidate, List<string> acceptedChoices)
        {
            string outputMsg = "";
            if(wasAccepted)
                outputMsg = $"{candidate} accepted!";
            else if (isItemAlreadyAccepted(candidate, acceptedChoices))
                outputMsg = $"{candidate} was already accepted";
            else if (!isInputAcceptable(candidate))
                outputMsg = "What you inputted was simply unaceeptable";

            if(outputMsg != "")
                Console.WriteLine(outputMsg);
        }

        bool tryAcceptNewDCChoice(string candidate, List<string> acceptedChoices)
        {
            if(isInputAcceptable(candidate) && !isItemAlreadyAccepted(candidate, acceptedChoices))
            {
                acceptedChoices.Add(candidate);
                return true;
            }
            return false;
        }

        bool isInputAcceptable(string input)
        {
            return !String.IsNullOrWhiteSpace(input) && input.Length <= MAX_STRING_LEN;
        }

        private bool isItemAlreadyAccepted(string candidate, List<string> accepted)
        {
            return accepted.Exists(choice => choice.ToLower() == candidate.ToLower());
        }

        private bool isChoiceDCAction(int opt)
        {
            return (opt >= 1) && (opt <= categoryActions.Count);
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
                    Console.WriteLine("DecisionSect.cs: Invalid Category Action in process action. Something's up");
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
            
            Console.WriteLine("Feel free to decide on your own if this list inspires you: ");
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
            return checkDCExists(category) && !isStringListEmpty(getChoicesDC(category));
        }

        /// <summary>
        /// loop for removing choices until no choices remain
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        private List<string> removeChoicesFromDCLoop(string category)
        {
            List<string> remainingChoices = getChoicesDC(category);
            int opt = INVALID_OPT;
            bool isExit = false;
            while(!isStringListEmpty(remainingChoices) && !isExit)
            {
                writeRemoveChoicesMenu(remainingChoices);
                opt = promptUser();
                isExit = isChoiceMenuExit(opt);
                string removed = processRemoveDecisionChoice(opt, remainingChoices);
                if(!isExit) printRemoveChoicesLoopMsg(removed, remainingChoices, category);
            }

            return remainingChoices;
        }

         private void writeRemoveChoicesMenu(List<string> remaining)
         {
            Console.WriteLine("Please select the number of the item you'd like to remove (can remove until nothing remains)...");
            writeListAsNumberMenu(remaining);
            printExitChoice();
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
            if((1 <= choiceOpt) && (choiceOpt <= remainingChoices.Count))
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
            if (isStringListEmpty(remainingChoices))
            {
                Console.WriteLine($"All choices removed from {category} category");
                return;
            }
            else if (removed == "")
                Console.WriteLine("What you inputted was invalid. Therefore, nothing was removed...");
            else
                Console.WriteLine($"Successfully removed {removed} option!");

            Console.WriteLine($"{category} choices remaining: {prettyStringifyList(remainingChoices)}");
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

        // TODO: move to util class
        private bool isInputStopCommand(string input)
        {
            string lCaseInput = input.ToLower();
            return lCaseInput == "stop" || lCaseInput == "exit" || isNumeric(lCaseInput);
        }

        // TODO: move to util class
        private bool isNumeric(string input)
        {
            Regex numericOnly = new(@"^[0-9]+$");
            return numericOnly.IsMatch(input);
        }

        // TODO: move to util class
        private bool isAlpha(string input){
            Regex alphaNumeric = new(@"^[a-zA-Z\s,]*$");
            return alphaNumeric.IsMatch(input);
        }

        //TODO: MOVE TO UTIL CLASS - 5/17/23
        private string prettyStringifyList(List<string> items)
        {
            return string.Join(", ", items);
        }

        //TODO: MOVE TO UTIL CLASS - 5/21/23
        /// <summary>
        /// prints a list in the form:
        /// 1. [string]
        /// 2. [string]
        /// ...
        /// n. [string]
        /// </summary>
        /// <param name="list"> - the list to print</param>
         private void writeListAsNumberMenu(List<string> list)
         {
            for(int i = 0; i < list.Count; i++)
                Console.WriteLine($"{i+1}. {list[i]}");
         }

        // TODO: MOVE TO UTIL CLASS - 5/23/23
         private bool isStringListEmpty(List<string> strings)
         {
            return strings.Count == 0;
        }
    }
}