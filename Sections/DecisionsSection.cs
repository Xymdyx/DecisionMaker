using System;
using System.Text.RegularExpressions;
using System.Collections.Specialized;

namespace DecisionMaker
{
    // NOTES: DC == "Decision Category"
    public class DecisionsSection : IDecisionMakerSection
    {
        // STRING CONSTANTS
        private const string DEFAULT_CATEGORY_DIRECTORY = @".\Decisions\Categories\";
        private const string DEFAULT_WIP_FILE = "wipcat";
        private const string TXT = ".txt";
        // category files with items delimited by newlines
        private const string DECISION_DELIMITER = "\n";
        private const string NO_CATEGORIES_DIR_MSG = "No decisions directory detected in the desired location...Creating";
        private const string NO_CATEGORIES_MSG = "Hmm. There appear to be no decision categories for us to choose from.";
        private const string INVALID_CHOICE_MSG = "What you inputted was not a valid Choice, please try again.";
        private const string MENU_EXIT_MSG = "Exiting to main menu";
        private const string STOP_INFO_MSG = "(type any positive number, \"stop\", or \"exit\" to stop adding)";
        private const string DNE_CAT_MSG = "This category doesn't exist";

        // INT CONSTANTS
        private const int INVALID_OPT = Int32.MinValue;
        private const int ADD_CAT_CODE = -1;
        private const int DELETE_ALL_CHOICES_CODE = -1;
        private const int EXIT_CODE = 0;
        private const int YES_CODE = 1;
        private const int NO_CODE = 2;
        private const int MAX_STRING_LEN = 360;

        private const int DESC_LINE_IDX = 1;
        private const int INFO_LEN = 2;

        // for rng
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
        public DecisionsSection()
        {
            this.rng = new();
            this.categoryMap = new();
            checkAndInitCategoryDir();
            addNewCategoriesToMap();
        }

        private void fullyUpdateCategories()
        {
            addNewCategoriesToMap();
            removeOldCategoriesFromMap();
        }

        /// initialize the category map by reading files in the categories directory
        private void addNewCategoriesToMap()
        {
            List<string> existing = scanForCategories();
            Console.WriteLine(prettyStringifyList(existing));
            foreach(string cat in existing.Where(c => !categoryMap.ContainsKey(c)))
            {
                string[] catLines = File.ReadAllLines(formatCategoryPath(cat));
                categoryMap.Add(cat, catLines[DESC_LINE_IDX]);
            }
        }

        private string formatCategoryPath(string category)
        {
            return DEFAULT_CATEGORY_DIRECTORY + category + TXT;
        }

        /// remove map categories no longer in Categories directory
        private void removeOldCategoriesFromMap()
        {
            List<string> existing = scanForCategories();
            foreach(string cat in categoryMap.Keys.Where(c => !existing.Contains(c)).ToList())
                categoryMap.Remove(cat);
        }

