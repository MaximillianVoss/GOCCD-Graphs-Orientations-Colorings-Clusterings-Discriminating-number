using Microsoft.VisualStudio.TestTools.UnitTesting;
using GraphBase.Параметры;
using System;

namespace GraphBase.Параметры
{
    [TestClass]
    public class AdjacencyMatrixTests
    {
        [TestMethod]
        public void ConstructorTest_ValidMatrix_InitializesCorrectly()
        {
            // Arrange
            int[,] matrix = { { 0, 1 }, { 1, 0 } };

            // Act
            var adjacencyMatrix = new AdjacencyMatrix(matrix);

            // Assert
            // Проверки для matrix, edges, degreeVector
        }

        [TestMethod]
        public void InitializeDegreeVectorAndEdgesTest_ValidMatrix_CalculatesCorrectly()
        {
            int[,] matrix = { { 0, 1 }, { 1, 0 } };
            var adjacencyMatrix = new AdjacencyMatrix(matrix);

            Assert.AreEqual(1, adjacencyMatrix.DegreeVector[0]);
            Assert.AreEqual(1, adjacencyMatrix.DegreeVector[1]);
            Assert.IsTrue(adjacencyMatrix.Edges.Contains(Tuple.Create(0, 1)));
            Assert.IsTrue(adjacencyMatrix.Edges.Contains(Tuple.Create(1, 0)));
        }

        // Тестирование преобразования в DegreeVector
        [TestMethod]
        public void ToDegreeVectorTest_ValidMatrix_ConvertsCorrectly()
        {
            int[,] matrix = { { 0, 1 }, { 1, 0 } };
            var adjacencyMatrix = new AdjacencyMatrix(matrix);
            var degreeVector = adjacencyMatrix.ToDegreeVector();

            Assert.AreEqual(1, degreeVector.Degrees[0]);
            Assert.AreEqual(1, degreeVector.Degrees[1]);
            Assert.IsTrue(degreeVector.Edges.Contains(Tuple.Create(0, 1)));
            Assert.IsTrue(degreeVector.Edges.Contains(Tuple.Create(1, 0)));
        }

        // Тестирование преобразования в G6String
        [TestMethod]
        public void ToG6StringTest_ValidMatrix_ConvertsCorrectly()
        {
            int[,] matrix = { { 0, 1 }, { 1, 0 } };
            var adjacencyMatrix = new AdjacencyMatrix(matrix);
            var g6String = adjacencyMatrix.ToG6String();

            Assert.AreEqual("A_", g6String.G6); // Предполагая, что "A_" - это правильное представление матрицы в формате G6
        }

        // Тестирование преобразования в CanonicalGraphCode
        [TestMethod]
        public void ToCanonicalGraphCodeTest_ValidMatrix_ConvertsCorrectly()
        {
            int[,] matrix = { { 0, 1 }, { 1, 0 } };
            var adjacencyMatrix = new AdjacencyMatrix(matrix);
            var canonicalCode = adjacencyMatrix.ToCanonicalGraphCode();

            // Проверяем, что хэш не пустой
            Assert.IsFalse(string.IsNullOrEmpty(canonicalCode.Hash));

            // Проверяем, что список рёбер содержит одно ребро (0, 1) или (1, 0)
            var expectedEdge = Tuple.Create(0, 1);
            var expectedEdgeReversed = Tuple.Create(1, 0); // Для симметрии, если она важна
            Assert.IsTrue(canonicalCode.Edges.Contains(expectedEdge) ||
                          canonicalCode.Edges.Contains(expectedEdgeReversed)); // Только одна из проверок необходима
        }

    }
}
