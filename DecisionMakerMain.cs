/*
* description: personal project for helping me decide what to do with my free time
* author: Sam Ford
* date started: 4/4/2023
*/
using System;
namespace DecisionMaker{
    public class DecisionMakerMain : IDecisionMakerSection
    {
        private ProfileSection profileSect;
        private DecisionsSection decisionsSect;
        private const int EXIT_CODE = 5;

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
                choice = promptUser();
            }while(choice != EXIT_CODE);
            return 0;
        }

        private void writeMenu()
        {
            Console.Write(
            "1. Category menu\n" +
            "2. Profile menu\n" +
            "3. File management menu\n" +
            "4. Help\n" +
            "5. Exit\n");
        }
        private int promptUser()
        {
            Console.WriteLine("Please enter a valid number: ");
            string input = Console.ReadLine()!;
            int opt = getUserInput(input);
            processMenuInput(opt);
            return opt;
        }

        private int getUserInput(string input)
        {
            int opt = -1;
            try
            {
                opt = System.Int32.Parse(input);
            }
            catch(Exception e) 
            {
                Console.Error.WriteLine($"Cannot convert input to integer: {e}");
            }
            return opt;
        }

        private void processMenuInput(int opt)
        {
            switch(opt)
            {
                case 1:
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
                case EXIT_CODE:
                    Console.WriteLine("exiting");
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