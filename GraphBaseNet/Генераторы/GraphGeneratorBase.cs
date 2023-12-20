using System.Collections.Generic;

namespace GraphBase.Генераторы
{
    public enum GeneratorType
    {
        /// <summary>
        /// Использует алгоритм получения канонического кода как из nauty
        /// Канонический код — это способ представления графа, который однозначен для изоморфных графов
        /// </summary>
        BY_CANONICAL_CODE = 1,
        /// <summary>
        /// Перебирает все графы для каждого кода
        /// </summary>
        BRUTE_FORCE_ALL_GRAPHS = 2,
        /// <summary>
        /// Перебирает все коды
        /// </summary>
        BRUTE_FORCE_ALL_CODES = 3,
        /// <summary>
        /// Перебирает все коды, останавливая рекурсивный процесс, если текущий код не канонический
        /// </summary>
        BRUTE_FORCE_ALL_CODES_WITH_FILTER = 4,
        /// <summary>
        /// Генерирует все связанные графы для заданного числа вершин
        /// Связанный граф — это свойство графа, обозначающее, что любые две вершины соединены путем вершин и ребер
        /// </summary>
        CONNECTED_GRAPHS = 5
    }

    public abstract class GraphGeneratorBase
    {
        #region Поля
        protected int vertexCount;
        #endregion

        #region Конструкторы/Деструкторы
        protected GraphGeneratorBase(int vertexCount)
        {
            this.vertexCount = vertexCount;
        }
        #endregion

        #region Абстрактные методы
        public abstract IEnumerable<string> GenerateAllGraphsG6();
        public abstract IEnumerable<string> GenerateAllGraphsG6(int vertexCount, GeneratorType generatorType);
        #endregion
    }
}
