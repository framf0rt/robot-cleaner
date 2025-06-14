using System.Diagnostics;
using System.Drawing;
using RobotCleaner.Controllers;

namespace RobotCleaner;

/// <summary>
///     The CommandRunner class is responsible for executing a set of cleaning instructions provided in the form of
///     commands.
///     It processes the instructions, calculates the cleaning results, and returns execution details including the number
///     of commands executed, the duration of execution, and the result of the cleaning operation.
/// </summary>
public class CommandRunner
{
    public static Executions Run(ExecutionRequest dataInput)
    {
        var stopWatch = new Stopwatch();
        stopWatch.Start();
        var result = CalculateCleaningDistance(dataInput);
        stopWatch.Stop();
        var seconds = stopWatch.ElapsedMilliseconds * 0.001;
        return new Executions
        {
            Commands = dataInput.Commands.Length,
            Duration = seconds,
            Result = result
        };
    }

    private static int CalculateCleaningDistance(ExecutionRequest dataInput)
    {
        if (dataInput.Commands.Length == 0) return 0;

        var prevPos = new Point(dataInput.Start.X, dataInput.Start.Y);
        var sPoints = new List<(Point, Point)>();
        foreach (var command in dataInput.Commands)
        {
            var newPosition = GetNewPosition(command.Direction, command.Steps, prevPos);
            sPoints.Add((prevPos, newPosition));
            prevPos = newPosition;
        }

        var lines = new HashSet<(Point p1, Point p2)>(new PointTupleComparer());
        sPoints.ForEach(x => lines.Add(x));

        var dist = lines.Sum(x => GetDistance(x.p1, x.p2));
        var (horizontalLines, verticalLines) = SplitLines(lines.ToList());

        var intersections = new HashSet<Point>(); // Track unique intersection points

        var overlap = 0;
        foreach (var verticalLine in verticalLines)
            foreach (var horizontalLine in horizontalLines)
                if (DoLinesIntersect(horizontalLine, verticalLine))
                {
                    var intersectionPoint = new Point(horizontalLine.y, verticalLine.x);
                    intersections.Add(intersectionPoint);
                    overlap++;
                }

        overlap -= intersections.Count;
        return dist + 1 - overlap;
    }

    private static bool DoLinesIntersect(
        (int y, int x1, int x2) horizontalLine, // (y-coord, start-x, end-x)
        (int x, int y1, int y2) verticalLine) // (x-coord, start-y, end-y)
    {
        return verticalLine.x >= horizontalLine.x1 &&
               verticalLine.x <= horizontalLine.x2 &&
               horizontalLine.y >= verticalLine.y1 &&
               horizontalLine.y <= verticalLine.y2;
    }

    private static (List<(int y, int x1, int x2)> horizontal, List<(int x, int y1, int y2)> vertical) SplitLines(
        List<(Point p1, Point p2)> lines)
    {
        var horizontalLines = new List<(int y, int x1, int x2)>();
        var verticalLines = new List<(int x, int y1, int y2)>();

        foreach (var line in lines)
            if (line.p1.Y == line.p2.Y) // Horizontal line
            {
                // Ensure x1 is the smaller x coordinate
                var x1 = Math.Min(line.p1.X, line.p2.X);
                var x2 = Math.Max(line.p1.X, line.p2.X);
                horizontalLines.Add((line.p1.Y, x1, x2));
            }
            else if (line.p1.X == line.p2.X) // Vertical line
            {
                // Ensure y1 is the smaller y coordinate
                var y1 = Math.Min(line.p1.Y, line.p2.Y);
                var y2 = Math.Max(line.p1.Y, line.p2.Y);
                verticalLines.Add((line.p1.X, y1, y2));
            }

        return (horizontalLines, verticalLines);
    }

    private static int GetDistance(Point p1, Point p2)
    {
        var dist = Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
        return (int)dist;
    }

    /// <summary>
    ///     Gets the position, by copying the previous position and modifying X/Y
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="steps"></param>
    /// <param name="prevPos"></param>
    /// <returns>A new Point, with either updated X or Y value</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private static Point GetNewPosition(string direction, int steps, Point prevPos)
    {
        return direction switch
        {
            "north" => prevPos with { Y = prevPos.Y + steps },
            "east" => prevPos with { X = prevPos.X + steps },
            "south" => prevPos with { Y = prevPos.Y - steps },
            "west" => prevPos with { X = prevPos.X - steps },
            _ => throw new ArgumentOutOfRangeException(nameof(direction),
                $"Not expected direction value: {direction}")
        };
    }


    private class PointTupleComparer : IEqualityComparer<(Point p1, Point p2)>
    {
        public bool Equals((Point p1, Point p2) x, (Point p1, Point p2) y)
        {
            return (x.p1.Equals(y.p1) && x.p2.Equals(y.p2)) ||
                   (x.p1.Equals(y.p2) && x.p2.Equals(y.p1));
        }

        public int GetHashCode((Point p1, Point p2) obj)
        {
            var (minPoint, maxPoint) = obj.p1.X < obj.p2.X ||
                                       (obj.p1.X == obj.p2.X && obj.p1.Y <= obj.p2.Y)
                ? (obj.p1, obj.p2)
                : (obj.p2, obj.p1);

            return HashCode.Combine(minPoint.X, minPoint.Y, maxPoint.X, maxPoint.Y);
        }
    }
}