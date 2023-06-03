/*
* description: personal project for helping me decide what to do with my free time
* author: Sam Ford
* date started: 4/4/2023
*/
using System;
namespace DecisionMaker
{
    public class DecisionMakerMain : IDecisionMakerSection
    {
        private ProfileSection profileSect;
        private DecisionsSection decisionsSect;
        private Personality personality;

        private readonly string[] navigationMenu = { "Decisions menu", "Profile menu", "File management menu", "Help"};
        
        public DecisionMakerMain()
        {
            this.profileSect = new();
            this.decisionsSect = new();
            this.personality = profileSect.appPersonality;
        }

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
        
        private int doMenuLoop()
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
                case 1:
                    Console.WriteLine("Entering decision menu...");
                    this.decisionsSect.doMenuLoop();
                    break;
                case 2:
                    Console.WriteLine("Coming next...");
                    profileSect.doMenuLoop();
                    break;
                case 3:
                    Console.WriteLine("Coming later...");
                    // send to fileManagement();
                    break;
                case 4:
                    Console.WriteLine("Coming last...");
                    // print help message
                    break;
                case MenuUtils.EXIT_CODE:
                    Console.WriteLine("Exiting app!");
                    break;
                default:
                    Console.WriteLine("Unrecognized command, please try again");
                    break;
            }
        }
    }
}