namespace DecisionMaker
{
    public interface IDecisionMakerSection
    {
        private void writeMenu(){}
        public int doMenuLoop(){return 0;}
        private void processMenuInput(int opt){}
        private void decideForUser(List<string> choices){}
        private int runRNG(){return 0;}
    }
}