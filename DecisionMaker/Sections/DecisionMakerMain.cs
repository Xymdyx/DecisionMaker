/*
* description: personal project for helping me decide what to do with my free time
* author: Sam Ford
* date started: 4/4/2023
*/

namespace DecisionMaker
{
    internal class DecisionMakerMain : IDecisionMakerSection
    {
        private ProfileSection profileSect;
        private DecisionsSection decisionsSect;
        private Personality personality;
        private FilesSection fileSect;
        private HelpSection helpSect;

        private readonly string[] navigationMenu = { "Decisions menu", "Profile menu", "File management menu", "Help"};
        
        public DecisionMakerMain()
        {
            this.profileSect = new();
            this.decisionsSect = new();
            this.personality = profileSect.appPersonality;
            this.fileSect = new(this.decisionsSect, this.profileSect);
            this.helpSect = new();
        }

        internal static bool checkAndInitDir() { return false; }

        public int main(string[] argv)
        {
            greet();
            doMenuLoop();
            decisionsSect.showAndSaveDecSummary();
            depart();
            fileSect.saveFilesBeforeExit();
            return 0;
        }

        private void greet()
        {
            Console.WriteLine(this.personality.mainGreeting);
            if(this.personality.isDisplayNameCustom())
                Console.WriteLine($"Welcome back, {this.personality.displayName}!");
        }
        
        internal int doMenuLoop()
        {
            int opt = MU.INVALID_OPT;
            do
            {
                writeMenu();
                opt = MenuUtils.promptUserAndReturnOpt();
                processMenuInput(opt);
            }while(!MU.isChoiceMenuExit(opt));
            return 0;
        }

        private void depart()
        {
            Console.WriteLine(this.personality.mainExit);
            if(this.personality.isDisplayNameCustom())
                Console.WriteLine($"Until next time, {this.personality.displayName}!");

            Console.WriteLine();
        }    

        private void writeMenu()
        {
            TextUtils.writeListAsNumberMenu(navigationMenu.ToList());
            MenuUtils.printExitChoice();
        }

        private void processMenuInput(int opt)
        {
            switch(opt)
            {
                case (int) DSE.DCs:
                    this.decisionsSect.doMenuLoop();
                    break;
                case (int) DSE.Profile:
                    profileSect.doMenuLoop();
                    break;
                case (int) DSE.FileManagement:
                    fileSect.doMenuLoop();
                    break;
                case (int) DSE.Help:
                    helpSect.doMenuLoop();
                    break;
                case MenuUtils.EXIT_CODE:
                    break;
                default:
                    MenuUtils.writeInvalidMsg();
                    break;
            }
        }
    }
}