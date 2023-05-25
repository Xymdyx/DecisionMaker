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
        private void parseCommandLine(){}

        private int doMenuLoop()
        {
            int choice = -1;
            do
            {
                writeMenu();
                choice = MenuUtils.promptUser();
            }while(choice != MenuUtils.EXIT_CODE);
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
                    // send to profileMenu()
                    break;
                case 3:
                    // send to fileManagement();
                    break;
                case 4:
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

        private void readExistingLists(){}
        private void addItemToList(){}
        private void decideForUser(List<string> options){}
        private int runRNG(){return 0;}
    }
}