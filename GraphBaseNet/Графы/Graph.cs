using System.Diagnostics;

namespace GraphBase.Графы
{
    public abstract class Graph
    {
        #region Поля
        protected int[,] _adjacencyMatrix;
        #endregion

        #region Свойства
        /// <summary>
        /// Получает матрицу смежности графа.
        /// </summary>
        public int[,] AdjacencyMatrix => this._adjacencyMatrix;

        /// <summary>
        /// Получает количество вершин в графе.
        /// </summary>
        public int VerticesCount => this._adjacencyMatrix.GetLength(0);
        #endregion

        #region Конструкторы/Деструкторы
        // Конструкторы остаются без изменений
        #endregion

        #region Абстрактные методы
        public abstract int GetChromaticNumber();
        public abstract int GetChromaticIndex();
        public abstract int GetDistinguishingNumber(int maxColors);

        /// <summary>
        /// Возвращает информацию о графе.
        /// </summary>
        /// <param name="numberOfColors">Число цветов для раскраски графа.</param>
        /// <returns>Строка с информацией о графе.</returns>
        public abstract string GetInfo(int numberOfColors = 0);

        /// <summary>
        /// Возвращает строку, представляющую граф, с временем выполнения метода GetInfo.
        /// </summary>
        /// <param name="numberOfColors">Число цветов для раскраски графа.</param>
        /// <returns>Строка, представляющая граф.</returns>
        public string ToString(int numberOfColors = 0)
        {
            var stopwatch = Stopwatch.StartNew();
            string info = this.GetInfo(numberOfColors);
            stopwatch.Stop();
            return $"{info}, Вычислено за: {stopwatch.ElapsedMilliseconds} мс";
        }
        #endregion
    }
}
