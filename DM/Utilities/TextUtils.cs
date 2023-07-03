/* desc: static class that verifies the format of user input
* and also formats input into a new outputted format
* author: Sam Ford
*/

using System;
using System.Text.RegularExpressions;
namespace DecisionMaker
{
    internal static class TextUtils
    {
        internal const string BLANK = "";
        internal const string TXT = ".txt";
        internal const int MAX_STRING_LEN = 360;
        internal const int MAX_FNAME_LEN = 256;
        internal static readonly string[] stopWords = { "stop", "exit", "done", "good", "quit", "leave", "finish", "end"};
        internal const string BAD_FNAME_CHARS = "!@#$%&*+=#`|\\/<>{}:?\"\'.";
        internal const string GOOD_FNAME_CHAR = "-";
        internal const string PAUSE_PROMPT = "\nPress any key to continue...\n";
        private const string TU_INFO_HEADER = "TextUtils.cs: ";

        internal static bool isInputAcceptable(string input)
        {
            return !String.IsNullOrWhiteSpace(input) && !isStringTooLong(input);
        }

        internal static bool isInputStopCommand(string input)
        {
            return stopWords.Contains(input) || TextUtils.isNumeric(input);
        }

        internal static bool isNumeric(string input)
        {
            Regex numericOnly = new(@"^[0-9]+$");
            return tryMatchRegex(numericOnly, input);
        }

        internal static bool isAlpha(string input)
        {
            Regex alphaNumeric = new(@"^[a-zA-Z\s,]*$");
            return tryMatchRegex(alphaNumeric, input);
        }

        private static bool tryMatchRegex(Regex r, string s)
        {
            bool matchesRegex = false;
            try
            {
                matchesRegex = r.IsMatch(s);
            }
            catch(Exception e)
            {
                Console.WriteLine($"{TU_INFO_HEADER} Error in matching {s} regex {r}...");
                logErrorMsg(e);
            }
            return matchesRegex;
        }

        // return string as comma separated list
        internal static string prettyStringifyList(List<string> items)
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
         internal static void writeListAsNumberMenu(List<string> list)
         {
            string numberMenu = getListAsNumberMenu(list);
            if(numberMenu != TU.BLANK)
                Console.Write(numberMenu);
        }

         internal static string getListAsNumberMenu(List<string> list)
         {
            string numberMenu = TU.BLANK;
            for(int i = 0; i < list.Count; i++)
                numberMenu += ($"{i+1}. {list[i]}\n");
            return numberMenu;
        }

         internal static bool isStringListEmpty(List<string> strings)
         {
            return strings.Count == 0;
        }

        internal static void writeInfoAndPause(string info)
        {
            try
            {
                Console.WriteLine(info);
                Console.WriteLine(PAUSE_PROMPT);
                Console.ReadKey();
                Console.WriteLine();
            }
            catch(Exception e)
            {
                Console.WriteLine($"{TU_INFO_HEADER} Error in writeInfoAndPause...");
                logErrorMsg(e);
            }
        }

        internal static int convertMenuInputToInt(string input)
        {
            int opt = MU.INVALID_OPT;
            try
            {
                opt = System.Int32.Parse(input);
            }
            catch(Exception e)
            {
                Console.Error.WriteLine(TU_INFO_HEADER + $"Cannot convert menu input {input} to integer...");
                logErrorMsg(e);
            }
            return opt;
        }

        internal static bool convertTextToInt32(string text, out int opt)
        {
            bool success = false;
            try
            {
                opt = System.Int32.Parse(text);
                success = true;
            }
            catch(Exception e)
            {
                opt = MU.INVALID_OPT;
                Console.Error.WriteLine(TU_INFO_HEADER + $"Cannot convert input {text} to integer...");
                logErrorMsg(e);
            }
            return success;
        }

        internal static void logErrorMsg(Exception e)
        {
            Console.WriteLine($"{e.Message}\n");
        }

        internal static bool doesStringListHaveNonBlankEl(int qIdx, List<string> strs)
        {
            return (qIdx >= 0 && qIdx < strs.Count) && !String.IsNullOrWhiteSpace(strs[qIdx]);
        }

        internal static string readLineAndTrim()
        {
            string trimmedIn = BLANK;
            try
            {
                trimmedIn = Console.ReadLine()!.Trim();
            }
            catch(Exception e)
            {
                Console.WriteLine($"{TU_INFO_HEADER} failed to read and trim input...");
                logErrorMsg(e);
            }
            return trimmedIn;
        }

        internal static List<string> readFileLinesAndTrim(string fPath)
        {
            List<string> fLines = new();
            if(File.Exists(fPath))
            {
                try
                {
                    fLines = File.ReadAllLines(fPath).ToList();
                    fLines.ForEach(s => s.Trim());
                }
                catch(Exception e)
                {
                    Console.WriteLine($"{TU_INFO_HEADER} failed to read lines of {fPath} file...");
                    logErrorMsg(e);
                }
            }
            return fLines;
        }

        internal static string readWholeFileAndTrim(string fPath)
        {
            string fContents = BLANK;
            if(File.Exists(fPath))
            {
                try
                {
                    fContents = File.ReadAllText(fPath).Trim();
                }
                catch(Exception e)
                {
                    Console.WriteLine($"{TU_INFO_HEADER} failed to read {fPath} file contents...");
                    logErrorMsg(e);
                }
            }
            return fContents;
        }

        internal static bool isStringTooLong(string s)
        {
            return (s != null) && (s.Length > MAX_STRING_LEN);
        }

        internal static string replaceBadCharsinFname(string fName)
        {
            string goodName = fName;
            try
            {
                Regex badFnameCharsRegex = new($"[{BAD_FNAME_CHARS}]+");
                goodName = badFnameCharsRegex.Replace(fName, GOOD_FNAME_CHAR);
            }
            catch(Exception e)
            {
                Console.WriteLine($"{TU_INFO_HEADER} failed to replace bad file name chars in {fName}...");
                logErrorMsg(e);
            }
            return goodName;
        }
    }
}
