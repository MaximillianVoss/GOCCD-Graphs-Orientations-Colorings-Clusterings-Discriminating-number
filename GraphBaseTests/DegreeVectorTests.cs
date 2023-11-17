using Microsoft.VisualStudio.TestTools.UnitTesting;
using GraphBase.Параметры;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphBase.Параметры
{
    [TestClass]
    public class DegreeVectorTests
    {
        [TestMethod]
        public void ConstructorTest_SimpleConstructor_CreatesDegreeVector()
        {
            int[] degrees = { 1, 2 };
            var degreeVector = new DegreeVector(degrees);

            CollectionAssert.AreEqual(degrees, degreeVector.Degrees);
            Assert.IsNull(degreeVector.Edges);
        }

        [TestMethod]
        public void ConstructorTest_ComplexConstructor_CreatesDegreeVectorWithEdges()
        {
            int[] degrees = { 1, 1 };
            var edges = new List<Tuple<int, int>> { Tuple.Create(0, 1) };
            var degreeVector = new DegreeVector(degrees, edges);

            CollectionAssert.AreEqual(degrees, degreeVector.Degrees);
            CollectionAssert.AreEquivalent(edges, degreeVector.Edges);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ToAdjacencyMatrixTest_WithoutEdges_ThrowsException()
        {
            int[] degrees = { 1, 1 };
            var degreeVector = new DegreeVector(degrees);

            var matrix = degreeVector.ToAdjacencyMatrix();
        }

        [TestMethod]
        public void ToAdjacencyMatrixTest_WithEdges_CreatesCorrectMatrix()
        {
            int[] degrees = { 1, 1 };
            var edges = new List<Tuple<int, int>> { Tuple.Create(0, 1) };
            var degreeVector = new DegreeVector(degrees, edges);

            var matrix = degreeVector.ToAdjacencyMatrix();

            Assert.AreEqual(0, matrix.Matrix[0, 0]);
            Assert.AreEqual(1, matrix.Matrix[0, 1]);
            Assert.AreEqual(1, matrix.Matrix[1, 0]);
            Assert.AreEqual(0, matrix.Matrix[1, 1]);
        }

        // Здесь могут быть дополнительные тесты, например, на проверку сериализации/десериализации объекта DegreeVector.
    }
}
