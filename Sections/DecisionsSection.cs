using System;
using System.Text.RegularExpressions;

namespace DecisionMaker
{
    public class DecisionsSection : IDecisionMakerSection
    {
        // STRING CONSTANTS
        private const string DEFAULT_CATEGORY_DIRECTORY = ".\\Decisions\\Categories\\";
        // category files with items delimited by newlines
        private const string DECISION_DELIMITER = "\n";
        private const string NO_CATEGORIES_DIR_MSG = "No decisions directory detected in the desired location...Creating";
        private const string NO_CATEGORIES_MSG = "Hmm. There appear to be no decision categories for us to choose from.";
        private const string INVALID_CHOICE_MSG = "What you inputted was not a valid option, please try again.";
        private const string MENU_EXIT_MSG = "Exiting to main menu";
        private const string STOP_INFO_MSG = "type any positive number, \"stop\", or \"exit\" to stop adding)";

        // INT CONSTANTS
        private const int INVALID_OPT = Int32.MinValue;
        private const int ADD_CAT_CODE = -1;
        private const int EXIT_CODE = 0;
        private const int YES_CODE = 1;
        private const int NO_CODE = 2;
        private const int MAX_STRING_LEN = 360;

        // need file reader/writer
        private FileStream categoryStream;
        // private map of categories with matching prompts
        private Dictionary<string, string> categoryMap;
        private readonly string[] categoryMenuChoices = {"Make a decision", "View decisions",
                                                        "Read category description", "Add choices",
                                                         "Remove choices", "Delete entire category"};
        public DecisionsSection()
        {
            this.categoryStream = null!;
            this.categoryMap = new();
            checkAndInitCategoryDir();
            initCategoryMap();
        }

        /// <summary>
        /// initialize the category map by reading files in the categories directory
        /// </summary>
        private void initCategoryMap()
        {
            //foreach filename in DecisionCategories dir
            // str catName = filename/ 1st line of file
            // str desc = 2nd line of file
            // categoryMap.Add(KV<catName, desc>)
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
            }while(!isEitherExit(opt));
            return 0;
        }

        private bool isEitherExit(int opt)
        {
            return isChoiceMainExit(opt) || (isChoiceNo(opt) && !hasDecisionCategories());
        }

