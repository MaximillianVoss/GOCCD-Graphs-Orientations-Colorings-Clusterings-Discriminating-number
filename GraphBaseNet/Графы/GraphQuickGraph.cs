using GraphBase.Параметры;
using QuickGraph;
using System.Collections.Generic;
using System.Linq;

namespace GraphBase.Графы
{
    public class GraphQuickGraph : Graph
    {
        // Конструкторы
        public GraphQuickGraph(int[,] adjacencyMatrix)
        {
            this._adjacencyMatrix = adjacencyMatrix;
        }

        private UndirectedGraph<int, Edge<int>> ConvertToQuickGraph()
        {
            var graph = new UndirectedGraph<int, Edge<int>>();

            int verticesCount = this._adjacencyMatrix.GetLength(0);
            for (int i = 0; i < verticesCount; i++)
            {
                _ = graph.AddVertex(i);
            }

            for (int i = 0; i < verticesCount; i++)
            {
                for (int j = i + 1; j < verticesCount; j++) // Начнем с j = i + 1, чтобы избежать двойного добавления рёбер
                {
                    if (this._adjacencyMatrix[i, j] != 0)
                    {
                        _ = graph.AddEdge(new Edge<int>(i, j));
                    }
                }
            }

            return graph;
        }


        public override int GetChromaticNumber()
        {
            UndirectedGraph<int, Edge<int>> graph = this.ConvertToQuickGraph();
            return this.GetChromaticNumber(graph);
        }

        public override int GetChromaticIndex()
        {
            UndirectedGraph<int, Edge<int>> graph = this.ConvertToQuickGraph();
            return this.GetChromaticIndex(graph);
        }

        public override int GetDistinguishingNumber(int maxColors)
        {
            // Реализация метода (по умолчанию возвращаем -1, если метод не реализован)
            return -1;
        }

        public override string GetInfo(int numberOfColors = 0)
        {
            // Получаем G6-представление графа
            string g6String = new G6String(new AdjacencyMatrix(this.AdjacencyMatrix)).G6;

            // Получаем хроматическое число и хроматический индекс
            int chromaticNumber = this.GetChromaticNumber();
            int chromaticIndex = this.GetChromaticIndex();

            // Формируем итоговую строку
            return $"G6-представление: {g6String}, Хроматическое число: {chromaticNumber}, Хроматический индекс: {chromaticIndex}";
        }

        public int GetChromaticNumber(UndirectedGraph<int, Edge<int>> graph)
        {
            var vertexColors = new Dictionary<int, int>();
            foreach (int vertex in graph.Vertices)
            {
                var availableColors = Enumerable.Range(0, graph.VertexCount).ToList();

                foreach (Edge<int> edge in graph.AdjacentEdges(vertex))
                {
                    if (vertexColors.TryGetValue(edge.Target, out int usedColor))
                    {
                        _ = availableColors.Remove(usedColor);
                    }
                }

                int colorToAssign = availableColors.FirstOrDefault();
                if (colorToAssign == 0 && availableColors.Count == 0)
                {
                    // Если нет доступных цветов, присваиваем новый цвет
                    colorToAssign = vertexColors.Values.DefaultIfEmpty().Max() + 1;
                }

                vertexColors[vertex] = colorToAssign;
            }

            return vertexColors.Values.Distinct().Count(); // Возвращаем количество уникальных цветов
        }

        public int GetChromaticIndex(UndirectedGraph<int, Edge<int>> graph)
        {
            var edgeColors = new Dictionary<Edge<int>, int>();
            int maxDegree = graph.Vertices.Max(vertex => graph.AdjacentEdges(vertex).Count());
            var allColors = Enumerable.Range(0, maxDegree + 1).ToList();

            foreach (Edge<int> edge in graph.Edges)
            {
                var usedColors = new HashSet<int>();

                foreach (Edge<int> adjacentEdge in graph.AdjacentEdges(edge.Source).Concat(graph.AdjacentEdges(edge.Target)))
                {
                    if (edgeColors.TryGetValue(adjacentEdge, out int color))
                    {
                        _ = usedColors.Add(color);
                    }
                }

                int availableColor = allColors.Except(usedColors).FirstOrDefault();
                if (availableColor == 0 && usedColors.Contains(0))
                {
                    // Если нет доступных цветов, присваиваем новый цвет
                    availableColor = edgeColors.Values.DefaultIfEmpty().Max() + 1;
                }

                edgeColors[edge] = availableColor;
            }

            return edgeColors.Values.Distinct().Count(); // Возвращаем количество уникальных цветов
        }

    }
}
