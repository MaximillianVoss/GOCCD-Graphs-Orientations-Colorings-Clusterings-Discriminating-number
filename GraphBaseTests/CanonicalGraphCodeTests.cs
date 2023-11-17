using Microsoft.VisualStudio.TestTools.UnitTesting;
using GraphBase.Параметры;
using System;
using System.Linq;
using System.Collections.Generic;

namespace GraphBase.Параметры
{
    [TestClass]
    public class CanonicalGraphCodeTests
    {
        [TestMethod]
        public void ConstructorTest_ValidInput_CreatesCanonicalGraphCode()
        {
            int[,] matrix = { { 0, 1 }, { 1, 0 } };
            var adjacencyMatrix = new AdjacencyMatrix(matrix);

            var canonicalCode = new CanonicalGraphCode(adjacencyMatrix);

            Assert.IsFalse(string.IsNullOrEmpty(canonicalCode.Hash));
            Assert.AreEqual(1, canonicalCode.Edges.Count);
            Assert.IsTrue(canonicalCode.Edges.Contains(Tuple.Create(0, 1)));
        }

        [TestMethod]
        public void GenerateHashFromMatrixTest_ValidMatrix_GeneratesConsistentHash()
        {
            int[,] matrix = { { 0, 1 }, { 1, 0 } };
            var adjacencyMatrix = new AdjacencyMatrix(matrix);

            var canonicalCode = new CanonicalGraphCode(adjacencyMatrix);
            var expectedHash = canonicalCode.GenerateHashFromMatrix(adjacencyMatrix);

            Assert.AreEqual(expectedHash, canonicalCode.Hash);
        }

        [TestMethod]
        public void ExtractEdgesFromMatrixTest_ValidMatrix_ExtractsEdgesCorrectly()
        {
            int[,] matrix = { { 0, 1, 0 }, { 1, 0, 1 }, { 0, 1, 0 } };
            var adjacencyMatrix = new AdjacencyMatrix(matrix);

            var canonicalCode = new CanonicalGraphCode(adjacencyMatrix);
            var expectedEdges = new List<Tuple<int, int>>
            {
                Tuple.Create(0, 1),
                Tuple.Create(1, 2)
            };

            CollectionAssert.AreEquivalent(expectedEdges, canonicalCode.Edges);
        }

        [TestMethod]
        public void ToAdjacencyMatrixTest_ValidCanonicalGraphCode_ConvertsBackCorrectly()
        {
            int[,] matrix = { { 0, 1 }, { 1, 0 } };
            var originalAdjacencyMatrix = new AdjacencyMatrix(matrix);

            var canonicalCode = new CanonicalGraphCode(originalAdjacencyMatrix);
            var reconstructedAdjacencyMatrix = canonicalCode.ToAdjacencyMatrix();

            Assert.IsTrue(Enumerable.Range(0, matrix.GetLength(0)).All(i =>
                Enumerable.Range(0, matrix.GetLength(1)).All(j =>
                    matrix[i, j] == reconstructedAdjacencyMatrix.Matrix[i, j])));
        }

        [TestMethod]
        public void FromAdjacencyMatrixTest_ValidMatrix_CreatesCanonicalGraphCode()
        {
            int[,] matrix = { { 0, 1 }, { 1, 0 } };
            var adjacencyMatrix = new AdjacencyMatrix(matrix);

            var canonicalCode = CanonicalGraphCode.FromAdjacencyMatrix(adjacencyMatrix);

            Assert.IsFalse(string.IsNullOrEmpty(canonicalCode.Hash));
            Assert.AreEqual(1, canonicalCode.Edges.Count);
            Assert.IsTrue(canonicalCode.Edges.Contains(Tuple.Create(0, 1)));
        }
    }
}
