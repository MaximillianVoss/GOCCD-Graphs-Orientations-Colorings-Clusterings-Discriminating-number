using System;
using System.Collections.Generic;
using System.Text;

namespace GraphBase.Параметры
{
    public class G6String
    {
        #region Поля
        private string g6;
        #endregion

        #region Свойства
        public string G6
        {
            get => this.g6;
            private set => this.g6 = value;
        }
        #endregion

        #region Конструкторы/Деструкторы
        public G6String(string g6)
        {
            this.g6 = g6;
        }
        public G6String(AdjacencyMatrix adjacencyMatrix)
        {
            this.G6 = ConvertAdjacencyMatrixToG6String(adjacencyMatrix);
        }
        #endregion

        #region Методы
        private string ConvertAdjacencyMatrixToG6String(AdjacencyMatrix adjacencyMatrix)
        {
            var sb = new StringBuilder();
            sb.Append((char)(adjacencyMatrix.Matrix.GetLength(0) + 63));

            int bitIndex = 0;
            int currentByte = 0;
            for (int i = 0; i < adjacencyMatrix.Matrix.GetLength(0); i++)
            {
                for (int j = 0; j < i; j++)
                {
                    if (adjacencyMatrix.Matrix[i, j] == 1)
                    {
                        currentByte |= 1 << (5 - bitIndex);
                    }

                    bitIndex++;
                    if (bitIndex == 6)
                    {
                        sb.Append((char)(currentByte + 63));
                        bitIndex = 0;
                        currentByte = 0;
                    }
                }
            }

            if (bitIndex > 0)
            {
                sb.Append((char)(currentByte + 63));
            }

            return sb.ToString();
        }

        public AdjacencyMatrix ToAdjacencyMatrix()
        {
            int n = this.G6[0] - 63;
            if (n < 0)
                throw new ArgumentException("Invalid G6 string.");

            string R_x = Rx(1, this.G6.Length - 1, this.G6);
            int[,] matrix = new int[n, n];
            int k = 0;
            for (int j = 1; j < n; j++)
            {
                for (int i = 0; i < j; i++)
                {
                    matrix[i, j] = matrix[j, i] = R_x[k] == '1' ? 1 : 0;
                    k++;
                }
            }

            return new AdjacencyMatrix(matrix);
        }
        /// <summary>
        /// Преобразует указанный сегмент строки G6 в бинарное представление.
        /// </summary>
        /// <param name="init">Начальный индекс сегмента строки G6 для преобразования.</param>
        /// <param name="end">Конечный индекс сегмента строки G6 для преобразования.</param>
        /// <param name="g6">Строка G6, представляющая граф.</param>
        /// <returns>Бинарная строка, представляющая сегмент строки G6.</returns>
        /// <remarks>
        /// Каждый символ строки G6, начиная с индекса <paramref name="init"/> до <paramref name="end"/>,
        /// преобразуется в шестибитное бинарное число, которое затем добавляется к результирующей бинарной строке.
        /// Строка G6 кодирует граф, где каждый символ (после корректировки на смещение ASCII в 63) представляет шесть бит,
        /// соответствующих рёбрам графа.
        /// </remarks>
        public static string Rx(int init, int end, string g6)
        {
            string R_x = "";

            for (int i = init; i <= end; i++)
            {
                string bin6 = Convert.ToString(g6.ElementAt(i) - 63, 2).PadLeft(6, '0');
                R_x += bin6;
            }
            return R_x;
        }
        #endregion

        #region Операторы
        // Место для операторов, если они будут добавлены в будущем
        #endregion

        #region Обработчики событий
        // Место для обработчиков событий, если они будут добавлены в будущем
        #endregion
    }
}
