using System;
using System.Linq;

namespace GraphOrientations
{
    public class GraphColoring
    {
        public static int[,] G6ToAdjacencyMatrix(string g6)
        {
            int n = ((int)Math.Sqrt((8 * g6.Length) + 1) - 1) >> 1;
            int[,] matrix = new int[n, n];

            int k = 0;
            for (int i = 1; i < n; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    int code = g6[k++] - 63;
                    if ((code & (1 << j)) != 0)
                    {
                        matrix[i, j] = matrix[j, i] = 1;
                    }
                }
            }
            return matrix;
        }

        public static int[] ColorGraph(string g6, int colors)
        {
            int[,] adjacencyMatrix = G6ToAdjacencyMatrix(g6);
            int vertexCount = adjacencyMatrix.GetLength(0);
            int[] coloring = new int[vertexCount];

            for (int i = 0; i < vertexCount; i++)
            {
                coloring[i] = -1;
            }

            coloring[0] = 0;

            bool[] availableColors = new bool[colors];
            for (int i = 0; i < colors; i++)
            {
                availableColors[i] = false;
            }

            for (int vertex = 1; vertex < vertexCount; vertex++)
            {
                for (int adjacentVertex = 0; adjacentVertex < vertexCount; adjacentVertex++)
                {
                    if (adjacencyMatrix[vertex, adjacentVertex] == 1 && coloring[adjacentVertex] != -1)
                    {
                        availableColors[coloring[adjacentVertex]] = true;
                    }
                }

                int color;
                for (color = 0; color < colors; color++)
                {
                    if (!availableColors[color])
                    {
                        break;
                    }
                }

                if (color < colors)
                {
                    coloring[vertex] = color;
                }
                else
                {
                    // Граф не может быть раскрашен с заданным количеством цветов
                    return null;
                }

                for (int adjacentVertex = 0; adjacentVertex < vertexCount; adjacentVertex++)
                {
                    if (adjacencyMatrix[vertex, adjacentVertex] == 1 && coloring[adjacentVertex] != -1)
                    {
                        availableColors[coloring[adjacentVertex]] = false;
                    }
                }
            }

            return coloring;
        }

        public static int ChromaticPolynomial(int[][] adjacencyMatrix, int colors)
        {
            int vertexCount = adjacencyMatrix.Length;
            bool[] colored = new bool[vertexCount];
            return RecursiveColoring(adjacencyMatrix, colored, colors, 0);
        }

        private static int RecursiveColoring(int[][] adjacencyMatrix, bool[] colored, int colors, int currentVertex)
        {
            if (currentVertex == adjacencyMatrix.Length)
            {
                return 1;
            }

            int coloringsCount = 0;
            for (int color = 1; color <= colors; color++)
            {
                if (IsValidColor(adjacencyMatrix, colored, currentVertex, color))
                {
                    colored[currentVertex] = true;
                    coloringsCount += RecursiveColoring(adjacencyMatrix, colored, colors, currentVertex + 1);
                    colored[currentVertex] = false;
                }
            }
            return coloringsCount;
        }

        public static int ChromaticPolynomial(string g6, int colors = 2)
        {
            int[,] adjacencyMatrix = G6ToAdjacencyMatrix(g6);
            int[][] jaggedMatrix = ConvertToJaggedArray(adjacencyMatrix);
            int vertexCount = jaggedMatrix.Length;
            bool[] colored = new bool[vertexCount];
            return RecursiveColoring(jaggedMatrix, colored, colors, 0);
        }

        private static int[][] ConvertToJaggedArray(int[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            int[][] jaggedArray = new int[rows][];

            for (int i = 0; i < rows; i++)
            {
                jaggedArray[i] = new int[cols];
                for (int j = 0; j < cols; j++)
                {
                    jaggedArray[i][j] = matrix[i, j];
                }
            }

            return jaggedArray;
        }

        private static bool IsValidColor(
            int[][] adjacencyMatrix,
            bool[] colored,
            int vertex,
            int color
            )
        {
            for (int i = 0; i < adjacencyMatrix[vertex].Length; i++)
            {
                if (i != vertex && adjacencyMatrix[vertex][i] == color && colored[i])
                {
                    return false;
                }
            }
            return true;
        }

        public static int ComputeDistinguishingNumber(int[,] adjacencyMatrix)
        {
            int vertexCount = adjacencyMatrix.GetLength(0);
            int[] colors = Enumerable.Repeat(-1, vertexCount).ToArray();
            bool[] availableColors = new bool[vertexCount];

            colors[0] = 0; // Начинаем с раскрашивания первой вершины в первый цвет

            for (int v = 1; v < vertexCount; v++)
            {
                Array.Fill(availableColors, true); // Сброс доступных цветов

                // Проверка цветов смежных вершин
                for (int i = 0; i < vertexCount; i++)
                {
                    if (adjacencyMatrix[v, i] == 1 && colors[i] != -1)
                    {
                        availableColors[colors[i]] = false;
                    }
                }

                // Нахождение первого доступного цвета
                int cr;
                for (cr = 0; cr < vertexCount; cr++)
                {
                    if (availableColors[cr]) break;
                }

                colors[v] = cr; // Назначение найденного цвета вершине
            }

            // Возврат количества используемых цветов + 1, поскольку индексация начинается с 0
            return colors.Max() + 1;
        }

        // Дополнительный метод для работы с g6 форматом
        public static int ComputeDistinguishingNumber(string g6)
        {
            int[,] adjacencyMatrix = G6ToAdjacencyMatrix(g6);
            return ComputeDistinguishingNumber(adjacencyMatrix);
        }

    }

}
