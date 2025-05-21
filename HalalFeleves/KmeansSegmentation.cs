using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace HalalFeleves
{
    static class KmeansSegmentation
    {
        public static void run()
        {
            Console.WriteLine("K-means klaszterezéses képszegmentáció");
            Console.WriteLine("-------------------------------------");

            string imagePath = "weeknd.jpg";
            int k = 5; // Klaszterek száma
            int maxIterations = 100;

            if (!File.Exists(imagePath))
            {
                Console.WriteLine($"A fájl nem található: {imagePath}");
                Console.WriteLine("Kérlek add meg a képfájl elérési útját: ");
                imagePath = Console.ReadLine();
            }

            try
            {
                using (Bitmap originalImage = new Bitmap(imagePath))
                {
                    Console.WriteLine($"Kép mérete: {originalImage.Width}x{originalImage.Height}");
                    Console.WriteLine($"Szegmentálás {k} klaszterrel...");

                    Bitmap segmentedImage = SegmentImage(originalImage, k, maxIterations);

                    string outputPath = Path.GetFileNameWithoutExtension(imagePath) + $"_segmented_k{k}.png";
                    segmentedImage.Save(outputPath, ImageFormat.Png);
                    Console.WriteLine($"Szegmentált kép mentve: {outputPath}");

                    segmentedImage.Dispose();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hiba történt: {ex.Message}");
            }

            Console.WriteLine("Nyomj egy gombot a kilépéshez...");
            Console.ReadKey();
        }

        static Bitmap SegmentImage(Bitmap image, int k, int maxIterations)
        {
            int width = image.Width;
            int height = image.Height;

            List<Point3D> points = new List<Point3D>(width * height);
            List<int> clusterAssignments = new List<int>(width * height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color pixel = image.GetPixel(x, y);
                    points.Add(new Point3D(pixel.R, pixel.G, pixel.B, x, y));
                    clusterAssignments.Add(-1);
                }
            }

            List<Point3D> centroids = InitializeCentroids(points, k);
            bool changed = true;
            int iteration = 0;

            while (changed && iteration < maxIterations)
            {
                changed = false;
                iteration++;

                Parallel.For(0, points.Count, i =>
                {
                    int nearestCentroidIndex = FindNearestCentroid(points[i], centroids);

                    lock (clusterAssignments)
                    {
                        if (clusterAssignments[i] != nearestCentroidIndex)
                        {
                            clusterAssignments[i] = nearestCentroidIndex;
                            changed = true;
                        }
                    }
                });

                if (changed)
                {
                    UpdateCentroids(points, clusterAssignments, centroids, k);
                }

                Console.WriteLine($"Iteráció {iteration}: {(changed ? "változott" : "nem változott")}");
            }

            Bitmap result = new Bitmap(width, height);

            Color[] clusterColors = new Color[k];
            for (int i = 0; i < k; i++)
            {
                clusterColors[i] = Color.FromArgb(
                    (int)centroids[i].X,
                    (int)centroids[i].Y,
                    (int)centroids[i].Z
                );
            }

            for (int i = 0; i < points.Count; i++)
            {
                int x = points[i].OriginalX;
                int y = points[i].OriginalY;
                int cluster = clusterAssignments[i];

                result.SetPixel(x, y, clusterColors[cluster]);
            }

            return result;
        }

        static List<Point3D> InitializeCentroids(List<Point3D> points, int k)
        {
            List<Point3D> centroids = new List<Point3D>();
            Random random = new Random();

            int firstIndex = random.Next(points.Count);
            centroids.Add(new Point3D(points[firstIndex]));

            for (int i = 1; i < k; i++)
            {
                double[] distances = new double[points.Count];
                double sum = 0;

                Parallel.For(0, points.Count, j =>
                {
                    double minDist = double.MaxValue;
                    foreach (var centroid in centroids)
                    {
                        double dist = Point3D.CalculateDistance(points[j], centroid);
                        minDist = Math.Min(minDist, dist);
                    }

                    lock (distances)
                    {
                        distances[j] = minDist * minDist;
                        sum += distances[j];
                    }
                });

                double threshold = random.NextDouble() * sum;
                double cumulativeSum = 0;
                int selectedIndex = 0;

                for (int j = 0; j < points.Count; j++)
                {
                    cumulativeSum += distances[j];
                    if (cumulativeSum >= threshold)
                    {
                        selectedIndex = j;
                        break;
                    }
                }

                centroids.Add(new Point3D(points[selectedIndex]));
            }

            return centroids;
        }

        static int FindNearestCentroid(Point3D point, List<Point3D> centroids)
        {
            int nearestIndex = 0;
            double minDistance = double.MaxValue;

            for (int i = 0; i < centroids.Count; i++)
            {
                double distance = Point3D.CalculateDistance(point, centroids[i]);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestIndex = i;
                }
            }

            return nearestIndex;
        }

        static void UpdateCentroids(List<Point3D> points, List<int> clusterAssignments, List<Point3D> centroids, int k)
        {
            double[] sumX = new double[k];
            double[] sumY = new double[k];
            double[] sumZ = new double[k];
            int[] counts = new int[k];

            for (int i = 0; i < points.Count; i++)
            {
                int cluster = clusterAssignments[i];
                sumX[cluster] += points[i].X;
                sumY[cluster] += points[i].Y;
                sumZ[cluster] += points[i].Z;
                counts[cluster]++;
            }

            for (int i = 0; i < k; i++)
            {
                if (counts[i] > 0)
                {
                    centroids[i].X = sumX[i] / counts[i];
                    centroids[i].Y = sumY[i] / counts[i];
                    centroids[i].Z = sumZ[i] / counts[i];
                }
            }
        }
    }

    class Point3D
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public int OriginalX { get; }
        public int OriginalY { get; }

        public Point3D(double x, double y, double z, int originalX = -1, int originalY = -1)
        {
            X = x;
            Y = y;
            Z = z;
            OriginalX = originalX;
            OriginalY = originalY;
        }

        public Point3D(Point3D other)
        {
            X = other.X;
            Y = other.Y;
            Z = other.Z;
            OriginalX = other.OriginalX;
            OriginalY = other.OriginalY;
        }

        public static double CalculateDistance(Point3D p1, Point3D p2)
        {
            // Euklideszi távolság az RGB térben
            double dx = p1.X - p2.X;
            double dy = p1.Y - p2.Y;
            double dz = p1.Z - p2.Z;

            return Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }
    }
}
