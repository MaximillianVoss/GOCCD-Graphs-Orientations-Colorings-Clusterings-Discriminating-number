using GraphBase.Графы;
using GraphBase.Параметры;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace GraphBaseTests.Граф
{
    [TestClass]
    public class GraphTests
    {
        [TestMethod]
        public void ConstructorTest()
        {
            // Тестируем конструктор с матрицей смежности
            int[,] matrix = { { 0, 1 }, { 1, 0 } };
            GraphCustom graph = new GraphCustom(matrix);

            Assert.AreEqual(2, graph.VerticesCount);
            CollectionAssert.AreEqual(matrix, graph.AdjacencyMatrix);
        }

        [TestMethod]
        public void Constructor_FromG6String_CreatesExpectedGraph()
        {
            // Тестируем конструктор со строкой G6
            var g6String = new G6String("A_");
            int[,] expectedAdjacencyMatrix = { { 0, 1 }, { 1, 0 } };

            GraphCustom graph = new GraphCustom(g6String);

            Assert.AreEqual(expectedAdjacencyMatrix.GetLength(0), graph.VerticesCount);
            for (int i = 0; i < graph.VerticesCount; i++)
            {
                for (int j = 0; j < graph.VerticesCount; j++)
                {
                    Assert.AreEqual(expectedAdjacencyMatrix[i, j], graph.AdjacencyMatrix[i, j]);
                }
            }
        }

        [TestMethod]
        public void Constructor_DegreeVector_CreatesExpectedGraph()
        {
            // Тестируем конструктор с вектором степеней
            int[] degreeVector = new int[] { 2, 2, 0 };
            var edges = new List<Tuple<int, int>> { Tuple.Create(0, 1), Tuple.Create(1, 0) }; // Явно указываем список рёбер
            int[,] expectedAdjacencyMatrix = {
                { 0, 1, 0 },
                { 1, 0, 0 },
                { 0, 0, 0 }
            };

            GraphCustom graph = new GraphCustom(new DegreeVector(degreeVector, edges)); // Используем конструктор с дополнительной информацией

            Assert.AreEqual(degreeVector.Length, graph.VerticesCount);
            for (int i = 0; i < graph.VerticesCount; i++)
            {
                for (int j = 0; j < graph.VerticesCount; j++)
                {
                    Assert.AreEqual(expectedAdjacencyMatrix[i, j], graph.AdjacencyMatrix[i, j]);
                }
            }
        }

        [TestMethod]
        public void Constructor_CanonicalGraphCode_CreatesExpectedGraph()
        {
            // Arrange
            // Создаем матрицу смежности и соответствующий список рёбер
            int[,] adjacencyMatrix = {
                { 0, 1, 0 },
                { 1, 0, 1 },
                { 0, 1, 0 }
            };


            // Создаем канонический код на основе этой матрицы
            var canonicalCode = new CanonicalGraphCode(new AdjacencyMatrix(adjacencyMatrix));

            // Act
            // Создаем граф из канонического кода
            GraphCustom graph = new GraphCustom(canonicalCode);

            // Assert
            // Проверяем, что матрица смежности графа соответствует ожидаемой
            Assert.AreEqual(adjacencyMatrix.GetLength(0), graph.VerticesCount, "Number of vertices should match.");
            for (int i = 0; i < graph.VerticesCount; i++)
            {
                for (int j = 0; j < graph.VerticesCount; j++)
                {
                    Assert.AreEqual(adjacencyMatrix[i, j], graph.AdjacencyMatrix[i, j],
                                    $"Adjacency matrix does not match expected value at position [{i}, {j}].");
                }
            }
        }

        [TestMethod]
        public void GetChromaticNumber_SimpleGraph_ReturnsExpectedNumber()
        {
            // Создание простого графа
            // Граф: A - B - C (линейный граф)
            int[,] adjacencyMatrix = new int[,]
            {
        { 0, 1, 0 },
        { 1, 0, 1 },
        { 0, 1, 0 }
            };
            GraphCustom graph = new GraphCustom(adjacencyMatrix);

            // Ожидаемое хроматическое число для линейного графа с тремя вершинами - 2
            int expectedChromaticNumber = 2;

            // Выполнение
            int actualChromaticNumber = graph.GetChromaticNumber();

            // Проверка
            Assert.AreEqual(expectedChromaticNumber, actualChromaticNumber);
        }

        [TestMethod]
        public void GetChromaticIndex_SimpleGraph_ReturnsExpectedIndex()
        {
            // Создание простого графа
            // Граф: A - B - C (линейный граф)
            int[,] adjacencyMatrix = new int[,]
            {
        { 0, 1, 0 },
        { 1, 0, 1 },
        { 0, 1, 0 }
            };
            GraphCustom graph = new GraphCustom(adjacencyMatrix);

            // Ожидаемый хроматический индекс для линейного графа с тремя вершинами - 2
            int expectedChromaticIndex = 2;

            // Выполнение
            int actualChromaticIndex = graph.GetChromaticIndex();

            // Проверка
            Assert.AreEqual(expectedChromaticIndex, actualChromaticIndex);
        }

        [TestMethod]
        public void GetDistinguishingNumber_CompleteGraph_ReturnsVertexCount()
        {
            //// Создаем полный граф с 3 вершинами
            //int[,] adjacencyMatrix = new int[,]
            //{
            //    { 0, 1, 1 },
            //    { 1, 0, 1 },
            //    { 1, 1, 0 }
            //};
            //GraphCustom graph = new GraphCustom(adjacencyMatrix);

            //// Для полного графа с 3 вершинами различительное число должно быть 3
            //int expectedDistinguishingNumber = 3;
            //// Вызываем метод GetDistinguishingNumber
            //int actualDistinguishingNumber = graph.GetDistinguishingNumber(); // Максимальное количество цветов

            //// Проверяем, что полученное различительное число соответствует ожидаемому
            //Assert.AreEqual(expectedDistinguishingNumber, actualDistinguishingNumber,
            //                "The distinguishing number of a complete graph should equal the number of its vertices.");
        }

    }
}
