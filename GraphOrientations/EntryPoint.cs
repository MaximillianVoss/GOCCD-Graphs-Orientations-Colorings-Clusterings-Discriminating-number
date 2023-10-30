using CommandLine;
using GraphOrientations.Generator;
using GraphOrientations.Writers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GraphOrientations
{
    internal class EntryPoint
    {
        /// <summary>
        /// Лимит графов
        /// </summary>
        private static int Limit = 1000;
        private static string outputFileNameCommon;
        private static StreamWriter fileWriter;

        public static void Main(string[] args)
        {
            var consoleArguments = new Options();
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(o =>
                {
                    var startTime = DateTime.Now;
                    var orientator = new GraphOrientator();
                    var generator = new GeneratorBase();
                    IWriter writer = o.WriteGraphsToFile ? new FileWriterCustom(o.FileName) : new ConsoleWriter();
                    int vertexCount = o.VertexCount;
                    int colorsCount = o.ColorsCount;
                    Console.WriteLine($"Число вершин: {vertexCount}");
                    Console.WriteLine($"число цветов: {colorsCount}");
                    outputFileNameCommon = $"раскраски_вершины_{vertexCount}_цвета_{colorsCount}.txt";

                    using (fileWriter = new StreamWriter(outputFileNameCommon))
                    {
                        fileWriter.Flush();
                        int totalCount = 0;
                        int id = 0;
                        var resultOutput = new List<string>();
                        var automorphismReader = new AutomorphismGroupRepository();
                        var groupSizeToCount = new Dictionary<int, (int graphsCount, int OrientationsTotalCount)>();
                        var graphs = generator.GenerateGraphs(vertexCount, GeneratorType.GENERATOR_BY_CANONICAL_CODE);
                        if (graphs.Count() > Limit)
                            graphs = graphs.Take(Limit).ToList();
                        var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };


                        Parallel.ForEach(graphs, parallelOptions, g6Graph =>
                        {
                            var graph = new Graph(g6Graph);
                            int groupSize = automorphismReader.GetNextAutomorphismGroupSize(g6Graph);

                            if (o.CalculateOnly)
                            {
                                int[] orientResult = orientator.OrientWithoutGraphs(g6Graph, o.NautyCalculation).ToArray();
                                ProcessGraphCalculations(Interlocked.Increment(ref id), g6Graph, groupSize, orientResult, colorsCount, groupSizeToCount, ref totalCount);

                            }
                            else
                            {
                                writer.WriteLine($"Current graph: {g6Graph}; Group size: {groupSize}");
                                writer.WriteLine();

                                foreach (var (oriented, _) in orientator.Orient(graph.AdjacencyMatrix))
                                {
                                    for (int i = 0; i < vertexCount; i++)
                                    {
                                        for (int j = 0, jMask = 1; j < vertexCount; j++, jMask <<= 1)
                                        {
                                            writer.Write((oriented[i] & jMask) == 0 ? 0 : 1);
                                        }
                                        writer.WriteLine();
                                    }
                                    writer.WriteLine();
                                    Interlocked.Increment(ref totalCount);
                                }
                                writer.WriteLine();
                            }
                        });

                        if (o.CalculateOnly)
                        {
                            DisplayResults(totalCount, startTime, groupSizeToCount, o.VertexCount);
                            SaveResultsToFile(o.VertexCount, resultOutput, groupSizeToCount);
                        }

                        Console.WriteLine($"Общее количество графов: {totalCount}..");
                        Console.WriteLine($"Общее время вычислений в секундах: {(DateTime.Now - startTime).TotalSeconds}.");

                    }

                });
            Console.WriteLine("Для продолжения нажмите любую клавишу");
            Console.ReadLine();
        }

        private static void ProcessGraphCalculations(int id, string g6Graph, int groupSize, int[] orientResult, int colorsCount, Dictionary<int, (int graphsCount, int OrientationsTotalCount)> groupSizeToCount, ref int totalCount)
        {
            int currentOrientationsCount = orientResult.Length;
            int graphsCountSavingGroupSize = orientResult.Count(x => x == groupSize);
            double averageGroupSize = (double)orientResult.Sum() / orientResult.Length;

            totalCount += currentOrientationsCount;

            if (groupSizeToCount.TryGetValue(groupSize, out var value))
            {
                groupSizeToCount[groupSize] = (value.graphsCount + 1, value.OrientationsTotalCount + currentOrientationsCount);
            }
            else
            {
                groupSizeToCount.Add(groupSize, (1, currentOrientationsCount));
            }
            var coloringsCount = GraphColoring.ChromaticPolynomial(g6Graph, colorsCount);
            //РазмерГруппы,КоличествоРаскрасок,КоличествоОриентаций,КоличествоГрафовССохранениемРазмераГруппы,СреднийРазмерГруппы
            string format = "Номер: {0,10}; Граф: {1,8}; РГ: {2,8}; КР: {3,8}; КО: {4,8}; КГсРГ: {5,8}; СРГ: {6,8:#.####}";
            string output = string.Format(format, id, g6Graph, groupSize, coloringsCount, currentOrientationsCount, graphsCountSavingGroupSize, averageGroupSize);
            WriteToFile(output);
            Console.WriteLine(output);
        }

        private static void WriteToFile(string text)
        {
            fileWriter.WriteLine(text);
        }

        private static void DisplayResults(int totalCount, DateTime startTime, Dictionary<int, (int graphsCount, int OrientationsTotalCount)> groupSizeToCount, int vertexCount)
        {
            Console.WriteLine($"Общее количество ориентаций {totalCount}.");
            Console.WriteLine($"Количество вершин в графе: {vertexCount}");
            Console.WriteLine("Среднее количество ориентаций в зависимости от размера группы:");

            const int paddingGroupSize = 20;
            const int paddingOrientationCount = 20;

            Console.WriteLine($"Размер группы".PadRight(paddingGroupSize) + $"Количество ориентаций".PadRight(paddingOrientationCount));
            foreach (var kvp in groupSizeToCount.OrderBy(x => x.Key))
            {
                Console.WriteLine($"{kvp.Key.ToString().PadRight(paddingGroupSize)}{(double)kvp.Value.OrientationsTotalCount / kvp.Value.graphsCount:#.##}".PadRight(paddingOrientationCount));
            }

        }

        private static void SaveResultsToFile(int n, List<string> resultOutput, Dictionary<int, (int graphsCount, int OrientationsTotalCount)> groupSizeToCount)
        {
            File.WriteAllLines($"Result_{n}.txt", resultOutput);

            List<string> graphsCountByGroupSize = new List<string>();
            List<string> orientationsAverageByGroupSize = new List<string>();
            foreach (var kvp in groupSizeToCount.OrderBy(x => x.Key))
            {
                graphsCountByGroupSize.Add($"({kvp.Key};{kvp.Value.OrientationsTotalCount})");
                orientationsAverageByGroupSize.Add($"({kvp.Key};{(double)kvp.Value.OrientationsTotalCount / kvp.Value.graphsCount})");
            }

            File.WriteAllText($"GraphsCountByGroupSize_{n}.txt", string.Join(" ", graphsCountByGroupSize));
            File.WriteAllText($"OrientationsAverageByGroupSize_{n}.txt", string.Join(" ", orientationsAverageByGroupSize));
        }
    }
}
