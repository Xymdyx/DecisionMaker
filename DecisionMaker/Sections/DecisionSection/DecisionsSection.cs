/*
* author: Sam Ford
* desc: Section for decision categories and making decisions from them
* note that DC == "Decision Category"
* date started: approx 4/23/2023
*/
namespace DecisionMaker
{
    internal class DecisionsSection : IDecisionMakerSection
    {
        private readonly Random rng;
        private Dictionary<string, DC> _dcMap;
        private List<string> _decisionSummary;
        internal Dictionary<string, DC> DcMap { get => _dcMap; }
        internal List<string> DecisionSummary { get => _decisionSummary; }

        internal DecisionsSection()
        {
            this.rng = new();
            this._dcMap = new();
            this._dcMap = new();
            this._decisionSummary = new();
            checkAndInitDir();
            addNewDcsToMapFromDir();
        }

        internal static bool checkAndInitDir()
        {
            return MU.checkAndInitADir(DSC.DEFAULT_DC_DIRECTORY);
        }        

        internal void removeDcsFromMapNotInDir()
        {
            if (checkAndInitDir())
                removeOldDcsFromMap();
        }

        internal void addNewDcsToMapFromDir()
        {
            try
            {
                List<string> existing = scanForDcs();
                foreach (string cat in existing.Where(c => !_dcMap.ContainsKey(c)))
                {
                    string catPath = formatDcPath(cat);
                    string[] catLines = File.ReadAllLines(catPath);
                    string catDesc = catLines[DSC.DESC_LINE_IDX];
                    List<string> catChoices = getChoicesFromDcFile(cat);

                    DC dc = new(cat, catDesc, catChoices);
                    _dcMap.TryAdd(cat, dc);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine($"{DSC.DS_INFO_INTRO} failed to add new DCs from dc dir");
                TU.logErrorMsg(e);
            }
        }

        // remove map categories no longer in Categories directory
        private void removeOldDcsFromMap()
        {
            List<string> existing = scanForDcs();
            foreach (string cat in _dcMap.Keys.Where(c => !existing.Contains(c)).ToList())
                _dcMap.Remove(cat);
        }

        internal static string formatDcPath(string fName)
        {
            if (TU.isInputAcceptable(fName))
                return DSC.DEFAULT_DC_DIRECTORY + fName + TU.TXT;
            return fName;
        }

        internal bool saveAllDcsInMap()
        {
            bool success = true;
            foreach (KeyValuePair<string, DC> d in _dcMap)
                success &= d.Value.saveFile();

            if (!success)
                Console.WriteLine($"{DSC.DS_INFO_INTRO} Failed to save all Decision Category files!");
            return success;
        }

        /// <summary>
        /// scans the \Decisions\Categories directory for txts and returns them as strings
        /// </summary>
        /// <returns>a list of the decision category filenames or an empty list on error </returns>
        internal static List<string> scanForDcs()
        {
            List<string> files = new();
            try
            {
                files = Directory.GetFiles(DSC.DEFAULT_DC_DIRECTORY, $"*{TU.TXT}").ToList();
                int i = 0;
                foreach (string path in files.ToList())
                {
                    int catLen = path.Length - DSC.DEFAULT_DC_DIRECTORY.Length - TU.TXT.Length;
                    files[i] = path.Substring(DSC.DEFAULT_DC_DIRECTORY.Length, catLen);
                    ++i;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(DSC.DS_INFO_INTRO + $"Error scanning for categories...");
                TU.logErrorMsg(e);
                files.Clear();
            }
            return files;
        }

        private List<string> getChoicesFromDcFile(string dc)
        {
            List<string> choices = new();
            try
            {
                choices = File.ReadAllLines(formatDcPath(dc)).Skip(DSC.INFO_LEN).ToList();
            }
            catch(Exception e)
            {
                Console.WriteLine($"{DSC.DS_INFO_INTRO} failed to read choices from existing {dc} category file...");
                TU.logErrorMsg(e);
            }
            return choices;
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
                printDcsMenu();
                opt = MU.promptUserAndReturnOpt();
                processMenuInput(opt);
            } while (!MU.isChoiceMenuExit(opt));
            return opt;
        }

        private bool hasDcs()
        {
            return _dcMap.Count > 0;
        }

        private void printDcsMenu()
        {
            string status = (hasDcs()) ? DSC.HAS_DCS_MSG : DSC.NO_DCS_MSG;
            Console.WriteLine(status);
            printSavedDcs();
            printNonDcActions();
        }

        internal void printSavedDcs()
        {
            int totalCategories = _dcMap.Count;
            for (int i = 0; i < totalCategories; i++)
            {
                DC dc = _dcMap.ElementAt(i).Value;
                Console.WriteLine($"{i + 1}. {dc}");
            }
        }

        private void printNonDcActions()
        {
            MU.printExitChoice();
            Console.WriteLine($"{(int)DSC.NonDcActions.AddNewDc}. Add a whole new Decision Category");
            Console.WriteLine($"{(int)DSC.NonDcActions.DoOneOff}. Make a quick one-off decision");
            Console.WriteLine($"{(int)DSC.NonDcActions.PickRandomInt}. Have us pick a random number in a range");
        }

        private void processMenuInput(int opt)
        {
            if (isChoiceExistingDc(opt))
                enterDcActionsMenu(opt);
            else
            {
                processNonDcActions(opt);
                Console.WriteLine();
            }
        }

        private void processNonDcActions(int opt)
        {
            switch (opt)
            {
                case (int)DSC.NonDcActions.AddNewDc:
                    createPermanentDc();
                    break;
                case (int)DSC.NonDcActions.DoOneOff:
                    doOneOffDecision();
                    break;
                case (int)DSC.NonDcActions.PickRandomInt:
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

        /// <summary>
        /// walks a user through a one-off decision, which won't be saved to the system
        /// </summary>
        /// <returns>whether the one off decision was made successfully</returns>
        private bool doOneOffDecision()
        {
            bool success = false;
            try
            {
                DC oneTimeDecision = inputDc();
                Console.WriteLine(DSC.ONE_OFF_DECIDE_MSG);
                success = decideForUser(oneTimeDecision);
            }
            catch (Exception e)
            {
                Console.WriteLine($"{DSC.DS_INFO_INTRO} Failed to do one-off decision...");
                TU.logErrorMsg(e);
            }
            return success;
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

            if (!succ1) num1 = DSC.DEFAULT_LOWER_BOUND;
            if (!succ2) num2 = DSC.DEFAULT_UPPER_BOUND;

            int rand = runRNG(num1, num2);
            commentIfBoundsSame(num1, num2);
            (int, int) bounds = returnBoundsTuple(num1, num2);

            Console.WriteLine($"Given the range of [{bounds.Item1}, {bounds.Item2}], we've decided upon {rand}...");
            return true;
        }

        private void commentIfBoundsSame(int num1, int num2)
        {
            if (num1 == num2)
                Console.WriteLine(DSC.SAME_BOUNDS_COMMENT);
        }

        internal string getDcNameFromMenuChoice(int opt)
        {
            string dcName = TU.BLANK;
            try
            {
                dcName = this._dcMap.ElementAt(opt - 1).Key;
            }
            catch (Exception e)
            {
                Console.WriteLine($"{DSC.DS_INFO_INTRO} Cannot get category name!");
                TU.logErrorMsg(e);
            }
            return dcName;
        }

        internal DC getDcFromMenuChoice(int opt)
        {
            DC dcVal = DC.EmptyDc;
            try
            {
                dcVal = _dcMap.ElementAt(opt - 1).Value;
            }
            catch (Exception e)
            {
                Console.WriteLine($"{DSC.DS_INFO_INTRO} Cannot get category object!");
                TU.logErrorMsg(e);
            }
            return dcVal;
        }

        // determine if the parsed menu option is for a saved DC
        internal bool isChoiceExistingDc(int opt)
        {
            return (hasDcs()) && ((opt >= MU.MENU_START) && (opt <= _dcMap.Count));
        }

        // loop for choosing what to do with a selected decision category
        private void enterDcActionsMenu(int dcChoice)
        {
            string selected = getDcNameFromMenuChoice(dcChoice);
            DC selectedDc = getDcFromMenuChoice(dcChoice);
            int dcOpt = MU.INVALID_OPT;
            bool doesTerminate = false;
            do
            {
                writeDcActionsMenu(selectedDc.CatName);
                dcOpt = MU.promptUserAndReturnOpt();
                doesTerminate = processDcActionsMenuInput(dcOpt, selected, selectedDc);
            } while (!MU.isChoiceMenuExit(dcOpt) && !doesTerminate);
        }

        private void writeDcActionsMenu(string dc)
        {
            Console.WriteLine($"Here are the choices for the {dc} decision category: ");
            List<string> actionNames = getDcActionKeys().ToList();
            TU.writeListAsNumberMenu(actionNames);
            MU.printExitChoice();
        }

        /// <summary>
        /// responds to what the user inputted in the category actions menu
        /// </summary>
        /// <param name="opt">- the valid/invalid option a user inputted </param>
        /// <param name="dc">- the category we're currently in </param>
        /// <returns>whether the categoryActions loop should terminate</returns>
        private bool processDcActionsMenuInput(int opt, string dc, DC selectedDC)
        {
            bool doesTerminate = false;
            if (isChoiceDcAction(opt))
                doesTerminate = processDcAction(opt, dc, selectedDC);
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
        /// bool- if a user's new decision category was successfully configured
        /// </returns>
        private bool createPermanentDc()
        {
            Console.WriteLine(DSC.CREATE_DC_MSG);
            DC newDc = inputDc();
            return newDc.IsValidDc() && saveAndAddDcToMap(newDc);
        }

        /// <summary>
        /// saves and adds a DC to the decisions list
        /// </summary>
        /// <param name="dc"></param>
        /// <returns>bool - whether the dc file was saved and added to the map</returns>
        internal bool saveAndAddDcToMap(DC dc)
        {
            return dc.saveFile() && _dcMap.TryAdd(dc.CatName, dc);
        }

        // Read all saved categories.
        private void announceSavedDcs()
        {
            if (hasDcs())
            {
                Console.WriteLine(DSC.SHOW_DCS_MSG);
                printSavedDcs();
                Console.WriteLine();
            }
        }

        /// <summary>
        /// accept user input for a new decision category step-by-step
        /// </summary>
        /// <returns> a new DC upon completion</returns>
        private DC inputDc()
        {
            string dcName = TU.BLANK;
            string dcDesc = TU.BLANK;
            List<string> dcChoices = new();
            try
            {
                dcName = nameDc();
                dcDesc = describeDc();
                DC dc = new(dcName, dcDesc);
                dc.CatChoices = addChoicesToDc(dc);
                return dc;
            }
            catch (Exception e)
            {
                Console.WriteLine($"{DSC.DS_INFO_INTRO} Failed to add new decision category. Saving any made progress...");
                TU.logErrorMsg(e);
                saveUnfinishedDc(dcName, dcDesc, dcChoices);
            }
            return DC.EmptyDc;
        }

        private bool saveUnfinishedDc(string name, string desc, List<string> choices)
        {
            FS.checkAndInitDir();
            try
            {
                File.WriteAllText(FSC.DEFAULT_WIP_FILE, name + DSC.DECISION_DELIMITER);
                File.AppendAllText(FSC.DEFAULT_WIP_FILE, desc + DSC.DECISION_DELIMITER);
                File.AppendAllLines(FSC.DEFAULT_WIP_FILE, choices);
            }
            catch(Exception e)
            {
                Console.WriteLine($"{DSC.DS_INFO_INTRO} failure in saving wipcat file.");
                TU.logErrorMsg(e);
            }
            return File.Exists(FSC.DEFAULT_WIP_FILE);
        }

        private string nameDc()
        {
            announceSavedDcs();
            string dcName = TU.BLANK;
            do
            {
                Console.WriteLine(DSC.NAME_DC_MSG);
                dcName = Console.ReadLine()!;
            } while (!TU.isInputAcceptable(dcName) || doesMapHaveDcName(dcName.Trim()));

            return dcName;
        }

        private bool doesMapHaveDcName(string dcName)
        {
            return _dcMap.Keys.Contains(dcName);
        }        

        private string describeDc()
        {
            string dcDesc = TU.BLANK;
            do
            {
                Console.WriteLine(DSC.DESCRIBE_DC_MSG);
                dcDesc = Console.ReadLine()!;
            } while (!TU.isInputAcceptable(dcDesc));
            return dcDesc;
        }

        private List<string> addChoicesToDc(DC selectedDc)
        {
            List<string> acceptedChoices = selectedDc.CatChoices;
            string choiceInput = TU.BLANK;
            bool stopWanted = false;
            do
            {
                printAddDcChoiceLoopInstructions(acceptedChoices);
                choiceInput = Console.ReadLine()!;
                stopWanted = TU.isInputStopCommand(choiceInput);
                bool accepted = false;
                if (!stopWanted)
                    accepted = tryAcceptNewDcChoice(choiceInput, acceptedChoices); // choose to accept or reject into choiceInputs

                printAddDcChoiceLoopMsg(accepted, choiceInput, acceptedChoices);
            } while (TU.isStringListEmpty(acceptedChoices) || !stopWanted);

            Console.WriteLine($"Choices for {selectedDc.CatName}: {TU.prettyStringifyList(acceptedChoices)}\n");
            return acceptedChoices;
        }

        private void printAddDcChoiceLoopInstructions(List<string> acceptedChoices)
        {
            string introEnd = (TU.isStringListEmpty(acceptedChoices)) ? ":" : $" {getAddDcChoicesStopMsg()}:";
            Console.WriteLine(DSC.ADD_CHOICE_INTRO_MSG + introEnd);
        }

        private string getAddDcChoicesStopMsg()
        {
            string stops = TU.prettyStringifyList(TU.stopWords.ToList());
            return $"({DSC.STOP_INFO_MSG}, type any positive number or any of the following in lowercase: {stops})";
        }

        private void printAddDcChoiceLoopMsg(bool wasAccepted, string candidate, List<string> acceptedChoices)
        {
            string outputMsg = TU.BLANK;
            if (wasAccepted)
                outputMsg = $"{candidate} accepted!";
            else if (isItemAlreadyAccepted(candidate, acceptedChoices))
                outputMsg = $"{candidate} was already accepted";
            else if (!TU.isInputAcceptable(candidate))
                outputMsg = DSC.ADD_CHOICE_REJECT_MSG;

            if (outputMsg != TU.BLANK)
                Console.WriteLine(outputMsg + "\n");
        }

        private bool tryAcceptNewDcChoice(string candidate, List<string> acceptedChoices)
        {
            if (TU.isInputAcceptable(candidate) && !isItemAlreadyAccepted(candidate, acceptedChoices))
            {
                acceptedChoices.Add(candidate);
                return true;
            }
            return false;
        }

        internal bool isItemAlreadyAccepted(string candidate, List<string> accepted)
        {
            return accepted.Exists(choice => choice.ToLower() == candidate.ToLower());
        }

        internal bool isChoiceDcAction(int opt)
        {
            return (opt >= MU.MENU_START) && (opt <= DSC.dcActions.Count);
        }

        /// <summary>
        /// process actions after choosing an existing category
        /// </summary>
        /// <param name="actionNum">- the number the user inputted...</param>
        /// <param name="dc">- the existing chosen category... </param>
        /// <returns>- whether the chosen action should terminate the category menu loop</returns>
        private bool processDcAction(int actionNum, string dc, DC selectedDc)
        {
            bool confirmHalt = true;
            switch (actionNum)
            {
                case (int)DSC.DcActionCodes.Decide:
                    confirmHalt = decideForUser(selectedDc);
                    break;
                case (int)DSC.DcActionCodes.ReadChoices:
                    confirmHalt = readExistingDc(selectedDc);
                    break;
                case (int)DSC.DcActionCodes.ReadDesc:
                    confirmHalt = readDescDc(selectedDc);
                    break;
                case (int)DSC.DcActionCodes.ChangeDesc:
                    confirmHalt = changeDescDc(selectedDc);
                    break;
                case (int)DSC.DcActionCodes.Rename:
                    confirmHalt = renameDc(selectedDc);
                    break;
                case (int)DSC.DcActionCodes.AddChoices:
                    confirmHalt = addChoicesToExistingDc(selectedDc);
                    break;
                case (int)DSC.DcActionCodes.RemoveChoices:
                    confirmHalt = removeChoicesFromDc(selectedDc);
                    break;
                case (int)DSC.DcActionCodes.DeleteDc:
                    confirmHalt = confirmDeleteDc(selectedDc);
                    break;
                default:
                    Console.WriteLine(DSC.DS_INFO_INTRO + "Invalid Category Action in process action. Something's up");
                    break;
            }
            return getDcActionTerminateVals()[actionNum - 1] && confirmHalt;
        }

        /// <summary>
        /// given a decision category, choose a random item from that decision category for the user to commit to
        /// </summary>
        /// <param name="dc">- the category to pull a choice from </param>
        internal bool decideForUser(DC dc)
        {
            if (!dc.hasChoices())
            {
                Console.WriteLine(DSC.NO_CHOICES_MSG);
                return false;
            }

            int chosenInt = runRNG(DSC.ORIGIN_IDX, dc.CatChoices.Count - 1);
            string chosenOpt = dc.CatChoices[chosenInt];
            Console.WriteLine($"For {dc.CatName}, we've decided upon: {chosenOpt}");
            tryAddDecisionToSummary(dc, chosenOpt);
            return dc.checkFileExists();
        }

        /// <summary>
        /// Add a decision made by decideForUser to _decisionSummary
        /// to keep track of all decisions made this session
        /// pre-cond: the DC is non-null and in _dcMap
        /// </summary>
        /// <param name="dc"> Decision Category the user chose for a decision </param>
        /// <param name="chosenOpt"> the decision chosen by decideForUser </param>
        /// <returns>whether the decision was successfully recorded to the summary</returns>
        internal bool tryAddDecisionToSummary(DC dc, string chosenOpt)
        {
            string decision = TU.BLANK;
            int startSize = _decisionSummary.Count;
            if (dc.CatChoices.Contains(chosenOpt))
            {
                string decisionType = (doesMapHaveDcName(dc.CatName)) ? DC.DC_SUMMARY_TAG : DC.ONE_OFF_SUMMARY_TAG;
                decision = $"{dc.CatName} {decisionType}: {chosenOpt}";
                _decisionSummary.Add(decision);
            }
            return wasDecisionPutInSummary(decision, startSize);
        }

        private bool wasDecisionPutInSummary(string decision, int startSize)
        {
            return (_decisionSummary.Count == startSize + 1) && (_decisionSummary.Last() == decision);
        }

        /// <summary>
        /// print all options in a categories file line-by-line
        /// </summary>
        /// <param name="dc"></param>
        /// <returns>whether it is safe to continue operation on dc</returns>
        private bool readExistingDc(DC dc)
        {
            if (!dc.hasChoices())
            {
                Console.WriteLine(DSC.NO_CHOICES_MSG);
                return false;
            }

            Console.WriteLine(DSC.READ_DC_MSG);
            Console.WriteLine(dc.stringifyChoices());
            return dc.checkFileExists();
        }

        /// <summary>
        /// reads dc's description
        /// </summary>
        /// <param name="dc"></param>
        /// <returns>bool- whether it is safe to continue operation on dc</returns>
        private bool readDescDc(DC dc)
        {
            bool exists = dc.checkFileExists();
            if (exists)
                Console.WriteLine($"Description for {dc.CatName}: {dc.CatDesc}");
            return !exists;
        }

        /// <summary>
        /// let user change dc's description
        /// </summary>
        /// <param name="dc"></param>
        /// <returns>bool- whether it is safe to continue operation on dc</returns>
        private bool changeDescDc(DC dc)
        {
            bool exists = dc.checkFileExists();
            if (exists)
            {
                readDescDc(dc);
                string newDesc = describeDc();
                dc.CatDesc = newDesc;
                Console.WriteLine($"Changed {dc.CatName} description to \"{dc.CatDesc}\"!");
                dc.saveFile();
            }
            return !exists;
        }

        /// <summary>
        /// let user rename dc's description
        /// </summary>
        /// <param name="dc"></param>
        /// <returns>bool- whether it is safe to continue operation on dc</returns>
        private bool renameDc(DC dc)
        {
            if (!deleteAndRemoveDcFromMap(dc))
            {
                Console.WriteLine($"{DSC.DS_INFO_INTRO} Failed to delete old {dc.CatPath} file and remove {dc.CatName} from map...");
                return true;
            }

            Console.WriteLine($"Please rename the {dc.CatName} category (you may re-enter the same one)...");
            dc.CatName = nameDc();
            if (!saveAndAddDcToMap(dc))
            {
                Console.WriteLine($"{DSC.DS_INFO_INTRO} Failed to add new {dc.CatPath} file and add {dc.CatName} to map...");
                return true;
            }
            return !dc.checkFileExists();
        }


        /// <summary>
        /// let user add more unique choices to a DC
        /// </summary>
        /// <param name="dc"></param>
        /// <returns>bool- whether it is safe to continue operation on dc</returns>
        private bool addChoicesToExistingDc(DC dc)
        {
            List<string> added = addChoicesToDc(dc);
            dc.CatChoices = added;
            return dc.saveFile();
        }


        /// <summary>
        /// let user remove choices from a DC
        /// </summary>
        /// <param name="dc"></param>
        /// <returns>bool- whether it is safe to continue operation on dc</returns>
        private bool removeChoicesFromDc(DC dc)
        {
            if (!dc.hasChoices())
            {
                Console.WriteLine(DSC.NO_CHOICES_MSG);
                return false;
            }

            List<string> remaining = removeDcChoicesLoop(dc);
            dc.CatChoices = remaining;
            return dc.saveFile();
        }

        private List<string> removeDcChoicesLoop(DC selectedDc)
        {
            List<string> remainingChoices = selectedDc.CatChoices;
            int opt = MU.INVALID_OPT;
            bool isExit = false;
            while (!TU.isStringListEmpty(remainingChoices) && !isExit)
            {
                writeRemoveDcChoicesMenu(remainingChoices);
                opt = MU.promptUserAndReturnOpt();
                isExit = MU.isChoiceMenuExit(opt);
                string removed = processRemoveDcChoice(opt, remainingChoices);
                if (!isExit) printRemoveChoicesLoopMsg(removed, remainingChoices, selectedDc.CatName);
            }
            return remainingChoices;
        }

        private void writeRemoveDcChoicesMenu(List<string> remaining)
        {
            Console.WriteLine(DSC.REMOVE_CHOICES_MENU_MSG);
            TU.writeListAsNumberMenu(remaining);
            MU.printExitChoice();
            Console.WriteLine($"{DSC.DELETE_ALL_CHOICES_CODE}. To remove all choices");
        }

        private string processRemoveDcChoice(int opt, List<string> remainingChoices)
        {
            string removedEl = TU.BLANK;
            if (opt == DSC.DELETE_ALL_CHOICES_CODE)
                remainingChoices.Clear();
            else
                removedEl = tryRemoveChoice(opt, remainingChoices);

            return removedEl;
        }

        internal string tryRemoveChoice(int choiceOpt, List<string> remainingChoices)
        {
            string removed = TU.BLANK;
            if ((MU.MENU_START <= choiceOpt) && (choiceOpt <= remainingChoices.Count))
            {
                try
                {
                    int choiceIdx = choiceOpt - 1;
                    removed = remainingChoices.ElementAt(choiceIdx);
                    remainingChoices.RemoveAt(choiceIdx);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    Console.WriteLine($"{DSC.DS_INFO_INTRO} Failed to remove decision choice from {remainingChoices} given option {choiceOpt}");
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
            else if (removed == TU.BLANK)
                Console.WriteLine(DSC.REMOVE_CHOICE_REJECT_MSG);
            else
                Console.WriteLine($"Successfully removed {removed} option!");

            Console.WriteLine($"{dc} choices remaining: {TU.prettyStringifyList(remainingChoices)}\n");
        }

        /// <summary>
        /// let user delete a dc upon confirmation
        /// </summary>
        /// <param name="dc"></param>
        /// <returns>bool- whether it is safe to continue operation on dc</returns>
        private bool confirmDeleteDc(DC dc)
        {
            int opt = MU.INVALID_OPT;
            bool terminateConfirm = false;
            do
            {
                Console.WriteLine($"Please confirm you want to delete the {dc.CatName} decision category:");
                MU.writeBinaryMenu();
                opt = MU.promptUserAndReturnOpt();
                terminateConfirm = processDeleteDcOpt(opt, dc);
            } while (!MU.isBinaryChoice(opt));
            return terminateConfirm;
        }

        private bool processDeleteDcOpt(int opt, DC dc)
        {
            switch (opt)
            {
                case MU.YES_CODE:
                    return deleteAndRemoveDcFromMap(dc);
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

        /// <summary>
        /// completely remove DC from computer and dc dir
        /// </summary>
        /// <param name="dc"></param>
        /// <returns>bool - whether the file was completely removed from the program</returns>
        internal bool deleteAndRemoveDcFromMap(DC dc)
        {
            return _dcMap.Remove(dc.CatName) && dc.deleteFile();
        }

        private int runRNG(int num1, int num2)
        {
            (int, int) bounds = returnBoundsTuple(num1, num2);
            return rng.Next(bounds.Item1, bounds.Item2);
        }

        /// <summary>
        /// returns an integer tuple of the form (lowerbound, upperbound)
        /// </summary>
        internal (int, int) returnBoundsTuple(int num1, int num2)
        {
            int lb = Math.Min(num1, num2);
            int ub = Math.Max(num1, num2);
            return (lb, ub);
        }

        internal string[] getDcActionKeys()
        {
            string[] actionKeys = new string[DSC.dcActions.Count];
            DSC.dcActions.Keys.CopyTo(actionKeys, DSC.ORIGIN_IDX);
            return actionKeys;
        }

        internal bool[] getDcActionTerminateVals()
        {
            bool[] terminateVals = new bool[DSC.dcActions.Count];
            DSC.dcActions.Values.CopyTo(terminateVals, DSC.ORIGIN_IDX);
            return terminateVals;
        }

        /// <summary>
        /// displays a list of every thing in _decisionSummary to console 
        /// and tries to save it to a file if _decisionSummary is non-empty
        /// </summary>
        /// <returns>whether the summary was saved to a file or not</returns>
        internal bool showAndSaveDecSummary()
        {
            string decisionsMade = TU.BLANK;
            if (!TU.isStringListEmpty(_decisionSummary))
            {
                decisionsMade = TU.getListAsNumberMenu(_decisionSummary);
                Console.WriteLine($"Decisions made this session:\n{decisionsMade}");
            }
            else
                Console.WriteLine(DSC.NO_DECISIONS_MSG);
            return trySaveDecSummaryFile(decisionsMade);
        }

        private bool trySaveDecSummaryFile(string decisionsList)
        {
            bool saved = false;
            if (!String.IsNullOrWhiteSpace(decisionsList))
            {
                try
                {
                    MU.checkAndInitADir(DSC.DEFAULT_LAST_SESSION_DIRECTORY);
                    string decisionsMadeFile = $"{DSC.DEC_SUMMARY_FILE_INTRO} {DateTime.Now}:\n{decisionsList}";
                    File.WriteAllText(DSC.DEFAULT_LAST_SESSION_FPATH, decisionsMadeFile);
                    saved = true;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{DSC.DS_INFO_INTRO} failed to save decisions summary file");
                    TU.logErrorMsg(e);
                }
            }
            return saved;
        }
    }
}