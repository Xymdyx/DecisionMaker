using System;

namespace DecisionMaker
{
    public class DecisionsSection : IDecisionMakerSection
    {
        // need category file directory
        private const string DEFAULT_CATEGORY_DIRECTORY = ".\\Decisions\\Categories";
        // category files with items delimited by newlines
        private const string DECISION_DELIMITER = "\n";
        private const string NO_CATEGORIES_MSG = "Hmm. There appear to be no decision categories for us to choose from.";
        private const string INVALID_CHOICE_MSG = "What you inputted was not a valid option, please try again.";
        private const string MENU_EXIT_MSG = "Exiting to main menu";
        private int EXIT_CODE = 0;
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
            readExistingLists(); // initialize the map
        }

        public int doMenuLoop()
        {
            Console.WriteLine("Welcome to the Decisions menu. This is where the magic happens!");
            int opt = -1;
            do
            {
                writeMenu();
                opt = promptUser();
            }while(!isChoiceMainExit(opt));
            return 0;
        }

        private void writeMenu()
        {
            int totalCategories = categoryMap.Count;
            if(totalCategories > 0)
            {
                printSavedCategories();
                return;
            }
            Console.WriteLine(NO_CATEGORIES_MSG);
        }

        private void printSavedCategories()
        {
            int totalCategories = categoryMap.Count;            
            Console.WriteLine("What would you like us to choose today?");
            for(int i = 0; i < totalCategories; i++)
            {
                KeyValuePair<string,string> category = categoryMap.ElementAt(i);
                Console.WriteLine($" {i+1}. {category.Key}: {category.Value}");
            }
            printExitChoice();
        }

        private void printExitChoice(){
            Console.WriteLine($"{EXIT_CODE}. Exit");
        }

        private int promptUser()
        {
            Console.WriteLine("Please choose a valid number: ");
            string input = Console.ReadLine()!;
            int opt = getUserInput(input);
            processMenuInput(opt);
            return opt;        
        }

        private int getUserInput(string input)
        {
            int opt = -1;
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

        private void processMenuInput(int opt)
        {
            if(isChoiceInOptionRange(opt))
            {
                Console.WriteLine($"Going to {categoryMap.Keys.ElementAt(opt)} menu...");
                enterCategoryMenu(opt);
            }
            else if(isChoiceMainExit(opt))
            {
                Console.WriteLine(MENU_EXIT_MSG);
            }
            else
                Console.WriteLine(INVALID_CHOICE_MSG);
        }

        private bool isChoiceInOptionRange(int opt)
        {
            return(categoryMap.Count > 0) && ((opt >= 1) && (opt <= categoryMap.Count));
        }

        private bool isChoiceMainExit(int opt)
        {
            return opt == EXIT_CODE;
        }

        private void enterCategoryMenu(int categoryIdx)
        {
            string selectedCategory = categoryMap.ElementAt(categoryIdx).Key;
            Console.WriteLine($"Here are the options for the {selectedCategory} decision category: ");
            for(int i = 0; i < categoryMenuChoices.Length; i++)
                Console.WriteLine($"{i+1}. {categoryMenuChoices[i]}");
    
            printExitChoice();
            int categoryOpt = -1;
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

        private bool isChoiceCategoryAction(int opt){
            return (opt >= 1) && (opt <= categoryMenuChoices.Length);
        }
 
        private void processCategoryAction(int actionNum){
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
                    Console.WriteLine("DecisionSect.cs: Invalid Category Action in process action. Something' up");
                    break;
            }
        }

        private void readExistingLists(){}

        private void readExistingCategory(string category)
        {
            // open the relevant txt file
            // one category per line
            // store in an array and return
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

    }
}