using Xunit;
using BusBoard.Controllers;

namespace BusBoard.Tests;

public class BusBoard_GetPostcodeFromUser
{
    [Theory]
    [InlineData("AA11 1AA","AA111AA")]
    [InlineData("AA1A 1AA","AA1A1AA")]
    [InlineData("AA1 1AA","AA11AA")]
    [InlineData("A1 1AA","A11AA")]
    [InlineData("AA1A1AA","AA1A1AA")]
    [InlineData("AA11AA","AA11AA")]
    [InlineData("A11AA","A11AA")]
    [InlineData("aa1a 1aa","AA1A1AA")]
    [InlineData("aa1 1aa","AA11AA")]
    [InlineData("a1 1aa","AA111AA")]
    [InlineData("aa11aa","AA111AA")]
    [InlineData("a11aa","A11AA")]
    [InlineData("AA111aA","AA111AA")]
    [InlineData("AA1a1Aa","AA1A1AA")]
    public void GetPostcodeFromUser_ValidPostcodeInput_ReturnsFormattedPostcode(string input, string expectedOutput)
    {
        // Act
        var result = UserInputController.ValidatePostcodeFromUser(input);

        // Assert
        Assert.Equal(expectedOutput, result);
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
        // Act & Assert
        Assert.Throws<ArgumentException>(() => UserInputController.ValidatePostcodeFromUser(input));
    }
}