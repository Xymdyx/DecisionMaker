/*
* author: Sam Ford
* desc: Utilities for menu creation and loops
* date started: approx 5/5/2023
*/
using TU = DecisionMaker.TextUtils;
namespace DecisionMaker
{
    internal static class MenuUtils
    {
        internal static string BINARY_CHOICE_MSG ="1. Yes\n2. No"; 
        internal const string CHOOSE_NUM_MSG = "Please choose a valid number: "; 
        internal const string INVALID_CHOICE_MSG = "What you inputted was not a valid choice, please try again."; 
        internal const string MENU_EXIT_MSG = "Exiting to previous menu";
        private const string MU_INFO_INTRO = "MenuUtils.cs: ";
        internal const int INVALID_OPT = Int32.MinValue; 
        internal const int EXIT_CODE = 0; 
        internal const int YES_CODE = 1; 
        internal const int NO_CODE = 2;
        internal const int MENU_START = 1;

        /// <summary>
        /// main user choice parsing method where they must choose from listed integers
        /// </summary>
        /// <returns>
        /// a processed integer... INVALID_OPT for invalid choice
        /// </returns>
        internal static int promptUserAndReturnOpt()
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
                Console.WriteLine($"{MU_INFO_INTRO} failed in prompt user....");
                TU.logErrorMsg(e);
            }
            return opt;        
        }

        internal static bool isChoiceMenuExit(int opt)
        {
            return opt == EXIT_CODE;
        }

        internal static bool isChoiceYes(int opt)
        {
            return opt == YES_CODE;
        }        

        internal static bool isChoiceNo(int opt)
        {
            return opt == NO_CODE;
        }

        internal static void printToPreviousMenu()
        {
            Console.WriteLine("Returning to previous menu");
        }

        internal static void printExitChoice()
        {
            Console.WriteLine($"{EXIT_CODE}. Exit");
        }

        internal static bool isBinaryChoice(int opt)
        {
            return isBinaryInputExit(opt) || isChoiceNo(opt);
        }

        internal static bool isBinaryInputExit(int opt)
        {
            return isChoiceYes(opt) || isChoiceMenuExit(opt);
        }

        internal static void writeBinaryMenu()
        {
            Console.WriteLine(BINARY_CHOICE_MSG);
            printExitChoice();
        }

        internal static void writeInvalidMsg()
        {
            Console.WriteLine(MenuUtils.INVALID_CHOICE_MSG);
        }
    }
}