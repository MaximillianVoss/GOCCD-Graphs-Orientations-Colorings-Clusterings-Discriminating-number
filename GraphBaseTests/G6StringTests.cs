using Microsoft.VisualStudio.TestTools.UnitTesting;
using GraphBase.Параметры;
using System;
using System.Linq;

namespace GraphBase.Параметры
{
    [TestClass]
    public class G6StringTests
    {
        [TestMethod]
        public void ConstructorTest_ValidAdjacencyMatrix_CreatesG6String()
        {
            int[,] matrix = { { 0, 1 }, { 1, 0 } };
            var adjacencyMatrix = new AdjacencyMatrix(matrix);

            var g6String = new G6String(adjacencyMatrix);

            Assert.IsFalse(string.IsNullOrEmpty(g6String.G6));
            // Дополнительно проверить, что строка G6 соответствует ожидаемому формату
        }

        [TestMethod]
        public void ToAdjacencyMatrixTest_ValidG6String_ConvertsBackCorrectly()
        {
            var adjacencyMatrix = new AdjacencyMatrix(new int[,] { { 0, 1 }, { 1, 0 } });
            var g6String = new G6String(adjacencyMatrix);

            var resultMatrix = g6String.ToAdjacencyMatrix();

            for (int i = 0; i < resultMatrix.Matrix.GetLength(0); i++)
            {
                for (int j = 0; j < resultMatrix.Matrix.GetLength(1); j++)
                {
                    Assert.AreEqual(adjacencyMatrix.Matrix[i, j], resultMatrix.Matrix[i, j]);
                }
            }
        }

        [TestMethod]
        public void RxMethodTest_ValidG6String_ProducesCorrectBinaryString()
        {
            string g6 = "A_";
            // Пример ожидаемого двоичного представления для строки G6 "A_",
            // где A представляет граф из двух вершин (код 63 + 2) и "_" - это пустое ребро.
            string expectedBinary = "100000";  // Если "_" представляет пустое ребро
            string actualBinary = G6String.Rx(1, g6.Length - 1, g6);

            Assert.AreEqual(expectedBinary, actualBinary);
        }


        // Дополнительные тесты для проверки различных размеров матриц и различных строк G6.
    }
}
