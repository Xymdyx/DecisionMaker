/*
* author: Sam Ford
* desc: Section for decision categories and making decisions from them
* note that DC == "Decision Category"
* date started: approx 4/23/2023
*/
using MU = DecisionMaker.MenuUtils;
using TU = DecisionMaker.TextUtils;
using DC = DecisionMaker.DecisionCategory;
using FS = DecisionMaker.FilesSection;
using FSC = DecisionMaker.FileSectConstants;
using DSC = DecisionMaker.DecisionSectConstants;
namespace DecisionMaker
{
    internal class DecisionsSection:IDecisionMakerSection
    {
        private readonly Random rng;
        private Dictionary<string, DC> _dcMap;
        internal Dictionary<string, DC> DcMap{ get => _dcMap; }

        internal DecisionsSection()
        {
            this.rng = new();
            this._dcMap = new();
            this._dcMap = new();
            checkAndInitDir();
            addNewCategoriesToMap();
        }

        private void fullyUpdateStoredDCs()
        {
            syncDcMapToDcDir();
            addNewCategoriesToMap();
        }

        internal void syncDcMapToDcDir()
        {
            if(checkAndInitDir())
                removeOldCategoriesFromMap();
        }

        /// <summary>
        /// initialize categories directory on startup if it doesn't exist already
        /// </summary>
        internal static bool checkAndInitDir()
        {
            if (!Directory.Exists(DSC.DEFAULT_DC_DIRECTORY))
            {
                try
                {
                    Console.WriteLine(DSC.NO_DC_DIR_MSG);
                    Directory.CreateDirectory(DSC.DEFAULT_DC_DIRECTORY);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{DSC.DS_INFO_INTRO} failed to initialize {DSC.DEFAULT_DC_DIRECTORY} directory...\n{e.Message}\n");
                }
            }
            return Directory.Exists(DSC.DEFAULT_DC_DIRECTORY);
        }

        // initialize the category map by reading files in Categories directory
        private void addNewCategoriesToMap()
        {
            List<string> existing = scanForDCs();
            foreach(string cat in existing.Where(c => !_dcMap.ContainsKey(c)))
            {
                string catPath = formatDCPath(cat);
                string[] catLines = File.ReadAllLines(catPath);
                string catDesc = catLines[DSC.DESC_LINE_IDX];
                List<string> catChoices = getChoicesFromDcFile(cat);

                DC dc = new(cat, catDesc, catChoices);
                _dcMap.TryAdd(cat, dc);
            }
        }

        // remove map categories no longer in Categories directory
        private void removeOldCategoriesFromMap()
        {
            List<string> existing = scanForDCs();
            foreach(string cat in _dcMap.Keys.Where(c => !existing.Contains(c)).ToList())
                _dcMap.Remove(cat);
        }

        internal static string formatDCPath(string dc)
        {
            return DSC.DEFAULT_DC_DIRECTORY + dc + TU.TXT;
        }

        /// <summary>
        /// scans the \Decisions\Categories directory for txts and returns them as strings
        /// </summary>
        /// <returns>a list of the decision category filenames </returns>
        private List<string> scanForDCs()
        {
            try
            {
                List<string> files = Directory.GetFiles(DSC.DEFAULT_DC_DIRECTORY, $"*{TU.TXT}").ToList();
                int i = 0;
                foreach (string path in files.ToList())
                {
                    int catLen = path.Length - DSC.DEFAULT_DC_DIRECTORY.Length - TU.TXT.Length;
                    files[i] = path.Substring(DSC.DEFAULT_DC_DIRECTORY.Length, catLen);
                    ++i;
                }
                return files;
            }
            catch(Exception e)
            {
                Console.WriteLine(DSC.DS_INFO_INTRO + $"Error scanning for categories...\n{e.Message}\n");
            }
            return new();
        }

        private List<string> getChoicesFromDcFile(string dc)
        {
            return File.ReadAllLines(formatDCPath(dc)).Skip(DSC.INFO_LEN).ToList();
        }

