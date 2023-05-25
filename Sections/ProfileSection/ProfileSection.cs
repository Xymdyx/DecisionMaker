namespace DecisionMaker
{
    public class ProfileSection:IDecisionMakerSection
    {
        // need profile directory path
        private const string PROFILE_DEFAULT_PATH = ".\\ProfileStorage\\profile.txt";
        private const string DEFAULT_GREETING = "Hello there, friend!";
        private const string PROFILE_MENU_GREETING = "Welcome to the Profile Menu. This is where you can customize this program's configurable messages!";
        private const string DEFAULT_EXIT_MSG = "Goodbye, friend. We hope you found what you were looking for!";
        public ProfileSection()
        {
            scanForConfigurations();
        }
        public int doMenuLoop()
        {
            Console.WriteLine(PROFILE_MENU_GREETING);
            int opt = MenuUtils.INVALID_OPT;
            do
            {
                writeMenu();
                MenuUtils.printExitChoice();
                opt = MenuUtils.promptUser();
                processMenuInput(opt);
            } while (MenuUtils.isChoiceMenuExit(opt));
            return 0;
        }
        private void writeMenu()
        {
            /* TODO: 5/24/23
            1. choose which component to customize
            2. choose which part of the selected component to modify
            3. Confirm change with a Binary confirmation
            4. Store customizations in txts and read from them at initialization
            */
        }
        private void processMenuInput(int opt){}
        private void readExistingLists(){}
        private void addItemToList(){}
        private void decideForUser(List<string> choices){}
        private int runRNG(){return 0;}
        private void changeUsername(){}
        private void changeGreeting(){}
        private void changeExitMsg(){}
        private void scanForConfigurations(){}
    }
}