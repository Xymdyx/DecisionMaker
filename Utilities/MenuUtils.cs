/*
* author: Sam Ford
* desc: Utilities for menu creation and loops
* date started: approx 5/5/2023
*/
using TU = DecisionMaker.TextUtils;
namespace DecisionMaker
{
    public static class MenuUtils
    {
        public static string BINARY_CHOICE_MSG ="1. Yes\n2. No"; 
        public const string CHOOSE_NUM_MSG = "Please choose a valid number: "; 
        public const string INVALID_CHOICE_MSG = "What you inputted was not a valid choice, please try again."; 
        public const string MENU_EXIT_MSG = "Exiting to previous menu";
        private const string MU_INFO_INTRO = "MenuUtils.cs: ";
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
        public static int promptUserAndReturnOpt()
        {
            Console.WriteLine(CHOOSE_NUM_MSG);
            int opt = INVALID_OPT;
            try
            {
                string input = Console.ReadLine()!;
                Console.WriteLine();
                opt = TU.convertMenuInputToInt(input);
            }
            catch(Exception e)
            {
                Console.WriteLine($"{MU_INFO_INTRO} failed in prompt user....\n{e.Message}\n");
            }
            return opt;        
        }

        public static bool isChoiceMenuExit(int opt)
        {
            return opt == EXIT_CODE;
        }

        public static bool isChoiceYes(int opt)
        {
            return opt == YES_CODE;
        }        

        public static bool isChoiceNo(int opt)
        {
            return opt == NO_CODE;
        }

        public static void printToPreviousMenu()
        {
            Console.WriteLine("Returning to previous menu");
        }

        public static void printExitChoice()
        {
            Console.WriteLine($"{EXIT_CODE}. Exit");
        }

        public static bool isBinaryChoice(int opt)
        {
            return isBinaryInputExit(opt) || isChoiceNo(opt);
        }

        public static bool isBinaryInputExit(int opt)
        {
            return isChoiceYes(opt) || isChoiceMenuExit(opt);
        }

        public static void writeBinaryMenu()
        {
            Console.WriteLine(BINARY_CHOICE_MSG);
            printExitChoice();
        }

        public static void writeInvalidMsg()
        {
            Console.WriteLine(MenuUtils.INVALID_CHOICE_MSG);
        }
    }
}