        /// <summary>
        /// the entry loop for the Decision Section
        /// </summary>
        /// <returns></returns>
        internal int doMenuLoop()
        {
            Console.WriteLine(DSC.DECISIONS_WELCOME_MSG);
            int opt = MU.INVALID_OPT;
            do
            {
                fullyUpdateStoredDCs();
                writeDCsMenu();
                opt = MU.promptUserAndReturnOpt();
                processMenuInput(opt);
            }while(!wantsToExit(opt));
            return opt;
        }

        private bool wantsToExit(int opt)
        {
            return MU.isChoiceMenuExit(opt) || (MU.isChoiceNo(opt) && !hasDCs());
        }

        private bool hasDCs()
        {
            return _dcMap.Count > 0;
        }

        private void writeDCsMenu()
        {
            string status = (hasDCs()) ? DSC.HAS_DCS_MSG : DSC.NO_DCS_MSG;
            Console.WriteLine(status);
            printSavedDCs();
            printNonDcActions();
        }

        internal void printSavedDCs()
        {
            int totalCategories = _dcMap.Count;
            for(int i = 0; i < totalCategories; i++)
            {
                DC dc = _dcMap.ElementAt(i).Value;
                Console.WriteLine($"{i+1}. {dc}");
            }
        }

        private void printNonDcActions()
        {
            MU.printExitChoice();
            Console.WriteLine($"{(int) DSC.NonDcActions.AddNewDc}. Add a whole new Decision Category");
            Console.WriteLine($"{(int) DSC.NonDcActions.DoOneOff}. Make a quick one-off decision");
            Console.WriteLine($"{(int) DSC.NonDcActions.PickRandomInt}. Have us pick a random number in a range");
        }

        // processes entry point menu
        private void processMenuInput(int opt)
        {
            if(isChoiceInChoiceRange(opt))
                enterDCActionsMenu(opt);
            else
            {
                processNonDcActions(opt);
                Console.WriteLine();
            }
        }

        private void processNonDcActions(int opt)
        {
        /*TODO: 1. Add oneoff option
        *       2. Add random number option, which prints a random number given a range
        *       3. Remove add1stDc menu as DCs are no loner the only option.
        *       4. After the first if, use a switch with an enum...
        * - SF 6/16/23
        */
            switch(opt)
            {
                case (int) DSC.NonDcActions.AddNewDc:
                    createPermanentDC();
                    break;
                case (int) DSC.NonDcActions.DoOneOff:
                    doOneOffDecision();
                    break;
                case (int) DSC.NonDcActions.PickRandomInt:
                    doRandomInt();
                    break;
                case MU.EXIT_CODE:
                    Console.WriteLine(MU.MENU_EXIT_MSG);
                    break;
                default:
                    MU.writeInvalidMsg();
                    break;
            }
        }

        private bool doOneOffDecision()
        {
            try
            {
                DC oneTimeDecision = inputDC();
                Console.WriteLine(DSC.ONE_OFF_DECIDE_MSG);
                return decideForUser(oneTimeDecision);
            }
            catch(Exception e)
            {
                Console.WriteLine($"{DSC.DS_INFO_INTRO} Failed to do one-off decision...\n{e.Message}\n");
                return false;
            }
        }

        /// <summary>
        /// prompts user for two numbers and prints a random integer in their inclusive range
        /// </summary>
        /// <returns>whether this operation was successful </returns>
        private bool doRandomInt()
        {
            Console.WriteLine(DSC.RAND_BOUNDS_INFO);
            bool succ1 = TU.convertTextToInt32(Console.ReadLine()!, out int num1);
            bool succ2 = TU.convertTextToInt32(Console.ReadLine()!, out int num2);

            if(!succ1) num1 = DSC.DEFAULT_LOWER_BOUND;
            if(!succ2) num2 = DSC.DEFAULT_UPPER_BOUND;

            int rand = runRNG(num1, num2);
            commentIfBoundsSame(num1, num2);
            (int,int) bounds = returnBoundsTuple(num1, num2);

            Console.WriteLine($"Given the range of [{bounds.Item1}, {bounds.Item2}], we've decided upon {rand}...");
            return true;
        }

