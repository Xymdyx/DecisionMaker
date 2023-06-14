/*
* desc: class for a Decision a user wants this application to help with, whether it be recurring or one-time.
* author: Sam Ford
* date: 6/13/23
*/

using DS = DecisionMaker.DecisionsSection;
using TU = DecisionMaker.TextUtils;
namespace DecisionMaker
{
    public class DecisionCategory
    {
        private const string DC_INFO_HEADER = "DecisionCategory.cs:";
        private string _catName;
        private string _catDesc;
        private List<string> _catChoices;
        private string _catPath;

        public string CatName
        {
            get {return _catName; }
            set
            {
                _catName = value;
                _catPath = DS.formatDCPath(value);
            }
        }
        public string CatDesc { get => _catDesc; set => _catDesc = value; }
        public List<string> CatChoices { get => _catChoices; set => _catChoices = value; }

        public DecisionCategory(string name, string desc, List<string> choices)
        {
            _catName = name;
            _catDesc = desc;
            _catChoices = choices;
            _catPath = DS.formatDCPath(name);
            saveFile();
        }

        public bool saveFile()
        {
            try
            {
                File.WriteAllText(_catPath, _catName + DS.DECISION_DELIMITER);
                File.AppendAllText(_catPath, _catDesc + DS.DECISION_DELIMITER);
                File.AppendAllLines(_catPath, _catChoices);
                Console.WriteLine($"Saved file {_catPath}!");
                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine($"{DC_INFO_HEADER} failed to save file {_catPath}...\n {e}");
                return false;
            }
        }

        public bool deleteFile()
        {
            try
            {
                File.Delete(_catPath);
                Console.WriteLine($"Deleted file {_catPath}!");
                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine($"{DC_INFO_HEADER} failed to delete file ${_catPath}...\n {e}");
                return false;
            }        
        }

        public bool checkFileExists()
        {
            return File.Exists(_catPath);
        }

        public bool hasChoices()
        {
            return checkFileExists() && _catChoices.Count > 0;
        }

        public void printAllInfo()
        {
            Console.WriteLine("DC " + _catName + ": " + _catDesc +
                    "\n" + TU.prettyStringifyList(_catChoices) + "\n");
        }
        public override string ToString()
        {
            return _catName + ": " + _catDesc;
        }
    }
}
