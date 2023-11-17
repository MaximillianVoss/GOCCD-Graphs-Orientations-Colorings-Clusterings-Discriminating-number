using GraphBase.Генераторы;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphBaseTests.Генераторы
{
    [TestClass]
    public class GraphGeneratorCustomTests
    {
        [TestMethod]
        public void GenerateAllGraphsG6_VertexCount2_ProducesCorrectNumberOfGraphs()
        {
            // Arrange
            int vertexCount = 2;
            var generator = new GraphGeneratorCustom(vertexCount);
            int expectedNumberOfGraphs = 2; // Для 2 вершин должно быть 2^(2 * (2 - 1) / 2) = 2 графа

            // Act
            List<string> generatedGraphs = generator.GenerateAllGraphsG6().ToList();

            // Assert
            Assert.AreEqual(expectedNumberOfGraphs, generatedGraphs.Count, "The number of generated graphs should match the expected number.");
        }

        [TestMethod]
        public void GenerateAllGraphsG6_VertexCount3_ProducesCorrectGraphs()
        {
            // Arrange
            int vertexCount = 3;
            var generator = new GraphGeneratorCustom(vertexCount);
            var expectedGraphs = new HashSet<string>
        {
            "A_", // Пустой граф
            "AY", // Один ребро
            "A`", // Два ребра, образующих цепь
            "A~"  // Полный граф
        };

            // Act
            HashSet<string> generatedGraphs = generator.GenerateAllGraphsG6().ToHashSet();

            // Assert
            CollectionAssert.AreEquivalent(expectedGraphs.ToList(), generatedGraphs.ToList(), "The generated graphs should match the expected set of graphs.");
        }

        // Дополнительные тесты могут проверять специфические графы, проверять правильность формата G6 и так далее.
    }

}
