/*
* desc: class for a Decision a user wants this application to help with, whether it be recurring or one-time.
* author: Sam Ford
* date: 6/13/23
*/
public class DecisionCategory
{
    private string _catName;
    private string _catDesc;
    private List<string> _catChoices;

    public string CatName {get => _catName; set => _catName = value;}
    public string CatDesc { get => _catDesc; set => _catDesc = value; }
    public string CatChoices { get => _catDesc; }

    DecisionCategory(string name, string desc, List<string> choices)
    {
        _catName = name;
        _catDesc = desc;
        _catChoices = choices;
    }
}