        private void commentIfBoundsSame(int num1, int num2)
        {
            if(num1 == num2)
                Console.WriteLine(DSC.SAME_BOUNDS_COMMENT);
        }

        internal string getDCNameFromMenuChoice(int opt)
        {
            return this._dcMap.ElementAt(opt - 1).Key;
        }

        internal DC getDCFromMenuChoice(int opt)
        {
            return _dcMap.ElementAt(opt - 1).Value;
        }

        // determine if the input is for an existing category
        internal bool isChoiceInChoiceRange(int opt)
        {
            return(hasDCs()) && ((opt >= MU.MENU_START) && (opt <= _dcMap.Count));
        }

        // loop for choosing what to do with a selected decision category
        private void enterDCActionsMenu(int dcChoice)
        {
            string selected = getDCNameFromMenuChoice(dcChoice);
            DC selectedDc = getDCFromMenuChoice(dcChoice);
            int dcOpt = MU.INVALID_OPT;
            bool doesTerminate = false;
            do
            {
                writeDCActionsMenu(selectedDc.CatName);
                dcOpt = MU.promptUserAndReturnOpt();
                doesTerminate = processDCActionsMenuInput(dcOpt, selected, selectedDc);
            }while(!MU.isChoiceMenuExit(dcOpt) && !doesTerminate);
        }

        private void writeDCActionsMenu(string dc)
        {
            Console.WriteLine($"Here are the choices for the {dc} decision category: ");
            List<string> actionNames = getDCActionKeys().ToList();
            TU.writeListAsNumberMenu(actionNames);
            MU.printExitChoice();
        }

        /// <summary>
        /// responds to what the user inputted in the category actions menu
        /// </summary>
        /// <param name="opt">- the valid/invalid option a user inputted </param>
        /// <param name="dc">- the category we're currently in </param>
        /// <returns>whether the categoryActions loop should terminate</returns>
        private bool processDCActionsMenuInput(int opt, string dc, DC selectedDC)
        {
            bool doesTerminate = false;
            if(isChoiceDCAction(opt))
                doesTerminate = processDCAction(opt, dc, selectedDC);
            else if (MU.isChoiceMenuExit(opt))
            {
                Console.WriteLine(MU.MENU_EXIT_MSG);
                doesTerminate = true;
            }
            else
                MU.writeInvalidMsg();

            Console.WriteLine();
            return doesTerminate;
        }

        /// <summary>
        /// Attempt to create a new decision category.
        /// </summary>
        /// <returns>
        /// bool- true if a user's new decision category was successfully configured
        /// false otherwise
        /// </returns>
        private bool createPermanentDC()
        {
            printExistingDCs();
            Console.WriteLine(DSC.CREATE_DC_MSG);
            DC newDc = inputDC();
            return newDc.IsValidDc() && newDc.saveFile() && _dcMap.TryAdd(newDc.CatName, newDc);
        }

        // Read all saved categories.
        private void printExistingDCs()
        {
            if(hasDCs())
            {
                Console.WriteLine(DSC.SHOW_DCS_MSG);
                printSavedDCs();
            }
        }

        // accept user input for a new decision category step-by-step
        private DC inputDC()
        {
            string dcName = "";
            string dcDesc = "";
            List<string> dcChoices = new();
            try
            {
                dcName = nameDC();
                dcDesc = describeDC();
                DC dc = new(dcName, dcDesc);
                dc.CatChoices = addChoicesToDC(dc);
                return dc;
            }
            catch(Exception e)
            {
                Console.WriteLine($"{DSC.DS_INFO_INTRO} Failed to add new decision category. Saving any made progress...\n{e.Message}\n");
                saveUnfinishedDC(dcName, dcDesc, dcChoices);
            }
            return DC.EmptyDc;
        }

