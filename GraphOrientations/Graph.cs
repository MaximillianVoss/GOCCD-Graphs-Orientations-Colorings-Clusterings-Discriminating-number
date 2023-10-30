using System;
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
