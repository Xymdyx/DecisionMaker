namespace DecisionMaker
{
    public class FilesSection:IDecisionMakerSection
    {
        private readonly string[] fileActions = {"Delete a decision category", "Delete all decision categories",
                                                "Clear greeting", "Clear exiting", "Clear display name", "Clear profile" };

        private void writeMenu()
        {
            // 1. implement decision category deletion
            // 2. implement profile clearing
            // 3. implement decision cateogry deletion restoration
            // 4. clear wipcat file
        }

        public int doMenuLoop(){return 0;}
        private void processMenuInput(int opt){}
        private void decideForUser(List<string> choices){}
        private int runRNG(){return 0;}
    }
}