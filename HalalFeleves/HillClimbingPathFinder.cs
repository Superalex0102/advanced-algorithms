using System;
using System.Collections.Generic;
using System.Linq;

namespace HalalFeleves
{
    static class HillClimbingPathFinder
    {
        public static void run()
        {
            // Labirintus reprezentáció (0 = szabad út, 1 = fal)
            int[,] labyrinth = new int[,]
            {
                { 0, 1, 0, 0, 0 },
                { 0, 0, 0, 1, 0 },
                { 0, 1, 0, 1, 0 },
                { 1, 1, 0, 1, 0 },
                { 0, 0, 0, 0, 0 }
            };

            Point start = new Point(0, 0);
            Point goal = new Point(4, 4);

            var path = HillClimbing(labyrinth, start, goal);

            if (path != null)
            {
                Console.WriteLine("Megtalált útvonal:");
                foreach (var point in path)
                {
                    Console.WriteLine($"({point.X}, {point.Y})");
                }

                VisualizeLabyrinth(labyrinth, path);
            }
            else
            {
                Console.WriteLine("Nem található útvonal a célhoz.");
            }

            Console.ReadKey();
        }

        public static List<Point> HillClimbing(int[,] labyrinth, Point start, Point goal)
        {
            List<Point> currentPath = new List<Point> { start };
            HashSet<Point> visited = new HashSet<Point> { start };
            Random random = new Random();

            int maxIterations = 1000;
            int iterations = 0;

            while (iterations < maxIterations)
            {
                iterations++;

                Point current = currentPath.Last();

                if (current.Equals(goal))
                {
                    return currentPath;
                }

                List<Point> neighbors = GetNeighbors(current, labyrinth, visited);

                if (neighbors.Count == 0)
                {
                    if (currentPath.Count <= 1)
                    {
                        return null;
                    }

                    currentPath.RemoveAt(currentPath.Count - 1);
                    continue;
                }

                Point bestNeighbor = neighbors
                    .OrderBy(p => ManhattanDistance(p, goal))
                    .First();

                if (random.NextDouble() < 0.1)
                {
                    bestNeighbor = neighbors[random.Next(neighbors.Count)];
                }

                currentPath.Add(bestNeighbor);
                visited.Add(bestNeighbor);
            }

            return null;
        }

        private static int ManhattanDistance(Point a, Point b)
        {
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        }

        private static List<Point> GetNeighbors(Point point, int[,] labyrinth, HashSet<Point> visited)
        {
            int width = labyrinth.GetLength(1);
            int height = labyrinth.GetLength(0);

            List<Point> neighbors = new List<Point>();

            // Lehetséges mozgási irányok: fel, jobbra, le, balra
            int[] dx = { 0, 1, 0, -1 };
            int[] dy = { -1, 0, 1, 0 };

            for (int i = 0; i < 4; i++)
            {
                int newX = point.X + dx[i];
                int newY = point.Y + dy[i];

                Point newPoint = new Point(newX, newY);

                if (newX >= 0 && newX < width &&
                    newY >= 0 && newY < height &&
                    labyrinth[newY, newX] == 0 &&
                    !visited.Contains(newPoint))
                {
                    neighbors.Add(newPoint);
                }
            }

            return neighbors;
        }

        private static void VisualizeLabyrinth(int[,] labyrinth, List<Point> path)
        {
            int height = labyrinth.GetLength(0);
            int width = labyrinth.GetLength(1);

            Console.WriteLine("\nLabirintus:");

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Point current = new Point(x, y);

                    if (path.Contains(current))
                    {
                        // Útvonal része
                        Console.Write("* ");
                    }
                    else if (labyrinth[y, x] == 1)
                    {
                        // Fal
                        Console.Write("# ");
                    }
                    else
                    {
                        // Szabad mező
                        Console.Write(". ");
                    }
                }
                Console.WriteLine();
            }
        }
    }

    public class Point
    {
        public int X { get; }
        public int Y { get; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object obj)
        {
            if (obj is Point other)
            {
                return X == other.X && Y == other.Y;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }
    }
}
