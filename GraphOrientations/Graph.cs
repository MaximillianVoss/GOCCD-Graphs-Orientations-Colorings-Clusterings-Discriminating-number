using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraphOrientations
{
    public class Graph
    {
        #region Поля

        #endregion

        #region Свойства
        /// <summary>
        /// Граф в формате G6
        /// </summary>
        public String G6 { set; get; }
        /// <summary>
        /// Матрица смежности
        /// </summary>
        public int[] AdjacencyMatrix { set; get; }
        /// <summary>
        /// Цвета графа
        /// </summary>
        public int[] Colors { get; set; }
        /// <summary>
        /// Число вершин
        /// </summary>
        public int VertexCount => this.AdjacencyMatrix.Length;
        #endregion

        #region Методы
        /// <summary>
        /// Выполняет преобразование графа в формате G6 в матрицу смежности в виде одномерного массива целых чисел.
        /// </summary>
        /// <param name="strG6">
        /// Формат G6 (Graph6) является компактным способом представления неориентированных графов. 
        /// Он использует шестидесятичное кодирование для представления связей между вершинами графа.
        /// </param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int[] FromG6(string strG6)
        {
            var n = strG6[0] - '?';
            var result = new int[n];

            Parallel.For(1, n, i =>
            {
                var rOffset = 32;
                var k = 1;
                var val = strG6[k] - '?';

                var offset = 1;

                for (int j = 0; j < i; j++)
                {
                    if ((val & rOffset) > 0)
                    {
                        lock (result)
                        {
                            result[i] |= offset;
                            result[j] |= 1 << i;
                        }
                    }

                    if ((rOffset >>= 1) == 0)
                    {
                        rOffset = 32;
                        if (++k < strG6.Length)
                        {
                            val = strG6[k] - '?';
                        }
                        else if (j != i - 1)
                        {
                            throw new Exception("Ошибка формата strG6");
                        }
                    }

                    offset <<= 1;
                }
            });

            return result;
        }
        public int[] FromG6()
        {
            return this.FromG6(this.G6);
        }
        public int CalculateAutomorphismCount()
        {
            int n = (int)Math.Sqrt(this.AdjacencyMatrix.Length);
            var matrix = new int[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    matrix[i, j] = (this.AdjacencyMatrix[i] >> j) & 1;
                }
            }

            var permutations = GetPermutations(Enumerable.Range(0, n).ToList());
            return permutations.Count(p => IsAutomorphism(p, matrix, this.Colors));
        }
        private bool IsAutomorphism(List<int> permutation, int[,] matrix, int[] colors)
        {
            int n = matrix.GetLength(0);

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (matrix[i, j] != matrix[permutation[i], permutation[j]] ||
                        (colors != null && colors[i] != colors[permutation[i]]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        private static IEnumerable<List<int>> GetPermutations(List<int> list)
        {
            if (list.Count == 1)
                yield return list;
            else
            {
                foreach (var i in list)
                {
                    var remainingList = list.Where(item => item != i).ToList();
                    foreach (var tail in GetPermutations(remainingList))
                    {
                        tail.Add(i);
                        yield return tail;
                    }
                }
            }
        }
        public int GetDistinguishingNumber()
        {
            int[] colors = new int[this.VertexCount];
            return FindDistinguishingNumber(colors, 0);
        }
        private int FindDistinguishingNumber(int[] colors, int colorIndex)
        {
            // Конвертация одномерного массива смежности в двумерный
            int n = (int)Math.Sqrt(this.AdjacencyMatrix.Length);
            var matrix = new int[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    matrix[i, j] = (this.AdjacencyMatrix[i] >> j) & 1;
                }
            }

            if (colorIndex == colors.Length)
            {
                if (!IsAutomorphismBroken(matrix, colors))
                    return colors.Max() + 1; // Начиная с 1, а не 0
            }
            else
            {
                // Перебор цветов для каждой вершины
                for (int color = 1; color <= colors.Length; color++)
                {
                    colors[colorIndex] = color;
                    int result = FindDistinguishingNumber(colors, colorIndex + 1);
                    if (result != -1)
                        return result;
                }
            }
            return -1;
        }
        private bool IsAutomorphismBroken(int[,] matrix, int[] colors)
        {
            var n = matrix.GetLength(0);
            var permutations = GetPermutations(Enumerable.Range(0, n).ToList());

            foreach (var permutation in permutations)
            {
                if (IsAutomorphism(permutation, matrix, colors))
                    return false; // Найден сохраняющийся автоморфизм
            }

            return true; // Все автоморфизмы нарушены
        }
        #endregion

        #region Конструкторы/Деструкторы
        public Graph(String g6)
        {
            this.G6 = g6 ?? throw new Exception("G6 строка графа не сожет быть null");
            this.AdjacencyMatrix = this.FromG6(this.G6);
        }
        #endregion

        #region Операторы

        #endregion

        #region Обработчики событий

        #endregion


    }
}
