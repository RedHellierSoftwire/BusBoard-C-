using System.Text.RegularExpressions;

namespace BusBoard.Controllers;

public partial class UserInputController
{
    public string GetStringInputFromUser(string prompt)
    {
        Console.Write(prompt);
        string postcode = Console.ReadLine()!;
        return postcode;
    }

    public int GetIntInputFromUser(string prompt)
    {
        Console.Write(prompt);
        try
        {
            int integer = int.Parse(Console.ReadLine()!);
            return integer;
        }
        catch (FormatException)
        {
            throw new FormatException("Input was not a valid integer");
        }
    }

    public string GetPostcodeFromUser()
    {
        string postcode = GetStringInputFromUser("Enter Postcode: ");

        if (string.IsNullOrWhiteSpace(postcode))
        {
            throw new ArgumentException("Postcode cannot be empty");
        }

        Regex postcodeRegex = MyRegex();

        postcode = postcode.Replace(" ", "").ToUpper();

        if (!postcodeRegex.IsMatch(postcode))
        {
            throw new ArgumentException("Postcode is not valid");
        }

        if (postcode.Length < 5 || postcode.Length > 7)
        {
            throw new ArgumentException("Postcode must be between 5 and 7 characters long");
        }

        return postcode;
    }

    [GeneratedRegex("^([A-Z][A-HJ-Y]?[0-9][A-Z0-9]? ?[0-9][A-Z]{2}|GIR ?0A{2})$", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex MyRegex();
}