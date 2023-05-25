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
        
        public DecisionMakerMain()
        {
            this.profileSect = new();
            this.decisionsSect = new();
        }

        public int main(string[] argv)
        {
            Console.WriteLine("Holey moley here we go again...");
            Console.WriteLine("We will ask you what you want a decision for shortly...");
            doMenuLoop();
            Console.WriteLine("Thanks for consulting us!");
            return 0;
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

        private void writeMenu()
        {
            Console.Write(
            "1. Decisions menu\n" +
            "2. Profile menu\n" +
            "3. File management menu\n" +
            "4. Help\n");
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
                    Console.WriteLine("Exiting DecisionMaker!");
                    break;
                default:
                    Console.WriteLine("Unrecognized command, please try again");
                    break;
            }
        }
    }
}