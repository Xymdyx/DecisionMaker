/// desc: static class that verifies the format of user input
/// and also formats input into a new outputted format
/// author: Sam Ford

using System;
using System.Text.RegularExpressions;

namespace DecisionMaker
{
    public static class TextUtils
    {
        public const string TXT = ".txt";
        public const int MAX_STRING_LEN = 360;
        public static string[] stopWords = { "stop", "exit", "done", "good", "quit", "finished" };

        public static bool isInputAcceptable(string input)
        {
            return !String.IsNullOrWhiteSpace(input) && input.Length <= MAX_STRING_LEN;
        }
        
        public static bool isInputStopCommand(string input)
        {
            return stopWords.Contains(input) || TextUtils.isNumeric(input);
        }

        public static bool isNumeric(string input)
        {
            Regex numericOnly = new(@"^[0-9]+$");
            return numericOnly.IsMatch(input);
        }

        public static bool isAlpha(string input){
            Regex alphaNumeric = new(@"^[a-zA-Z\s,]*$");
            return alphaNumeric.IsMatch(input);
        }

        // return string as comma separated list
        public static string prettyStringifyList(List<string> items)
        {
            return string.Join(", ", items);
        }

        /// <summary>
        /// prints a list in the form:
        /// 1. [string]
        /// 2. [string]
        /// ...
        /// n. [string]
        /// </summary>
        /// <param name="list"> - the list to print</param>
         public static void writeListAsNumberMenu(List<string> list)
         {
            for(int i = 0; i < list.Count; i++)
                Console.WriteLine($"{i+1}. {list[i]}");
         }

         public static bool isStringListEmpty(List<string> strings)
         {
            return strings.Count == 0;
        }
    }
}
