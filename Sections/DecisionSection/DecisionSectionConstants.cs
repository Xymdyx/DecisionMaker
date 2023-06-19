/*
* author: Sam Ford
* desc: Static class purely for storing Decision Section constants
* date started: approx 6/18/2023
*/
using System.Collections.Specialized;
namespace DecisionMaker
{
    public static class DecisionSectConstants
    {
        public const string DEFAULT_DC_DIRECTORY = @".\Decisions\Categories\";
        public const string DECISION_DELIMITER = "\n";
        public const string NO_DC_DIR_MSG = "No decisions directory detected in the desired location...Creating";
        public const string HAS_DCS_MSG = "What would you like us to choose today?";
        public const string NO_DCS_MSG = "Hmm. There appear to be no decision categories for us to choose from.";
        public const string STOP_INFO_MSG = "to stop adding";
        public const string NO_CHOICES_MSG = "No choices to choose from! Please add some...";
        public const string DECISIONS_WELCOME_MSG = "Welcome to the Decisions menu. This is where the magic happens!";
        public const string ADD_CHOICE_INTRO_MSG = "Please provide an alphanumeric string for a choice that hasn't already been added";
        public const string REMOVE_CHOICES_MENU_MSG = "Please select the number of the item you'd like to remove (can remove until nothing remains)...";
        public const string CREATE_DC_MSG = "Please help us create a new decision category...";
        public const string SHOW_DCS_MSG = "Here are the existing decision categories:";
        public const string NAME_DC_MSG = "Please name this new decision category (no duplicates allowed)";
        public const string DESCRIBE_DC_MSG = "Please give a description for this category:";
        public const string READ_DC_MSG = "Feel free to decide on your own if this list inspires you:";
        public const string ADD_CHOICE_REJECT_MSG = "What you inputted was simply unaceeptable";
        public const string REMOVE_CHOICE_REJECT_MSG = "What you inputted was invalid. Therefore, nothing was removed...";
        public const string DS_ERR_INTRO = "DecisionSect.cs: ";
        public const string ONE_OFF_DECIDE_MSG = "Deciding now for this one time decision....!";
        public static string RAND_BOUNDS_INFO = $"Please pick two numbers between {Int32.MinValue} and {Int32.MaxValue} (default is {DEFAULT_LOWER_BOUND}--{DEFAULT_UPPER_BOUND} inclusive):";
        public const string SAME_BOUNDS_COMMENT = "Same bounds, huh? Not very random but we'll allow it...";

        public const int ORIGIN_IDX = 0;
        public const int DELETE_ALL_CHOICES_CODE = -1;
        public const int DESC_LINE_IDX = 1;
        public const int INFO_LEN = 2;
        public const int DEFAULT_LOWER_BOUND = 1;
        public const int DEFAULT_UPPER_BOUND = 100;

        public static OrderedDictionary dcActions = new()
        {
            {"Make a decision", true},
            {"View decisions", false},
            {"Read category description", false},
            {"Change category description", false},
            {"Add choices", false},
            {"Remove choices", false},
            {"Delete entire category", true}
        };        

        public enum DcActionCodes
        {
            Decide = 1,
            ReadChoices,
            ReadDesc,
            ChangeDesc,
            AddChoices,
            RemoveChoices,
            DeleteDc
        }

        public enum NonDcActions
        {
            PickRandomInt = -3,
            DoOneOff,
            AddNewDc
        }
    }
}