        private List<string> scanForCategories()
        {
            try
            {
                List<string> files = Directory.GetFiles(DEFAULT_CATEGORY_DIRECTORY, $"*{TXT}").ToList();
                int i = 0;
                foreach (string path in files.ToList())
                {
                    int catLen = path.Length - DEFAULT_CATEGORY_DIRECTORY.Length - TXT.Length;
                    files[i] = path.Substring(DEFAULT_CATEGORY_DIRECTORY.Length, catLen);
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
        private void checkAndInitCategoryDir()
        {
            if(!Directory.Exists(DEFAULT_CATEGORY_DIRECTORY))
            {
                Console.WriteLine(NO_CATEGORIES_DIR_MSG);
                Directory.CreateDirectory(DEFAULT_CATEGORY_DIRECTORY);
            }
        }

        /// <summary>
        /// the entry loop for the Decision Section
        /// </summary>
        /// <returns></returns>
        public int doMenuLoop()
        {
            Console.WriteLine("Welcome to the Decisions menu. This is where the magic happens!");
            int opt = INVALID_OPT;
            do
            {
                writeMenu();
                opt = promptUser();
                processMenuInput(opt);
            }while(!isEitherExit(opt));
            return 0;
        }

        private bool isEitherExit(int opt)
        {
            return isChoiceMainExit(opt) || (isChoiceNo(opt) && !hasDecisionCategories());
        }

        private void writeMenu()
        {
            if(hasDecisionCategories()) //FIXME: uncomment after initial test
            {
                Console.WriteLine("What would you like us to choose today?");
                printSavedCategories();
                printExitChoice();                
                return;
            }
            Console.WriteLine(NO_CATEGORIES_MSG);
            add1stDC();
            return;
        }

        private bool hasDecisionCategories()
        {
            return categoryMap.Count > 0;
        }

        private void printSavedCategories()
        {
            int totalCategories = categoryMap.Count;            
            for(int i = 0; i < totalCategories; i++)
            {
                KeyValuePair<string,string> category = categoryMap.ElementAt(i);
                Console.WriteLine($" {i+1}. {category.Key}: {category.Value}");
            }
        }

        private void add1stDC()
        {
            Console.WriteLine("Let's add a decision category shall we? Please confirm that you would like to do so.");
            Console.WriteLine(
                "1. Yes\n" +
                "2. No\n"
            );
        }

        private void printExitChoice()
        {
            Console.WriteLine($"{EXIT_CODE}. Exit");
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
        private void processMenuInput(int opt)
        {
            if(hasDecisionCategories())
                processCategoriesMenuInput(opt);
            else
                process1stCategoryConfirmation(opt);
        }

        // processes existing categories menu input
        private void processCategoriesMenuInput(int opt)
        {
            if(isChoiceInChoiceRange(opt))
            {
                int catIdx = opt-1;
                Console.WriteLine($"Going to {categoryMap.Keys.ElementAt(catIdx)} menu...");
                enterCategoryActionsMenu(catIdx);
            }
            else if(isChoiceMainExit(opt))
                Console.WriteLine(MENU_EXIT_MSG);
            else if(isChoiceAddNewCategory(opt))
                createDC();
            else
                Console.WriteLine(INVALID_CHOICE_MSG);
        }

        // processes no categories menu input, 
        // aka adding the first category to the categories directory
        private void process1stCategoryConfirmation(int opt)
        {
            switch(opt){
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

        // determine if the input is for a pre-existing category
        private bool isChoiceInChoiceRange(int opt)
        {
            return(hasDecisionCategories()) && ((opt >= 1) && (opt <= categoryMap.Count));
        }

        // determines if the inputted number is for exiting e main menu.
        private bool isChoiceMainExit(int opt)
        {
            return opt == EXIT_CODE;
        }

        // determines if inputted number is NO
        private bool isChoiceNo(int opt)
        {
            return opt == NO_CODE;
        }

        // determines if the inputted number is for adding a new category
        private bool isChoiceAddNewCategory(int opt)
        {
            return opt == ADD_CAT_CODE;
        }

        // loop for choosing what to do with a decision category
        private void enterCategoryActionsMenu(int categoryIdx)
        {
            string selected = categoryMap.ElementAt(categoryIdx).Key;
            int categoryOpt = INVALID_OPT;
            bool doesTerminate = false;
            do
            {
                writeCategoryActionsMenu(selected);
                categoryOpt = promptUser();
                doesTerminate = processCategoryActionsMenuInput(categoryOpt, selected);
            }while(!isChoiceMainExit(categoryOpt) || doesTerminate);
        }

        private void writeCategoryActionsMenu(string category)
        {
            Console.WriteLine($"Here are the choices for the {category} decision category: ");
            List<string> actionNames = getDecisionActionKeys().ToList();
            writeListAsNumberMenu(actionNames);
            printExitChoice();
        }

        /// <summary>
        /// responds to what the user inputted in the category actions menu
        /// </summary>
        /// <param name="opt">- the valid/invalid option a user inputted </param>
        /// <param name="category">- the category we're currently in </param>
        /// <returns>whether the categoryActions loop should terminate</returns>
        private bool processCategoryActionsMenuInput(int opt, string category)
        {
            bool doesTerminate = false;
            if(isChoiceCategoryAction(opt))
                doesTerminate = processCategoryAction(opt, category);
            else if (isChoiceMainExit(opt))
            {
                Console.WriteLine(MENU_EXIT_MSG);
                doesTerminate = true;
            }
            else
                Console.WriteLine(INVALID_CHOICE_MSG);

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
            printExistingDecisionCategories();
            Console.WriteLine("Please help us create a new decision category...");
            return inputDC();
        }

        // Read all saved categories.
        private void printExistingDecisionCategories()
        {
            if(hasDecisionCategories())
            {
                Console.WriteLine("Here are the existing decision categories:");
                printSavedCategories();
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
                saveWIPCategoryFile(categoryName, categoryDesc, categoryChoices);
                return false;
            }
            return true;
        }

        private void saveDCFile(string name, string desc, List<string> choices)
        {
            string categoryPath = formatCategoryPath(name);
            File.WriteAllText(categoryPath, name + "\n");
            File.AppendAllText(categoryPath, desc + "\n");
            File.AppendAllLines(categoryPath, choices);
        }

        private void saveWIPCategoryFile(string name, string desc, List<string> choices)
        {
            string categoryPath = formatCategoryPath(DEFAULT_WIP_FILE);
            File.WriteAllText(categoryPath, name + "\n");
            File.AppendAllText(categoryPath, desc + "\n");
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
            List<string> acceptedChoices = checkCategoryExists(category) ? getCategoryChoices(category) : new();
            string choiceInput = "";
            bool stopWanted = false;
            do
            {
                printAddChoiceLoopInstructions(acceptedChoices);
                choiceInput = Console.ReadLine()!;
                stopWanted = isInputStopCommand(choiceInput);
                bool accepted = false;
                if(!stopWanted)
                    accepted = tryAcceptNewCategoryChoice(choiceInput, acceptedChoices); // choose to accept or reject into choiceInputs

                printAddChoiceLoopMsg(accepted, choiceInput, acceptedChoices);
            }while(isStringListEmpty(acceptedChoices) || !stopWanted);

            Console.WriteLine($"Choices approved for {category}: {prettyStringifyList(acceptedChoices)}\n");
            return acceptedChoices;
        }

        private bool checkCategoryExists(string category)
        {
            return File.Exists(formatCategoryPath(category));
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
            else if (isItemInChoices(candidate, acceptedChoices))
                outputMsg = $"{candidate} was already accepted";
            else if (!isInputAcceptable(candidate))
                outputMsg = "What you inputted was simply unaceeptable";

            if(outputMsg != "")
                Console.WriteLine(outputMsg);
        }

        bool tryAcceptNewCategoryChoice(string candidate, List<string> acceptedChoices)
        {
            if(isInputAcceptable(candidate) && !isItemInChoices(candidate, acceptedChoices))
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

        private bool isItemInChoices(string candidate, List<string> accepted)
        {
            return accepted.Exists(choice => choice.ToLower() == candidate.ToLower());
        }

        private bool isChoiceCategoryAction(int opt)
        {
            return (opt >= 1) && (opt <= categoryActions.Count);
        }
 
        /// <summary>
        /// process actions after choosing an existing category
        /// </summary>
        /// <param name="actionNum">- the number the user inputted...</param>
        /// <param name="category">- the existing chosen category... </param>
        /// <returns>- whether the chosen action should terminate the category menu loop</returns>
        private bool processCategoryAction(int actionNum, string category)
        {
            switch(actionNum)
            {
                case 1:
                    decideForUser(category);
                    break;
                case 2:
                    readExistingCategory(category);
                    break;
                case 3:
                    Console.WriteLine($"Description for {category}: {getCategoryDesc(category)}");
                    break;
                case 4:
                    addChoicesToExistingDC(category);
                    break;
                case 5:
                    removeChoicesFromDC(category);
                    break;
                case 6:
                    // delete category file
                    break;
                default:
                    Console.WriteLine("DecisionSect.cs: Invalid Category Action in process action. Something's up");
                    break;
            }
            return getDecisionActionTerminateVals()[actionNum - 1];
        }

        /// <summary>
        /// given a decision category, choose a random item from that decision category for the user to commit to
        /// </summary>
        /// <param name="category">- the category to pull a choice from </param>
        private void decideForUser(string category)
        {
            List<string> choices = getCategoryChoices(category);
            int chosen = runRNG(choices);
            Console.WriteLine($"For {category}, we've decided upon: {choices[chosen]}");
        }

        // print all options in a categories file line-by-line
        private void readExistingCategory(string category)
        {
            Console.WriteLine("Feel free to decide on your own if this list inspires you: ");
            List<string> choices = getCategoryChoices(category);
            foreach(string c in choices)
                Console.WriteLine(c);
        }

        private string getCategoryDesc(string category)
        {
            string catDesc = categoryMap[category];
            return (catDesc != null) ? catDesc : DNE_CAT_MSG;
        }

        private void addChoicesToExistingDC(string category)
        {
            List<string> addedChoices = addChoicesToDC(category);
            saveDCFile(category, getCategoryDesc(category), addedChoices);
        }

        private void removeChoicesFromDC(string category)
        {
            if (!checkCategoryExists(category))
                return; 
            
            List<string> remainingChoices = removeChoicesFromDCLoop(category);
            saveDCFile(category, getCategoryDesc(category), remainingChoices);
        }

        /// <summary>
        /// loop for removing choices until no choices remain
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        private List<string> removeChoicesFromDCLoop(string category)
        {
            List<string> remainingChoices = getCategoryChoices(category);
            int opt = INVALID_OPT;
            bool isExit = false;
            do
            {
                writeRemoveChoicesMenu(remainingChoices);
                opt = promptUser();
                isExit = isChoiceMainExit(opt);
                string removed = processRemoveDecisionChoice(opt, remainingChoices);
                if(!isExit) printRemoveChoicesLoopMsg(removed, remainingChoices, category);
            }while(!isStringListEmpty(remainingChoices) && !isExit);

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

        private string[] getDecisionActionKeys()
        {
            string[] actionKeys = new string[this.categoryActions.Count];
            this.categoryActions.Keys.CopyTo(actionKeys, 0);
            return actionKeys;
        }

        private bool[] getDecisionActionTerminateVals()
        {
            bool[] terminateVals = new bool[this.categoryActions.Count];
            this.categoryActions.Values.CopyTo(terminateVals, 0);
            return terminateVals;
        }

        private List<string> getCategoryChoices(string category)
        {
            return File.ReadAllLines(formatCategoryPath(category)).Skip(INFO_LEN).ToList();
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

         private bool isStringListEmpty(List<string> strings)
         {
            return strings.Count == 0;
        }
    }
}