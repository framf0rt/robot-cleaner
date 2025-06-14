using System.Text.Json.Serialization;

namespace RobotCleaner.Controllers;

public class ExecutionRequest
{
    [JsonPropertyName("start")] public required Start Start { get; init; }
    [JsonPropertyName("commands")] public required Command[] Commands { get; set; }
}

public record Start
{
    [JsonPropertyName("x")] public int X { get; init; }
    [JsonPropertyName("y")] public int Y { get; init; }
}

public class Command
{
    [JsonPropertyName("direction")] public required string Direction { get; init; }


    [JsonPropertyName("steps")] public int Steps { get; set; }
}