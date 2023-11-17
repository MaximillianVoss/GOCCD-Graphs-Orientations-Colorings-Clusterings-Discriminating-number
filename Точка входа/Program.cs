using GraphBase.Графы;
using GraphBase.Генераторы;
using System;
using System.Diagnostics;
using System.IO;
using GraphBase.Параметры;

class Program
{
    static void Main(string[] args)
    {
        bool DEBUG = false; // Значение флага DEBUG

        try
        {
            Console.Write("Введите количество вершин для генерации графов: ");
            int vertexCount = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Выберите метод генерации графов:");
            Console.WriteLine("1 - Генерация с помощью вектора степеней");
            Console.WriteLine("2 - Генерация с помощью канонического кода");
            int methodChoice = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Выберите тип графа:");
            Console.WriteLine("1 - Пользовательский");
            Console.WriteLine("2 - QuickGraph");
            int graphTypeChoice = Convert.ToInt32(Console.ReadLine());

            var generator = new GeneratorNaughty(vertexCount);
            string generationMethod = methodChoice == 1 ? "Вектор степеней" : "Канонический код";
            string graphType = graphTypeChoice == 1 ? "Пользовательский" : "QuickGraph";

            var reportFileName = $"Отчет граф {vertexCount} вершин - {generationMethod} - {graphType}.txt";

            using (var reportFile = new StreamWriter(reportFileName))
            {
                var totalStopwatch = Stopwatch.StartNew();
                int graphNumber = 1; // Счетчик номера графа

                foreach (var g6 in generator.GenerateAllGraphsG6(vertexCount, GeneratorType.GENERATOR_BY_CANONICAL_CODE))
                {
                    try
                    {
                        AdjacencyMatrix adjacencyMatrix = new G6String(g6).ToAdjacencyMatrix();
                        Graph graph = null;

                        switch (methodChoice)
                        {
                            case 1:
                                DegreeVector degreeVector = adjacencyMatrix.ToDegreeVector();
                                graph = graphTypeChoice == 1 ? new GraphCustom(degreeVector) : new GraphQuickGraph(adjacencyMatrix.Matrix);
                                break;
                            case 2:
                                CanonicalGraphCode canonicalGraphCode = adjacencyMatrix.ToCanonicalGraphCode();
                                graph = graphTypeChoice == 1 ? new GraphCustom(canonicalGraphCode) : new GraphQuickGraph(adjacencyMatrix.Matrix);
                                break;
                            default:
                                throw new InvalidOperationException("Неизвестный метод генерации графов.");
                        }

                        // Выводим номер графа перед информацией
                        Console.Write($"Граф #{graphNumber}:");
                        reportFile.Write($"Граф #{graphNumber}:");

                        var graphInfo = graph.GetInfo();
                        Console.WriteLine(graphInfo);
                        reportFile.WriteLine(graphInfo);

                        graphNumber++; // Увеличиваем счетчик номера графа
                    }
                    catch (Exception ex)
                    {
                        if (DEBUG)
                        {
                            Console.WriteLine($"Ошибка при обработке графа: {ex.Message}");
                            reportFile.WriteLine($"Ошибка при обработке графа: {ex.Message}");
                        }
                    }
                }

                totalStopwatch.Stop();
                var totalTimeInfo = $"Общее время выполнения: {totalStopwatch.ElapsedMilliseconds} мс";

                Console.WriteLine(totalTimeInfo);
                reportFile.WriteLine(totalTimeInfo);
            }
        }
        catch (Exception ex)
        {
            if (DEBUG)
            {
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
            }
        }
    }
}
