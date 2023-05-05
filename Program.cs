// See https://aka.ms/new-console-template for more information
using DecisionMaker;

class Program{
    public static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        DecisionMakerMain mainSect = new();
        mainSect.main(args);
    }
}