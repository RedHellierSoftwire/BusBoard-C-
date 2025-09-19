using System;

namespace BusBoard.Web.Models;

public class PostcodeInput
{
    public string Postcode { get; set; }

    public PostcodeInput(string? postcode)
    {
        Postcode = postcode ?? "";
    }
}
