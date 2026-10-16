using System;
using System.Threading;
using Frends.PowerShell.RunScript.Definitions;
using NUnit.Framework;

namespace Frends.PowerShell.RunScript.Tests;

[TestFixture]
public class ErrorHandlerTest
{
    private const string CustomErrorMessage = "CustomErrorMessage";

    private static Input InvalidInput() => new()
    {
        ReadFromFile = false,
        Script = "throw 'Script failed'",
    };

    [Test]
    public void Should_Throw_Error_When_ThrowErrorOnFailure_Is_True()
    {
        var ex = Assert.Throws<Exception>(() =>
            PowerShell.RunScript(InvalidInput(), new Options(), CancellationToken.None));
        Assert.That(ex, Is.Not.Null);
    }

    [Test]
    public void Should_Return_Failed_Result_When_ThrowErrorOnFailure_Is_False()
    {
        var result = PowerShell.RunScript(
            InvalidInput(),
            new Options { ThrowErrorOnFailure = false },
            CancellationToken.None);
        Assert.That(result.Success, Is.False);
        Assert.That(result.Error.Message, Does.Contain("Script failed"));
    }

    [Test]
    public void Should_Use_Custom_ErrorMessageOnFailure()
    {
        var ex = Assert.Throws<Exception>(() =>
            PowerShell.RunScript(
                InvalidInput(),
                new Options { ErrorMessageOnFailure = CustomErrorMessage },
                CancellationToken.None));
        Assert.That(ex.Message, Does.Contain(CustomErrorMessage));
    }
}