        private void writeMenu()
        {
            // if(hasDecisionCategories()) //FIXME: uncomment after initial test
            // {
            //     Console.WriteLine("What would you like us to choose today?");
            //     printSavedCategories();
            //     printExitChoice();                
            //     return;
            // }
            Console.WriteLine(NO_CATEGORIES_MSG);
            add1stDecisionCategory();
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

        private void add1stDecisionCategory()
        {
            Console.WriteLine("Let's add a decision category shall we? Please confirm that you would like to do so.");
            Console.WriteLine(
                "1. Yes\n" +
                "2. No\n");
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
            processMenuInput(opt);
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
            if(isChoiceInOptionRange(opt))
            {
                int catIdx = opt-1;
                Console.WriteLine($"Going to {categoryMap.Keys.ElementAt(catIdx)} menu...");
                enterCategoryMenu(catIdx);
            }
            else if(isChoiceMainExit(opt))
                Console.WriteLine(MENU_EXIT_MSG);
            else if(isChoiceAddNewCategory(opt))
                createDecisionCategory();
            else
                Console.WriteLine(INVALID_CHOICE_MSG);
        }


        // processes no categories menu input, 
        // aka adding the first category to the categories directory
        private void process1stCategoryConfirmation(int opt)
        {
            switch(opt){
                case YES_CODE:
                    createDecisionCategory();
                    break;
                case NO_CODE:
                    Console.WriteLine(MENU_EXIT_MSG);
                    break;
                default:
                    Console.WriteLine(INVALID_CHOICE_MSG);
                    break;
            }
        }

        // determine if the inputted option is for a pre-existing category
        private bool isChoiceInOptionRange(int opt)
        {
            return(categoryMap.Count > 0) && ((opt >= 1) && (opt <= categoryMap.Count));
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

        private void enterCategoryMenu(int categoryIdx)
        {
            string selectedCategory = categoryMap.ElementAt(categoryIdx).Key;
            Console.WriteLine($"Here are the options for the {selectedCategory} decision category: ");
            for(int i = 0; i < categoryMenuChoices.Length; i++)
                Console.WriteLine($"{i+1}. {categoryMenuChoices[i]}");
    
            printExitChoice();
            int categoryOpt = INVALID_OPT;
            do
            {
                categoryOpt = promptUser();
                processCategoryMenuInput(categoryOpt);
            }while(!isChoiceMainExit(categoryOpt));
        }

        private void processCategoryMenuInput(int opt)
        {
            if(isChoiceCategoryAction(opt))
            {
                processCategoryAction(opt);
            }
            else if(isChoiceMainExit(opt))
            {
                Console.WriteLine(MENU_EXIT_MSG);
            }
            else
                Console.WriteLine(INVALID_CHOICE_MSG);
        }

    /// <summary>
    /// Attempt to create a new decision category.
    /// </summary>
    /// <returns>
    /// bool - true if a user's new decision category was successfully configured
    /// </returns>
        private bool createDecisionCategory()
        {
            printExistingDecisionCategories();
            try
            {
                Console.WriteLine("Please help us create a new decision category...");
                // inputDecisionCategory();
            }
            catch(Exception e)
            {
                //TODO: save progress to a temporary file that's checked on bootup?
                // make Category object? -- 5/17/23
                Console.WriteLine($"{e} Failed to add new decision category. Saving any made progress");
                return false;
            }
            return true;
        }

        // Read all existing categories.
        private void printExistingDecisionCategories()
        {
            if(hasDecisionCategories()){
                Console.WriteLine("Here are the existing decision categories:");
                printSavedCategories();
            }
        }

        // accept user input for a new decision category step-by-step
        // TODO: extract to a category object for easy bundling
        private void inputDecisionCategory()
        {
                string categoryName = nameDecisionCategory();
                string categoryDesc = describeDecisionCategory();
                List<string> categoryChoices =  addChoicesToDecisionCategory(categoryName);
                string categoryPath = $"{DEFAULT_CATEGORY_DIRECTORY}{categoryName}.txt";
                File.WriteAllText(categoryPath, $"{categoryName}\n");
                File.WriteAllText(categoryPath, categoryDesc);
                File.WriteAllLines(categoryPath, categoryChoices);
        }



        private string nameDecisionCategory()
        {
            string categoryName = "";
            do
            {
                Console.WriteLine("Please name this new decision category (no duplicates allowed)");
                categoryName = Console.ReadLine()!;
            }while(String.IsNullOrWhiteSpace(categoryName) || categoryMap.Keys.Contains(categoryName));

            return categoryName;
        }

        private string describeDecisionCategory()
        {
            string categoryDesc = "";
            do
            {
                Console.WriteLine("Please give a description for this category: ");
                categoryDesc = Console.ReadLine()!;
            }while(!isInputAcceptable(categoryDesc));
            return categoryDesc;
        }

        private List<string> addChoicesToDecisionCategory(string currCategory)
        {
            List<string> acceptedChoices = new();
            string choiceInput = "";
            bool stopWanted = false;
            do{
                printAddChoiceLoopInstructions(acceptedChoices);
                choiceInput = Console.ReadLine()!;
                stopWanted = isInputStopCommand(choiceInput);
                bool accepted = false;
                if(!stopWanted)
                    accepted = tryAcceptNewCategoryChoice(choiceInput, acceptedChoices); // choose to accept or reject into choiceInputs

                printAddChoiceLoopMsg(accepted, choiceInput, acceptedChoices);
            }while(acceptedChoices.Count == 0 && !stopWanted);

            Console.WriteLine($"Choices approved for {currCategory}: {acceptedChoices}");
            return acceptedChoices;
        }

        private void printAddChoiceLoopInstructions(List<string> acceptedChoices)
        {
            const string introStart = "Please provide an alphanumeric string for a choice that hasn't already been added";
            string introEnd = (acceptedChoices.Count == 0) ? ":" : $"{STOP_INFO_MSG}:";
            Console.WriteLine(introStart + introEnd);
        }

        private void printAddChoiceLoopMsg(bool wasAccepted, string choiceStr, List<string> acceptedChoices)
        {
            string outputMsg = "";
            if(wasAccepted)
                outputMsg = $"{choiceStr} accepted into {acceptedChoices}";
            else if(acceptedChoices.Contains(choiceStr.ToLower()))
                outputMsg = $"{choiceStr} was already accepted";
            else if(!isInputAcceptable(choiceStr))
                outputMsg = "What you inputted was simply unaceeptable";
            
            if(outputMsg != "")
                Console.WriteLine(outputMsg);
        }

        bool tryAcceptNewCategoryChoice(string tentativeChoice, List<string> acceptedChoices)
        {
            if(isInputAcceptable(tentativeChoice) && !acceptedChoices.Contains(tentativeChoice.ToLower()))
            {
                acceptedChoices.Add(tentativeChoice);
                return true;
            }
            return false;
        }

        bool isInputAcceptable(string input)
        {
            return !String.IsNullOrWhiteSpace(input) && input.Length <= MAX_STRING_LEN;
        }

        private bool isChoiceCategoryAction(int opt)
        {
            return (opt >= 1) && (opt <= categoryMenuChoices.Length);
        }
 
        private void processCategoryAction(int actionNum)
        {
            switch(actionNum){
                case 1:
                    // decide
                    break;
                case 2:
                    // print choices
                    break;
                case 3:
                    break;
                case 4:
                    break;
                case 5:
                    break;
                case 6:
                    // delete category file
                    break;
                default:
                    Console.WriteLine("DecisionSect.cs: Invalid Category Action in process action. Something's up");
                    break;
            }
        }

        private List<string> readExistingCategory(string category)
        {
            // open the relevant txt file
            // one category per line
            // store in an array and return
            return null!;
        }
        private void askUserForFutureCategoryChoices(string category)
        {
            // input loop for one new choice at a time
            // if the choice doesn't exist already, add to new array
            // user can exit via entering any number or typing "exit"
            // add new choices to end of category file
        }

        private void addItemToList(){}
        private void printCategoryOptions(){}
        private void decideForUser(List<string> categoryChoices){}
        private int runRNG(){return 0;}
        private List<string> scanForCategories(){return null!;}

        private bool isInputStopCommand(string input)
        {
            string lCaseInput = input.ToLower();
            return lCaseInput == "stop" || lCaseInput == "exit" || isNumeric(lCaseInput);
        }

        private bool isNumeric(string input)
        {
            Regex numericOnly = new(@"^[0-9]+$");
            return numericOnly.IsMatch(input);
        }

        private bool isAlphaNumeric(string input){
            Regex alphaNumeric = new(@"^[0-9a-zA-Z\s,]*$");
            return alphaNumeric.IsMatch(input);
        }
    }
}