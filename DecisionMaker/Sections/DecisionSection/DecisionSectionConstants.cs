/*
* author: Sam Ford
* desc: Static class purely for storing Decision Section constants
* date started: approx 6/18/2023
*/
using System.Collections.Specialized;
namespace DecisionMaker
{
    internal static class DecisionSectConstants
    {
        internal const string DEFAULT_DECISIONS_DIRECTORY = @".\Decisions\";
        internal const string DEFAULT_DC_DIRECTORY = DEFAULT_DECISIONS_DIRECTORY + @"Categories\";
        internal const string DEFAULT_LAST_SESSION_DIRECTORY = DEFAULT_DECISIONS_DIRECTORY + @"LastSessionDecisions\";
        internal const string DEFAULT_LAST_SESSION_FPATH = DEFAULT_LAST_SESSION_DIRECTORY + "LastSessionDecisions.txt";
        internal const string DECISION_DELIMITER = "\n";
        internal const string NO_DC_DIR_MSG = "No decisions directory detected in the desired location...Creating";
        internal const string HAS_DCS_MSG = "What would you like us to choose today?";
        internal const string NO_DCS_MSG = "Hmm. There appear to be no decision categories for us to choose from.";
        internal const string STOP_INFO_MSG = "to stop adding";
        internal const string NO_CHOICES_MSG = "No choices to choose from! Please add some...";
        internal const string DECISIONS_WELCOME_MSG = "Welcome to the Decisions menu. This is where the magic happens!";
        internal const string ADD_CHOICE_INTRO_MSG = "Please provide an alphanumeric string for a choice that hasn't already been added";
        internal const string REMOVE_CHOICES_MENU_MSG = "Please select the number of the item you'd like to remove (can remove until nothing remains)...";
        internal const string CREATE_DC_MSG = "Please help us create a new decision category...";
        internal const string SHOW_DCS_MSG = "Here are the existing decision categories:";
        internal const string NAME_DC_MSG = "Please name this new decision category (no duplicates allowed)";
        internal const string DESCRIBE_DC_MSG = "Please give a description for this category:";
        internal const string READ_DC_MSG = "Feel free to decide on your own if this list inspires you:";
        internal const string ADD_CHOICE_REJECT_MSG = "What you inputted was simply unaceeptable";
        internal const string REMOVE_CHOICE_REJECT_MSG = "What you inputted was invalid. Therefore, nothing was removed...";
        internal const string DS_INFO_INTRO = "DecisionSect.cs: ";
        internal const string ONE_OFF_DECIDE_MSG = "Deciding now for this one time decision....!";
        internal static readonly string RAND_BOUNDS_INFO = $"Please pick two numbers between {Int32.MinValue} and {Int32.MaxValue} (default is {DEFAULT_LOWER_BOUND}--{DEFAULT_UPPER_BOUND} inclusive):";
        internal const string SAME_BOUNDS_COMMENT = "Same bounds, huh? Not very random but we'll allow it...";
        internal const string DEC_SUMMARY_FILE_INTRO = "Summary of all decisions made on";
        internal const string NO_DECISIONS_MSG = "No decisions made this session. Maybe next time!\n";
        internal const int ORIGIN_IDX = 0;
        internal const int DELETE_ALL_CHOICES_CODE = -1;
        internal const int DESC_LINE_IDX = 1;
        internal const int INFO_LEN = 2;
        internal const int DEFAULT_LOWER_BOUND = 1;
        internal const int DEFAULT_UPPER_BOUND = 100;

        internal static readonly OrderedDictionary dcActions = new()
        {
            {"Make a decision", true},
            {"View decisions", false},
            {"Read category description", false},
            {"Change category description", false},
            {"Rename category", false},
            {"Add choices", false},
            {"Remove choices", false},
            {"Delete entire category", true}
        };        

        internal enum DcActionCodes
        {
            Decide = 1,
            ReadChoices,
            ReadDesc,
            ChangeDesc,
            Rename,
            AddChoices,
            RemoveChoices,
            DeleteDc
        }

        internal enum NonDcActions
        {
            PickRandomInt = -3,
            DoOneOff,
            AddNewDc
        }
    }
}