/*
* author: Sam Ford
* desc: Section for creating DCs and nonDcActions.
* Holds all DCs saved to computer
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
        private bool _hasAddressedWipCat;
        internal DcSection DcSect { get; private set; }
        internal Dictionary<string, DC> DcMap { get => _dcMap; }
        internal List<string> DecisionSummary { get => _decisionSummary; }

        internal DecisionsSection()
        {
            this.rng = new();
            this._dcMap = new();
            this._decisionSummary = new();
            this._hasAddressedWipCat = false;
            this.DcSect = new(this);
            
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
            List<string> existing = scanForDcs();
            foreach (string cat in existing.Where(c => !_dcMap.ContainsKey(c)))
            {
                try
                {
                    string catPath = formatDcPath(cat);
                    DC dc = makeDcFromDcFile(catPath);
                    _dcMap.TryAdd(cat, dc);
                }
                catch(Exception e)
                {
                    Console.WriteLine($"{DSC.DS_INFO_INTRO} failed to add saved {cat} decision category from {DSC.DEFAULT_DC_DIRECTORY} dir to program");
                    TU.logErrorMsg(e);
                }
            }
        }

        private DC makeDcFromDcFile(string dcPath)
        {
            DC dc = DC.EmptyDc;
            try
            {
                List<string> catLines = File.ReadAllLines(dcPath).ToList();
                string dcName = catLines[DSC.DC_NAME_LINE_IDX];
                string catDesc = catLines[DSC.DC_DESC_LINE_IDX];
                List<string> catChoices = getDcChoicesFromFileLines(catLines);
                dc = new(dcName, catDesc, catChoices);
            }
            catch(Exception e)
            {
                Console.WriteLine($"{DSC.DS_INFO_INTRO} failed to construct decision category from {dcPath} file. File possibly in incorrect format...");
                TU.logErrorMsg(e);
            }
            return dc;
        }

        private List<string> getDcChoicesFromFileLines(List<string> catLines)
        {
            List<string> allChoices = (catLines != null) ? catLines.Skip(DSC.INFO_LEN).ToList() : new();
            allChoices.RemoveAll(c => !TU.isInputAcceptable(c));
            return allChoices;
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
                Console.WriteLine($"{DSC.DS_INFO_INTRO} Error scanning for categories...");
                TU.logErrorMsg(e);
                files.Clear();
            }
            return files;
        }

        /// <summary>
        /// the entry loop for the Decision Section
        /// </summary>
        /// <returns></returns>
        internal int doMenuLoop()
        {
            Console.WriteLine(DSC.DECISIONS_WELCOME_MSG);
            tryAddressDcInWipFile();
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
            {
                DC selected = getDcFromMenuChoice(opt);
                DcSect.enterDcActionsMenu(selected);
            }
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
        internal void announceSavedDcs()
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
            DC dc = DC.EmptyDc;
            try
            {
                string dcName = nameDc();
                string dcDesc = describeDc();
                dc = new(dcName, dcDesc);
                dc.CatChoices = addChoicesToDc(dc);
                return dc;
            }
            catch (Exception e)
            {
                logErrorAndSaveWipDc(e, dc);
            }
            return DC.EmptyDc;
        }

        internal void logErrorAndSaveWipDc(Exception e, DC dc)
        {
            Console.WriteLine($"{DSC.DS_INFO_INTRO} Failed to make decision category. Saving any made progress...");
            TU.logErrorMsg(e);
            saveUnfinishedDcToWipCat(dc);            
        }

        internal bool saveUnfinishedDcToWipCat(DC dc)
        {
            if (TU.isInputAcceptable(dc.CatName))
            {
                try
                {
                    FS.checkAndInitDir();
                    File.WriteAllText(FSC.DEFAULT_WIP_FILE, dc.CatName + DSC.DECISION_DELIMITER);
                    File.AppendAllText(FSC.DEFAULT_WIP_FILE, dc.CatDesc + DSC.DECISION_DELIMITER);
                    File.AppendAllLines(FSC.DEFAULT_WIP_FILE, dc.CatChoices);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{DSC.DS_INFO_INTRO} failure in saving wipcat file.");
                    TU.logErrorMsg(e);
                }
            }
            return File.Exists(FSC.DEFAULT_WIP_FILE);
        }

        internal string nameDc()
        {
            announceSavedDcs();
            string dcName = TU.BLANK;
            do
            {
                Console.WriteLine(DSC.NAME_DC_MSG);
                dcName = Console.ReadLine()!.Trim();
            } while (!TU.isInputAcceptable(dcName) || doesMapHaveDcName(dcName));

            return dcName;
        }

        internal bool doesMapHaveDcName(string dcName)
        {
            return _dcMap.Keys.Contains(dcName);
        }

        internal string describeDc()
        {
            string dcDesc = TU.BLANK;
            do
            {
                Console.WriteLine(DSC.DESCRIBE_DC_MSG);
                dcDesc = Console.ReadLine()!.Trim();
            } while (!TU.isInputAcceptable(dcDesc));
            return dcDesc;
        }

        internal List<string> addChoicesToDc(DC selectedDc)
        {
            List<string> acceptedChoices = selectedDc.CatChoices;
            string choiceInput = TU.BLANK;
            bool stopWanted = false;
            do
            {
                printAddDcChoiceLoopInstructions(acceptedChoices);
                choiceInput = Console.ReadLine()!.Trim();
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
        /// completely remove DC from dc map and dc dir
        /// </summary>
        /// <param name="dc"></param>
        /// <returns>bool - whether the file was completely removed from the program</returns>
        internal bool deleteAndRemoveDcFromMap(DC dc)
        {
            return _dcMap.Remove(dc.CatName) && dc.deleteFile();
        }

        internal int runRNG(int num1, int num2)
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

        /// <summary>
        /// called only if WipCat in FileManagement is a partially saved DC saved previously by this file
        /// This is done in this section's entrance menu so a user has the chance to delete the file first
        /// </summary>
        /// <returns>bool - if a valid decision category was successfully made from the wip file</returns>
        internal bool tryAddressDcInWipFile()
        {
            DC wipDc = DC.EmptyDc;
            if(!_hasAddressedWipCat && confirmCompleteWipDcOnce())
            {
                wipDc = tryToMakeDcFromWipFile();
                FS.tryDeleteWipFile();
                decideWipDcFate(wipDc);
            }
            return wipDc.IsValidDc();
        }

        // confirm if the user wants to complete a wipcat only once
        private bool confirmCompleteWipDcOnce()
        {
            bool confirmed = false;
            if(FS.isWipFileNonEmpty())
            {
                Console.WriteLine($"There is a WIP decision category in FileManagement, would you like to finish it {DSC.ONLY_1_CONFIRM}? ");
                MU.writeBinaryMenu();
                int opt = MU.promptUserAndReturnOpt();
                confirmed = MU.isChoiceYes(opt);
                _hasAddressedWipCat = true;
                if(!confirmed)
                    Console.WriteLine(FSC.HOW_FIND_WIP);
            }
            return confirmed;
        }

        // helper to addressDcInWipfile, meant to continue where a user left off in making a DC
        private DC tryToMakeDcFromWipFile()
        {
            DC wipDc = DC.EmptyDc;
            if (FS.isWipFileNonEmpty())
            {
                try
                {
                    List<string> dcLines = File.ReadAllLines(FSC.DEFAULT_WIP_FILE).ToList();
                    string dcName = (TU.doesStringListHaveNonBlankEl(DSC.DC_NAME_LINE_IDX, dcLines)) ? dcLines[DSC.DC_NAME_LINE_IDX] : nameDc();

                    Console.WriteLine($"WIP DC name: {dcName}");
                    string dcDesc = (TU.doesStringListHaveNonBlankEl(DSC.DC_DESC_LINE_IDX, dcLines)) ? dcLines[DSC.DC_DESC_LINE_IDX] : describeDc();

                    wipDc = new(dcName, dcDesc);
                    wipDc.CatChoices = getDcChoicesFromFileLines(dcLines);
                    if(!wipDc.hasChoices())
                        addChoicesToDc(wipDc);
                }
                catch(Exception e)
                {
                    logErrorAndSaveWipDc(e, wipDc);
                }
            }
            return wipDc;
        }

        // helper that lets user decides whether to save a finished dc from wipcat or not
        private bool decideWipDcFate(DC wipDc)
        {
            Console.WriteLine($"\nWIP {wipDc.CatName} Decision category completed. Confirm if you want to save it for future use {DSC.ONLY_1_CONFIRM}: ");
            MU.writeBinaryMenu();
            int opt = MU.promptUserAndReturnOpt();
            if(MU.isChoiceYes(opt))
                return saveAndAddDcToMap(wipDc);

            return decideForUser(wipDc);
        }
    }
}