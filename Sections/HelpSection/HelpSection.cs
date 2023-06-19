/*
* description: Section for the help menu, which tells the user more about the app
* author: Sam Ford
* date: 6/13/23
*/
using MU = DecisionMaker.MenuUtils;
using TU = DecisionMaker.TextUtils;
using HSC = DecisionMaker.HelpSectConstants;
namespace DecisionMaker
{
    public class HelpSection:IDecisionMakerSection
    {
        public static bool checkAndInitDir() { return false; }

        public int doMenuLoop()
        {
            int opt = MU.INVALID_OPT;
            Console.WriteLine(HSC.HELP_SECTION_GREETING);
            do
            {
                writeMenu();
                opt = MU.promptUser();
                processMenuInput(opt);
            } while (!MU.isChoiceMenuExit(opt));
            return opt;
        }

        private void writeMenu()
        {
            TU.writeListAsNumberMenu(HSC.mainTopics.ToList());
            MU.printExitChoice();
        }

        private void processMenuInput(int opt)
        {
            switch(opt)
            {
                case (int) HSC.HelpMenuTopics.Overview:
                    TU.writeInfoAndPause(HSC.OVERVIEW_INFO);
                    break;
                case (int) HSC.HelpMenuTopics.Navigation:
                    TU.writeInfoAndPause(HSC.NAV_INFO);
                    break;                    
                case (int) HSC.HelpMenuTopics.GeneratedFiles:
                    TU.writeInfoAndPause(HSC.GENERATED_FILES_INFO);
                    break;
                case MU.EXIT_CODE:
                    break;
                default:
                    MU.writeInvalidMsg();
                    break;
            }
        }
    }
}