        private void saveUnfinishedDC(string name, string desc, List<string> choices)
        {
            FS.checkAndInitDir();
            File.WriteAllText(FSC.DEFAULT_WIP_FILE, name + DSC.DECISION_DELIMITER);
            File.AppendAllText(FSC.DEFAULT_WIP_FILE, desc + DSC.DECISION_DELIMITER);
            File.AppendAllLines(FSC.DEFAULT_WIP_FILE, choices);
        }

        private string nameDC()
        {
            string dcName = "";
            do
            {
                Console.WriteLine(DSC.NAME_DC_MSG);
                dcName = Console.ReadLine()!;
            }while(String.IsNullOrWhiteSpace(dcName) || _dcMap.Keys.Contains(dcName.Trim()));

            return dcName;
        }

        private string describeDC()
        {
            string dcDesc = "";
            do
            {
                Console.WriteLine(DSC.DESCRIBE_DC_MSG);
                dcDesc = Console.ReadLine()!;
            }while(!TU.isInputAcceptable(dcDesc));
            return dcDesc;
        }

        private List<string> addChoicesToDC(DC selectedDc)
        {
            List<string> acceptedChoices = selectedDc.CatChoices;
            string choiceInput = "";
            bool stopWanted = false;
            do
            {
                printAddChoiceLoopInstructions(acceptedChoices);
                choiceInput = Console.ReadLine()!;
                stopWanted = TU.isInputStopCommand(choiceInput);
                bool accepted = false;
                if(!stopWanted)
                    accepted = tryAcceptNewDCChoice(choiceInput, acceptedChoices); // choose to accept or reject into choiceInputs

                printAddChoiceLoopMsg(accepted, choiceInput, acceptedChoices);
            }while(TU.isStringListEmpty(acceptedChoices) || !stopWanted);

            Console.WriteLine($"Choices for {selectedDc.CatName}: {TU.prettyStringifyList(acceptedChoices)}\n");
            return acceptedChoices;
        }

        private void printAddChoiceLoopInstructions(List<string> acceptedChoices)
        {
            string introEnd = (TU.isStringListEmpty(acceptedChoices)) ? ":" : $" {getAddChoicesStopMsg()}:";
            Console.WriteLine(DSC.ADD_CHOICE_INTRO_MSG + introEnd);
        }

        private string getAddChoicesStopMsg()
        {
            string stops = TU.prettyStringifyList(TU.stopWords.ToList());
            return $"({DSC.STOP_INFO_MSG}, type any positive number or any of the following in lowercase: {stops})";
        }

        private void printAddChoiceLoopMsg(bool wasAccepted, string candidate, List<string> acceptedChoices)
        {
            string outputMsg = "";
            if(wasAccepted)
                outputMsg = $"{candidate} accepted!";
            else if (isItemAlreadyAccepted(candidate, acceptedChoices))
                outputMsg = $"{candidate} was already accepted";
            else if (!TU.isInputAcceptable(candidate))
                outputMsg = DSC.ADD_CHOICE_REJECT_MSG;

            if(outputMsg != "")
                Console.WriteLine(outputMsg + "\n");
        }

        bool tryAcceptNewDCChoice(string candidate, List<string> acceptedChoices)
        {
            if(TU.isInputAcceptable(candidate) && !isItemAlreadyAccepted(candidate, acceptedChoices))
            {
                acceptedChoices.Add(candidate);
                return true;
            }
            return false;
        }

        private bool isItemAlreadyAccepted(string candidate, List<string> accepted)
        {
            return accepted.Exists(choice => choice.ToLower() == candidate.ToLower());
        }

        private bool isChoiceDCAction(int opt)
        {
            return (opt >= MU.MENU_START) && (opt <= DSC.dcActions.Count);
        }

