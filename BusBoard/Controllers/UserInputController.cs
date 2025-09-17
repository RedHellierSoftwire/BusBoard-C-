namespace BusBoard.Controllers;

public class UserInputController
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

        if (postcode.Split(" ").Length > 2)
        {
            throw new ArgumentException("Postcode is not valid");
        }

        postcode = postcode.Replace(" ", "").ToUpper();

        if (string.IsNullOrWhiteSpace(postcode))
        {
            throw new ArgumentException("Postcode cannot be empty");
        }

        if (postcode.Length < 5 || postcode.Length > 7)
        {
            throw new ArgumentException("Postcode must be between 5 and 7 characters long");
        }

        return postcode;
    }
}