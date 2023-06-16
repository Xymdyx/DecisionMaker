namespace DecisionMaker
{
    public interface IDecisionMakerSection
    {
        public static bool checkAndInitDir() { return false; }
        public int doMenuLoop(){return 0;}
    }
}