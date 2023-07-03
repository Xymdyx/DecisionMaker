/*
* author: Xymdyx
* desc: Section for saved decision categories and acting on them
* Dependent on its parent DecisionSection instance for insantiation
* note that DC == "Decision Category"
* date started: approx 7/2/2023
*/
namespace DecisionMaker
{
    internal class DcSection : IDecisionMakerSection
    {
        private DS _parentSect;

        internal DcSection()
        {
            this._parentSect = null!;
        }

        internal DcSection(DS decSect)
        {
            this._parentSect = decSect;
        }

        internal static bool checkAndInitDir(){ return DS.checkAndInitDir(); }
        internal int doMenuLoop(){ return 0; }

        // called by DecisionsSection once choice made in its menu
        // determined to be a saved dc
        internal int enterDcActionsMenu(DC selectedDc)
        {
            int dcOpt = MU.INVALID_OPT;
            bool doesTerminate = false;
            do
            {
                writeDcActionsMenu(selectedDc.CatName);
                dcOpt = MU.promptUserAndReturnOpt();
                doesTerminate = processDcActionsMenuInput(dcOpt, selectedDc);
                Console.WriteLine();
            } while (!MU.isChoiceMenuExit(dcOpt) && !doesTerminate);
            return dcOpt;
        }

        private void writeDcActionsMenu(string dc)
        {
            Console.WriteLine($"Here are the actions for the {dc} decision category: ");
            List<string> actionNames = getDcActionKeys().ToList();
            TU.writeListAsNumberMenu(actionNames);
            MU.printExitChoice();
        }

        /// <summary>
        /// responds to what the user inputted in the category actions menu
        /// </summary>
        /// <param name="opt">- the valid/invalid option a user inputted </param>
        /// <param name="selectedDc">- the category we're currently in </param>
        /// <returns>whether the categoryActions loop should terminate</returns>
        private bool processDcActionsMenuInput(int opt, DC selectedDC)
        {
            bool doesTerminate = false;
            if (isChoiceDcAction(opt))
                doesTerminate = processDcAction(opt, selectedDC);
            else if (MU.isChoiceMenuExit(opt))
            {
                Console.WriteLine(MU.MENU_EXIT_MSG);
                doesTerminate = true;
            }
            else
                MU.writeInvalidMsg();

            return doesTerminate;
        }

        internal bool isChoiceDcAction(int opt)
        {
            return (opt >= MU.MENU_START) && (opt <= DSC.dcActions.Count);
        }        

        /// <summary>
        /// process actions after choosing an existing category
        /// </summary>
        /// <param name="actionNum">- the number the user inputted...</param>
        /// <param name="selectedDc">- the existing chosen category... </param>
        /// <returns>- whether the chosen action should terminate the category menu loop</returns>
        private bool processDcAction(int actionNum, DC selectedDc)
        {
            bool confirmHalt = true;
            switch (actionNum)
            {
                case (int)DSC.DcActionCodes.Decide:
                    confirmHalt = _parentSect.decideForUser(selectedDc);
                    break;
                case (int)DSC.DcActionCodes.ReadChoices:
                    confirmHalt = readDcChoices(selectedDc);
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
        /// print all options in a categories file line-by-line
        /// </summary>
        /// <param name="dc"></param>
        /// <returns>whether it is safe to continue operation on dc</returns>
        private bool readDcChoices(DC dc)
        {
            if (!dc.hasChoices())
            {
                Console.WriteLine(DSC.NO_CHOICES_MSG);
                return false;
            }

            Console.WriteLine(DSC.READ_DC_MSG);
            Console.WriteLine(dc.stringifyChoicesToReadableLines());
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
                string newDesc = _parentSect.describeDc();
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
            if (!_parentSect.deleteAndRemoveDcFromMap(dc))
            {
                Console.WriteLine($"{DSC.DS_INFO_INTRO} Failed to delete old {dc.CatPath} file and remove {dc.CatName} from map...");
                return true;
            }

            Console.WriteLine($"Please rename the {dc.CatName} category (you may re-enter the same one)...");
            dc.CatName = _parentSect.nameDc();
            if (!_parentSect.saveAndAddDcToMap(dc))
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
            List<string> added = _parentSect.addChoicesToDc(dc);
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
                    return _parentSect.deleteAndRemoveDcFromMap(dc);
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

    }
}