        /// <summary>
        /// process actions after choosing an existing category
        /// </summary>
        /// <param name="actionNum">- the number the user inputted...</param>
        /// <param name="dc">- the existing chosen category... </param>
        /// <returns>- whether the chosen action should terminate the category menu loop</returns>
        private bool processDCAction(int actionNum, string dc, DC selectedDc)
        {
            bool confirmHalt = true;
            switch(actionNum)
            {
                case (int) DSC.DcActionCodes.Decide:
                    confirmHalt = decideForUser(selectedDc);
                    break;
                case (int) DSC.DcActionCodes.ReadChoices:
                    confirmHalt = readExistingDC(selectedDc);
                    break;
                case (int) DSC.DcActionCodes.ReadDesc:
                    confirmHalt = readDescDC(selectedDc);
                    break;
                case (int) DSC.DcActionCodes.ChangeDesc:
                    confirmHalt = changeDescDC(selectedDc);
                    break;
                case (int) DSC.DcActionCodes.AddChoices:
                    confirmHalt = addChoicesToExistingDC(selectedDc);
                    break;
                case (int) DSC.DcActionCodes.RemoveChoices:
                    confirmHalt = removeChoicesFromDC(selectedDc);
                    break;
                case (int) DSC.DcActionCodes.DeleteDc:
                    confirmHalt = confirmDeleteDC(selectedDc);
                    break;
                default:
                    Console.WriteLine(DSC.DS_INFO_INTRO + "Invalid Category Action in process action. Something's up");
                    break;
            }
            return getDCActionTerminateVals()[actionNum-1] && confirmHalt;
        }

        /// <summary>
        /// given a decision category, choose a random item from that decision category for the user to commit to
        /// </summary>
        /// <param name="dc">- the category to pull a choice from </param>
        private bool decideForUser(DC dc)
        {
            if(!dc.hasChoices())
            {
                Console.WriteLine(DSC.NO_CHOICES_MSG);
                return false;
            }

            int chosen = runRNG(DSC.ORIGIN_IDX, dc.CatChoices.Count - 1);
            Console.WriteLine($"For {dc.CatName}, we've decided upon: {dc.CatChoices[chosen]}");
            return true;
        }

        // print all options in a categories file line-by-line
        private bool readExistingDC(DC dc)
        {
            if(!dc.hasChoices())
            {
                Console.WriteLine(DSC.NO_CHOICES_MSG);
                return false;
            }

            Console.WriteLine(DSC.READ_DC_MSG);
            Console.WriteLine(dc.stringifyChoices());
            return false;
        }

        private bool readDescDC(DC dc)
        {
            bool exists = dc.checkFileExists();
            if(exists)
                Console.WriteLine($"Description for {dc.CatName}: {dc.CatDesc}");
            return !exists;
        }

        private bool changeDescDC(DC dc)
        {
            bool exists = dc.checkFileExists();
            if (exists)
            {
                readDescDC(dc);
                string newDesc = describeDC();
                dc.CatDesc = newDesc;
                Console.WriteLine($"Changed {dc.CatName} description to \"{dc.CatDesc}\"!");
                dc.saveFile();
            }
            return !exists;            
        }        

        private bool addChoicesToExistingDC(DC dc)
        {
            List<string> added = addChoicesToDC(dc);
            dc.CatChoices = added;
            return dc.saveFile();
        }

        private bool removeChoicesFromDC(DC dc)
        {
            if (!dc.hasChoices())
            {
                Console.WriteLine(DSC.NO_CHOICES_MSG);
                return false;
            }

            List<string> remaining = removeChoicesFromDCLoop(dc);
            dc.CatChoices = remaining;
            return dc.saveFile();
        }

