using System.Globalization;
using System.Threading.Tasks;
using RobotCleaner.Controllers;
using Xunit;
using Xunit.Abstractions;

namespace RobotCleaner.Tests;

public class CommandRunnerTest(ITestOutputHelper testOutputHelper)
{
    /// <summary>
    ///     Sanity check, if this fails something is crazy
    /// </summary>
    [Fact]
    public void Cleans_one_step_north()
    {
        var commandRequest = new ExecutionRequest
        {
            Commands =
            [
                new Command
                {
                    Direction = "north",
                    Steps = 1
                }
            ],
            Start = new Start
            {
                X = 0,
                Y = 0
            }
        };
        var commandRunner = new CommandRunner();
        var result = CommandRunner.Run(commandRequest);
        Assert.Equal(1, result.Commands);
        Assert.Equal(2, result.Result);
    }

    [Theory]
    [InlineData(10_000, 21810, "backforththensmall-21810.txt")]
    [InlineData(1812, 21810, "backforththensmall-21810-modified.txt")]
    [InlineData(6, 600_001, "visitcorners-600001.txt")]
    [InlineData(10_000, 500_005_001, "visithuge-500005001.txt")]
    [InlineData(10_000, 493_757_501, "crossing-493757501.txt")]
    [InlineData(0, 0, "noinstr.txt")]
    [InlineData(10_000, 666_800_001, "visitmuch-666800001.txt")]
    public async Task Run_Test_With_Files(int commands, int expectedResult, string fileName)
    {
        var commandRequest = await TestHelper.ReadFile(fileName);
        var commandRunner = new CommandRunner();
        var result = CommandRunner.Run(commandRequest);
        // In your test or wherever you have access to the ExecutionRequest
        Assert.Equal(commands, result.Commands);
        Assert.Equal(expectedResult, result.Result);
        testOutputHelper.WriteLine(result.Duration.ToString(CultureInfo.InvariantCulture));
    }


}