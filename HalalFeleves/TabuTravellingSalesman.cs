using System;
using System.Collections.Generic;
using System.Linq;

namespace HalalFeleves
{
    static class TabuTravellingSalesman
    {
        public static void run()
        {
            Console.WriteLine("Utazóügynök probléma megoldása Tabu kereséssel");

            // Példa városok koordinátákkal (x, y)
            var cities = new List<City>
            {
                new City(0, 60, 200),
                new City(1, 180, 200),
                new City(2, 80, 180),
                new City(3, 140, 180),
                new City(4, 20, 160),
                new City(5, 100, 160),
                new City(6, 200, 160),
                new City(7, 140, 140),
                new City(8, 40, 120),
                new City(9, 100, 120),
                new City(10, 180, 100),
                new City(11, 60, 80),
                new City(12, 120, 80),
                new City(13, 180, 60),
                new City(14, 20, 40),
                new City(15, 100, 40),
                new City(16, 200, 40),
                new City(17, 20, 20),
                new City(18, 60, 20),
                new City(19, 160, 20)
            };

            // Tabu keresés paraméterek
            int tabuListSize = 20;              // A tabu lista mérete
            int maxIterations = 1000;           // Maximális iterációk száma
            int maxIterationsWithoutImprovement = 100;  // Leállási feltétel - ha ennyi iteráción keresztül nincs javulás

            var solver = new TabuSearchTSP(cities, tabuListSize, maxIterations, maxIterationsWithoutImprovement);
            var solution = solver.Solve();

            Console.WriteLine("\nLegjobb megoldás:");
            Console.WriteLine($"Útvonal: {string.Join(" -> ", solution.Route.Select(c => c.Id))}");
            Console.WriteLine($"Teljes távolság: {solution.TotalDistance}");

            Console.ReadKey();
        }

        public class City
        {
            public int Id { get; }
            public double X { get; }
            public double Y { get; }

            public City(int id, double x, double y)
            {
                Id = id;
                X = x;
                Y = y;
            }

            public double DistanceTo(City other)
            {
                return Math.Sqrt(Math.Pow(X - other.X, 2) + Math.Pow(Y - other.Y, 2));
            }
        }

        public class Solution
        {
            public List<City> Route { get; }
            public double TotalDistance { get; }

            public Solution(List<City> route)
            {
                Route = new List<City>(route);
                TotalDistance = CalculateTotalDistance();
            }

            private double CalculateTotalDistance()
            {
                double distance = 0;
                for (int i = 0; i < Route.Count - 1; i++)
                {
                    distance += Route[i].DistanceTo(Route[i + 1]);
                }
                // Visszatérés a kezdőpontba
                distance += Route[Route.Count - 1].DistanceTo(Route[0]);
                return distance;
            }

            public Solution Swap(int i, int j)
            {
                var newRoute = new List<City>(Route);
                var temp = newRoute[i];
                newRoute[i] = newRoute[j];
                newRoute[j] = temp;
                return new Solution(newRoute);
            }

            public override bool Equals(object obj)
            {
                if (obj is Solution other)
                {
                    if (Route.Count != other.Route.Count)
                        return false;

                    for (int i = 0; i < Route.Count; i++)
                    {
                        if (Route[i].Id != other.Route[i].Id)
                            return false;
                    }
                    return true;
                }
                return false;
            }

            public override int GetHashCode()
            {
                return Route.Select(c => c.Id).Aggregate(17, (acc, id) => acc * 31 + id);
            }
        }

        public class TabuMove
        {
            public int City1Index { get; }
            public int City2Index { get; }

            public TabuMove(int city1Index, int city2Index)
            {
                City1Index = city1Index;
                City2Index = city2Index;
            }

            public override bool Equals(object obj)
            {
                if (obj is TabuMove other)
                {
                    return (City1Index == other.City1Index && City2Index == other.City2Index) ||
                           (City1Index == other.City2Index && City2Index == other.City1Index);
                }
                return false;
            }

            public override int GetHashCode()
            {
                return City1Index + City2Index;
            }
        }

        public class TabuSearchTSP
        {
            private List<City> _cities;
            private List<TabuMove> _tabuList;
            private int _tabuListSize;
            private int _maxIterations;
            private int _maxIterationsWithoutImprovement;
            private Random _random;

            public TabuSearchTSP(List<City> cities, int tabuListSize = 20, int maxIterations = 1000, int maxIterationsWithoutImprovement = 100)
            {
                _cities = cities;
                _tabuListSize = tabuListSize;
                _maxIterations = maxIterations;
                _maxIterationsWithoutImprovement = maxIterationsWithoutImprovement;
                _tabuList = new List<TabuMove>();
                _random = new Random();
            }

            public Solution Solve()
            {
                var initialRoute = GenerateInitialSolution();
                var currentSolution = new Solution(initialRoute);
                var bestSolution = currentSolution;

                Console.WriteLine($"Kezdeti megoldás távolsága: {currentSolution.TotalDistance}");

                int iteration = 0;
                int iterationsWithoutImprovement = 0;

                while (iteration < _maxIterations && iterationsWithoutImprovement < _maxIterationsWithoutImprovement)
                {
                    var bestNeighbor = FindBestNeighbor(currentSolution);

                    if (bestNeighbor == null)
                    {
                        Console.WriteLine("Nem találtam szomszédos megoldást, megszakítom a keresést.");
                        break;
                    }

                    currentSolution = bestNeighbor.Solution;

                    _tabuList.Add(bestNeighbor.Move);
                    if (_tabuList.Count > _tabuListSize)
                    {
                        _tabuList.RemoveAt(0);
                    }

                    if (currentSolution.TotalDistance < bestSolution.TotalDistance)
                    {
                        bestSolution = currentSolution;
                        iterationsWithoutImprovement = 0;
                        Console.WriteLine($"Javulás az {iteration}. iterációban: {bestSolution.TotalDistance}");
                    }
                    else
                    {
                        iterationsWithoutImprovement++;
                    }

                    iteration++;
                }

                Console.WriteLine($"\nA keresés {iteration} iteráció után befejeződött.");
                return bestSolution;
            }

            private List<City> GenerateInitialSolution()
            {
                var route = new List<City>(_cities);

                for (int i = route.Count - 1; i > 0; i--)
                {
                    int j = _random.Next(i + 1);
                    var temp = route[i];
                    route[i] = route[j];
                    route[j] = temp;
                }

                return route;
            }

            private class NeighborSolution
            {
                public Solution Solution { get; }
                public TabuMove Move { get; }

                public NeighborSolution(Solution solution, TabuMove move)
                {
                    Solution = solution;
                    Move = move;
                }
            }

            private NeighborSolution FindBestNeighbor(Solution currentSolution)
            {
                NeighborSolution bestNeighbor = null;
                double bestDistance = double.MaxValue;

                for (int i = 0; i < currentSolution.Route.Count - 1; i++)
                {
                    for (int j = i + 1; j < currentSolution.Route.Count; j++)
                    {
                        var move = new TabuMove(i, j);

                        bool isTabu = _tabuList.Any(m => m.Equals(move));

                        var neighborSolution = currentSolution.Swap(i, j);

                        if (!isTabu || neighborSolution.TotalDistance < bestDistance)
                        {
                            if (neighborSolution.TotalDistance < bestDistance)
                            {
                                bestDistance = neighborSolution.TotalDistance;
                                bestNeighbor = new NeighborSolution(neighborSolution, move);
                            }
                        }
                    }
                }

                return bestNeighbor;
            }
        }
    }
}