        /// <summary>
        /// loop for removing choices until no choices remain
        /// </summary>
        /// <param name="selectedDc"></param>
        /// <returns></returns>
        private List<string> removeChoicesFromDCLoop(DC selectedDc)
        {
            List<string> remainingChoices = selectedDc.CatChoices;
            int opt = MU.INVALID_OPT;
            bool isExit = false;
            while(!TU.isStringListEmpty(remainingChoices) && !isExit)
            {
                writeRemoveChoicesMenu(remainingChoices);
                opt = MU.promptUserAndReturnOpt();
                isExit = MU.isChoiceMenuExit(opt);
                string removed = processRemoveDecisionChoice(opt, remainingChoices);
                if(!isExit) printRemoveChoicesLoopMsg(removed, remainingChoices, selectedDc.CatName);
            }
            return remainingChoices;
        }

         private void writeRemoveChoicesMenu(List<string> remaining)
         {
            Console.WriteLine(DSC.REMOVE_CHOICES_MENU_MSG);
            TU.writeListAsNumberMenu(remaining);
            MU.printExitChoice();
            printDeleteAllChoices();
        }

        private string processRemoveDecisionChoice(int opt, List<string> remainingChoices)
        {
            string removedEl = "";
            if (opt == DSC.DELETE_ALL_CHOICES_CODE)
                remainingChoices.Clear();
            else
               removedEl = tryRemoveChoice(opt, remainingChoices);

            return removedEl;
        }

        private void printDeleteAllChoices()
        {
            Console.WriteLine($"{DSC.DELETE_ALL_CHOICES_CODE}. To remove all choices");
        }

        private string tryRemoveChoice(int choiceOpt, List<string> remainingChoices)
        {
            string removed = "";
            if((MU.MENU_START <= choiceOpt) && (choiceOpt <= remainingChoices.Count))
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

        private void printRemoveChoicesLoopMsg(string removed, List<string> remainingChoices, string dc)
        {
            if (TU.isStringListEmpty(remainingChoices))
            {
                Console.WriteLine($"All choices removed from {dc} category\n");
                return;
            }
            else if (removed == "")
                Console.WriteLine(DSC.REMOVE_CHOICE_REJECT_MSG);
            else
                Console.WriteLine($"Successfully removed {removed} option!");

            Console.WriteLine($"{dc} choices remaining: {TU.prettyStringifyList(remainingChoices)}\n");
        }

        private bool confirmDeleteDC(DC dc)
        {
            int opt = MU.INVALID_OPT;
            bool terminateConfirm = false;
            do
            {
                Console.WriteLine($"Please confirm you want to delete the {dc} decision category:");
                MU.writeBinaryMenu();
                opt = MU.promptUserAndReturnOpt();
                terminateConfirm = processDeleteDCOpt(opt, dc);
            } while (!MU.isBinaryChoice(opt));
            return terminateConfirm;
        }

        private bool processDeleteDCOpt(int opt, DC dc)
        {
            switch(opt)
            {
                case MU.YES_CODE:
                    return dc.deleteFile() && _dcMap.Remove(dc.CatName);
                case MU.NO_CODE:
                    Console.WriteLine(MU.MENU_EXIT_MSG);
                    return false;
                case MU.EXIT_CODE:
                    return false;
                default:
                    MU.writeInvalidMsg();
                    return false;
            }
        }

        private int runRNG(int num1, int num2)
        {
            (int, int) bounds = returnBoundsTuple(num1, num2);
            return rng.Next(bounds.Item1, bounds.Item2);
        }

        /// <summary>
        /// returns an integer tuple of the form (lowerbound, upperbound)
        /// </summary>
        private (int, int) returnBoundsTuple(int num1, int num2)
        {
            int lb = Math.Min(num1, num2);
            int ub = Math.Max(num1, num2);
            return (lb,ub);
        }

        private string[] getDCActionKeys()
        {
            string[] actionKeys = new string[DSC.dcActions.Count];
            DSC.dcActions.Keys.CopyTo(actionKeys, DSC.ORIGIN_IDX);
            return actionKeys;
        }

        private bool[] getDCActionTerminateVals()
        {
            bool[] terminateVals = new bool[DSC.dcActions.Count];
            DSC.dcActions.Values.CopyTo(terminateVals, DSC.ORIGIN_IDX);
            return terminateVals;
        }
    }
}