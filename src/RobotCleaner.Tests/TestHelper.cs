using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using RobotCleaner.Controllers;

namespace RobotCleaner.Tests;

public static class TestHelper
{
    public static async Task<ExecutionRequest> ReadFile(string name)
    {
        var textData = await File.ReadAllLinesAsync("./TestData/" + name);
        var commandsAmount = textData.First();
        var start = textData.Skip(1).First().Split(" ");

        var request = new ExecutionRequest
        {
            Commands = [],
            Start = new Start
            {
                X = int.Parse(start[0]),
                Y = int.Parse(start[1])
            }
        };

        List<Command> commands = [];
        foreach (var line in textData.Skip(2))
        {
            var split = line.Split(" ");
            var direction = GetDirection(split[0]);
            var steps = int.Parse(split[1]);
            commands.Add(new Command
            {
                Direction = direction,
                Steps = steps
            });
        }

        request.Commands = commands.ToArray();
        return request;

        string GetDirection(string shortDirection)
        {
            return shortDirection switch
            {
                "N" => "north",
                "E" => "east",
                "S" => "south",
                "W" => "west",
                _ => throw new ArgumentException("Invalid direction")
            };
        }
    }
}