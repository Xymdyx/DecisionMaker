/*
* desc: class for a Decision a user wants this application to help with, whether it be recurring or one-time.
* author: Sam Ford
* date: 6/13/23
*/

namespace DecisionMaker
{
    internal class DecisionCategory
    {
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
        internal DecisionCategory(string name, string desc)
        {
            _catName = name;
            _catDesc = desc;
            _catChoices = new();
            _catPath = DS.formatDcPath(name);
        }

        internal DecisionCategory(string name, string desc, List<string> choices)
        {
            _catName = name;
            _catDesc = desc;
            _catChoices = choices;
            _catPath = DS.formatDcPath(name);
            
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
            return _catChoices.Count > 0;
        }

        internal void showAllInfo()
        {
            Console.WriteLine("DC " + _catName + ": " + _catDesc +
                    "\n" + TU.prettyStringifyList(_catChoices) + "\n");
        }

        /// <summary>
        /// Return this DC's choices in form
        /// choice 1
        /// .
        /// .
        /// choice N
        /// </summary>
        /// <returns></returns>
        internal string stringifyChoices()
        {
            return String.Join(DSC.DECISION_DELIMITER, _catChoices);
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
