using System;
using System.Collections.Generic;

namespace GraphBase.Параметры
{
    public class AdjacencyMatrix
    {
        #region Поля
        private int[,] matrix;
        private List<Tuple<int, int>> edges; // Список рёбер
        private int[] degreeVector; // Вектор степеней
        #endregion

        #region Свойства
        public int[,] Matrix
        {
            get => this.matrix;
            private set => this.matrix = value;
        }

        public List<Tuple<int, int>> Edges
        {
            get => this.edges;
            private set => this.edges = value;
        }

        public int[] DegreeVector
        {
            get => this.degreeVector;
            private set => this.degreeVector = value;
        }
        #endregion

        #region Конструкторы/Деструкторы
        public AdjacencyMatrix() : this(new int[0, 0])
        {

        }
        public AdjacencyMatrix(int[,] matrix)
        {
            this.Matrix = matrix;
            this.InitializeDegreeVectorAndEdges();
        }
        #endregion

        #region Методы
        private void InitializeDegreeVectorAndEdges()
        {
            int size = this.Matrix.GetLength(0);
            this.edges = new List<Tuple<int, int>>();
            this.degreeVector = new int[size];

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (this.Matrix[i, j] == 1)
                    {
                        this.edges.Add(Tuple.Create(i, j));
                        this.degreeVector[i]++;
                    }
                }
            }
        }

        public DegreeVector ToDegreeVector()
        {
            // Передаём вектор степеней и список рёбер
            return new DegreeVector(this.degreeVector, this.edges);
        }

        public G6String ToG6String()
        {
            return new G6String(this);
        }

        public CanonicalGraphCode ToCanonicalGraphCode()
        {
            return new CanonicalGraphCode(this);
        }

        // Статические методы для создания из других форматов
        public static AdjacencyMatrix FromDegreeVector(DegreeVector vector)
        {
            return vector.ToAdjacencyMatrix();
        }

        public static AdjacencyMatrix FromG6String(G6String g6)
        {
            return g6.ToAdjacencyMatrix();
        }

        public static AdjacencyMatrix FromCanonicalGraphCode(CanonicalGraphCode code)
        {
            return code.ToAdjacencyMatrix();
        }
        #endregion
    }
}
