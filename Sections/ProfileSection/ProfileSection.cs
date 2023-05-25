namespace DecisionMaker
{
    public class ProfileSection:IDecisionMakerSection
    {
        // need profile directory path
        private const string PROFILE_DEFAULT_PATH = ".\\ProfileStorage\\profile.txt";
        private const string DEFAULT_GREETING = "Hello there, friend!";
        private const string DEFAULT_EXIT_MSG = "Goodbye, friend. We hope you found what you were looking for!";
        public ProfileSection()
        {}
        private void writeMenu(){}
        public int doMenuLoop(){return 0;}
        private int promptUser(){return 0;}
        private void processMenuInput(int opt){}
        private void readExistingLists(){}
        private void addItemToList(){}
        private void decideForUser(List<string> choices){}
        private int runRNG(){return 0;}
        private void changeUsername(){}
        private void changeGreeting(){}
        private void changeExitMsg(){}
        private void modifyRNGRange(){}
        private void scanForConfigurations(){}
    }
}