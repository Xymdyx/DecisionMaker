/*
* desc: class for a Decision a user wants this application to help with, whether it be recurring or one-time.
* author: Sam Ford
* date: 6/13/23
*/

namespace DecisionMaker
{
    internal class DecisionCategory
    {
        internal const int CHOICES_PER_MANY_LINE = 10;
        private const int MANY_CHOICES_THRESHOLD = 80;
        internal static readonly DecisionCategory EmptyDc = new(TU.BLANK, TU.BLANK);
        internal const string DC_SUMMARY_TAG = "(saved category)";
        internal const string ONE_OFF_SUMMARY_TAG = "(one-off decision)";
        private const string DC_INFO_HEADER = "DecisionCategory.cs:";
        private string _catName;
        private string _catDesc;
        private List<string> _catChoices;
        private string _catPath;

        internal string CatName
        {
            get {return _catName; }
            set
            {
                _catName = value;
                _catPath = DS.formatDcPath(value);
            }
        }
        internal string CatDesc { get => _catDesc; set => _catDesc = value; }
        internal List<string> CatChoices { get => _catChoices; set => _catChoices = value; }
        internal string CatPath { get => _catPath; }

        internal DecisionCategory()
        {
            _catName = TU.BLANK;
            _catDesc = TU.BLANK;
            _catChoices = new();
            _catPath = TU.BLANK;
        }

        internal DecisionCategory(string name, string desc)
        {
            _catName = name ?? TU.BLANK;
            _catDesc = desc ?? TU.BLANK;
            _catChoices = new();
            _catPath = DS.formatDcPath(_catName);
        }

        internal DecisionCategory(string name, string desc, List<string> choices)
        {
            _catName = name ?? TU.BLANK;
            _catDesc = desc ?? TU.BLANK;
            _catChoices = choices ?? new();
            _catPath = DS.formatDcPath(_catName);

            if(!checkFileExists())
                saveFile();
        }

        internal bool saveFile()
        {
            bool saved = false;
            try
            {
                DS.checkAndInitDir();
                File.WriteAllText(_catPath, _catName + DSC.DECISION_DELIMITER);
                File.AppendAllText(_catPath, _catDesc + DSC.DECISION_DELIMITER);
                File.AppendAllLines(_catPath, _catChoices);
                Console.WriteLine($"Saved file {_catPath}!");
                saved = true;
            }
            catch(Exception e)
            {
                Console.WriteLine($"{DC_INFO_HEADER} failed to save file {_catPath}...");
                TU.logErrorMsg(e);
            }
            return saved;
        }

        internal bool deleteFile()
        {
            bool deleted = false;
            try
            {
                File.Delete(_catPath);
                Console.WriteLine($"Deleted file {_catPath}!");
                deleted = true;
            }
            catch(Exception e)
            {
                Console.WriteLine($"{DC_INFO_HEADER} failed to delete file ${_catPath}...");
                TU.logErrorMsg(e);
            }
            return deleted;
        }

        internal bool checkFileExists()
        {
            return File.Exists(_catPath);
        }


        internal bool hasChoices()
        {
            return getChoicesCount() > 0;
        }

        internal int getChoicesCount()
        {
            return _catChoices.Count;
        }        

        internal void showAllInfo()
        {
            Console.WriteLine("DC " + _catName + ": " + _catDesc +
                    "\n" + TU.prettyStringifyList(_catChoices) + "\n");
        }

        internal string stringifyToReadableForm()
        {
            string bestForm = stringifyChoicesOnALine();
            bool isCommasLong = TU.isStringTooLong(bestForm);
            if(isCommasLong)
                bestForm = stringifyChoicesToReadableLines();

            return bestForm;
        }

        internal string stringifyChoicesToReadableLines()
        {
            return (getChoicesCount() >= MANY_CHOICES_THRESHOLD) ? stringifyManyChoicesPerLine() : stringifyChoicePerLine();
        }

        /// Return a string with each choice on its own line
        internal string stringifyChoicePerLine()
        {
            return String.Join(DSC.DECISION_DELIMITER, _catChoices);
        }

        // returns choices as comma separated list
        internal string stringifyChoicesOnALine()
        {
            string choicesInfo = (hasChoices()) ? $"Choices for {CatName}: {TU.prettyStringifyList(CatChoices)}\n" : $"No choices in {CatName}\n";
            return choicesInfo;
        }

        /// prints many choices as comma separated list per line
        internal string stringifyManyChoicesPerLine()
        {
            string choicesInfo = TU.BLANK;
            int choiceCount = getChoicesCount();
            for (int i = 0; i < choiceCount; i+=CHOICES_PER_MANY_LINE)
            {
                int secLen = Math.Min(CHOICES_PER_MANY_LINE, choiceCount - i);
                List<string> choicesSection = CatChoices.GetRange(i, secLen);
                choicesInfo += (TU.prettyStringifyList(choicesSection) );
                choicesInfo += (i + CHOICES_PER_MANY_LINE < choiceCount) ? ",\n" : "\n";
            }
            return choicesInfo;
        }        

        internal bool IsValidDc()
        {
            return TU.isInputAcceptable(_catDesc) && TU.isInputAcceptable(_catName);
        }

        internal bool IsNotEmptyDc()
        {
            return !this.Equals(EmptyDc);
        }

        public override string ToString()
        {
            return _catName + ": " + _catDesc;
        }
    }
}
