using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RobotCleaner;

public class ExecutionDbContext(DbContextOptions<ExecutionDbContext> options) : DbContext(options)
{
    public DbSet<Executions> Results { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<Executions>()
            .Property(e => e.Timestamp)
            .HasDefaultValueSql("now()");
    }
}

public class Executions
{
    [Key] public int ID { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime Timestamp { get; private init; }

    public int Commands { get; set; }
    public int Result { get; set; }
    public double Duration { get; set; }
}