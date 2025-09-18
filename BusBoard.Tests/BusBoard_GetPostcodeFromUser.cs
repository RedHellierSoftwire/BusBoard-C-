using Xunit;
using BusBoard.API;
using BusBoard.Controllers;
using BusBoard.Models;
using System.Collections.Immutable;

namespace BusBoard.Tests;

public class BusBoard_GetPostcodeFromUser
{
    private readonly UserInputController _userInput = new();

    [Theory]
    [InlineData("AA11 1AA")]
    [InlineData("AA1A 1AA")]
    [InlineData("AA1 1AA")]
    [InlineData("A1 1AA")]
    [InlineData("AA1A1AA")]
    [InlineData("AA11AA")]
    [InlineData("A11AA")]
    [InlineData("aa1a 1aa")]
    [InlineData("aa1 1aa")]
    [InlineData("a1 1aa")]
    [InlineData("aa11aa")]
    [InlineData("a11aa")]
    [InlineData("AA111aA")]
    [InlineData("AA1a1Aa")]
    public void GetPostcodeFromUser_ValidPostcodeInput_ReturnsFormattedPostcode(string input)
    {
        // Arrange
        MemoryStream inputStream = new();
        StreamWriter writer = new(inputStream);
        writer.WriteLine(input);
        writer.Flush();
        inputStream.Position = 0;
        using var reader = new StreamReader(inputStream);
        Console.SetIn(reader);

        // Act
        var result = _userInput.GetPostcodeFromUser();

        // Assert
        Assert.Equal(input.Replace(" ", "").ToUpper(), result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("b")]
    [InlineData("letters")]
    [InlineData("a b12")]
    [InlineData("cinco b13")]
    [InlineData("aa n")]
    [InlineData("alotofletters")]
    [InlineData("aa bc n12")]
    [InlineData("d f er w 3")]
    [InlineData("AAAA1AA")]
    public void GetPostcodeFromUser_InvalidPostcodeInput_ThrowsArgumentException(string input)
    {
        // Arrange
        MemoryStream inputStream = new();
        StreamWriter writer = new(inputStream);
        writer.WriteLine(input);
        writer.Flush();
        inputStream.Position = 0;
        using var reader = new StreamReader(inputStream);
        Console.SetIn(reader);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _userInput.GetPostcodeFromUser());
    }
}