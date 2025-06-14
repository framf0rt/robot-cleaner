using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace RobotCleaner.Controllers;

[Route("developer-test/")]
public class CommandController(ExecutionDbContext executionDbContext) : ControllerBase
{
    [HttpPost("execute")]
    public async Task<IActionResult> Clean([FromBody] ExecutionRequest request)
    {
        var commandRunner = new CommandRunner();
        var result = CommandRunner.Run(request);
        await executionDbContext.Database.EnsureCreatedAsync();
        var entry = await executionDbContext.Results.AddAsync(result);
        await executionDbContext.SaveChangesAsync();
        return Ok(entry.Entity);
    }

    [HttpGet("executions")]
    public async Task<IActionResult> Cleaned()
    {
        await executionDbContext.Database.EnsureCreatedAsync();
        var result = await executionDbContext.Results.ToListAsync();
        return Ok(result);
    }
}