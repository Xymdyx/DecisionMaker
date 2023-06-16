/*
* description: personal project for helping me decide what to do with my free time
* author: Sam Ford
* date started: 4/4/2023
*/
using System;
using DSE = DecisionMaker.DecisionMakerSects;
namespace DecisionMaker
{
    public class DecisionMakerMain : IDecisionMakerSection
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

        public static bool checkAndInitDir() { return false; }

        public int main(string[] argv)
        {
            greet();
            doMenuLoop();
            depart();
            return 0;
        }

        private void greet()
        {
            Console.WriteLine(this.personality.mainGreeting);
            if(this.personality.isDisplayNameCustom())
                Console.WriteLine($"Welcome back, {this.personality.displayName}!");
        }
        
        public int doMenuLoop()
        {
            int opt = -1;
            do
            {
                writeMenu();
                opt = MenuUtils.promptUser();
                processMenuInput(opt);
            }while(opt != MenuUtils.EXIT_CODE);
            return 0;
        }

        private void depart()
        {
            Console.WriteLine(this.personality.mainExit);
            if(this.personality.isDisplayNameCustom())
                Console.WriteLine($"Until next time, {this.personality.displayName}!");
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
                case (int) DecisionMakerSects.DCs:
                    this.decisionsSect.doMenuLoop();
                    break;
                case (int) DecisionMakerSects.Profile:
                    profileSect.doMenuLoop();
                    break;
                case (int) DecisionMakerSects.FileManagement:
                    fileSect.doMenuLoop();
                    break;
                case (int) DecisionMakerSects.Help:
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