namespace DecisionMaker
{
    public static class MenuUtils
    {
        public static string BINARY_CHOICE_MSG ="1. Yes\n2. No\n"; 
        public const string CHOOSE_NUM_MSG = "Please choose a valid number: "; 
        public const string INVALID_CHOICE_MSG = "What you inputted was not a valid choice, please try again."; 
        public const string MENU_EXIT_MSG = "Exiting to main menu";
        private const string MU_ERR_INTRO = "MenuUtils.cs: ";

        public const int INVALID_OPT = Int32.MinValue; 
        public const int EXIT_CODE = 0; 
        public const int YES_CODE = 1; 
        public const int NO_CODE = 2;
        public const int MENU_START = 1;
        
        /// <summary>
        /// main user choice parsing method where they must choose from listed integers
        /// </summary>
        /// <returns>
        /// a processed integer... INVALID_OPT for invalid choice
        /// </returns>
        public static int promptUser()
        {
            Console.WriteLine(MenuUtils.CHOOSE_NUM_MSG);
            string input = Console.ReadLine()!;
            int opt = convertInputToInt(input);
            return opt;        
        }

        public static int convertInputToInt(string input)
        {
            int opt = MenuUtils.INVALID_OPT;
            try
            {
                opt = System.Int32.Parse(input);
            }
            catch(Exception e) 
            {
                Console.Error.WriteLine(MU_ERR_INTRO + $"Cannot convert input to integer: {e}");
            }
            return opt;
        }

        public static bool isChoiceMenuExit(int opt)
        {
            return opt == MenuUtils.EXIT_CODE;
        }

        public static bool isChoiceYes(int opt)
        {
            return opt == MenuUtils.YES_CODE;
        }        

        public static bool isChoiceNo(int opt)
        {
            return opt == MenuUtils.NO_CODE;
        }
    }
}