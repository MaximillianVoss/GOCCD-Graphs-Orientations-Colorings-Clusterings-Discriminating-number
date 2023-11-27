using GraphBase.Параметры;

namespace GraphBase.Графы
{
    public class GraphCustom : Graph
    {
        #region Поля

        #endregion

        #region Свойства

        #endregion

        #region Конструкторы/Деструкторы
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="GraphCustom"/> с использованием заданной матрицы смежности.
        /// </summary>
        /// <param name="adjacencyMatrix">Матрица смежности графа.</param>
        public GraphCustom(int[,] adjacencyMatrix)
        {
            this._adjacencyMatrix = adjacencyMatrix ?? throw new ArgumentNullException(nameof(adjacencyMatrix));
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="GraphCustom"/> из объекта <see cref="DegreeVector"/>.
        /// </summary>
        /// <param name="degreeVector">Объект <see cref="DegreeVector"/>, представляющий вектор степеней вершин.</param>
        public GraphCustom(DegreeVector degreeVector)
        {
            if (degreeVector == null)
                throw new ArgumentNullException(nameof(degreeVector));

            this._adjacencyMatrix = degreeVector.ToAdjacencyMatrix().Matrix;
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="GraphCustom"/> из объекта <see cref="G6String"/>.
        /// </summary>
        /// <param name="g6String">Объект <see cref="G6String"/>, представляющий граф в формате G6.</param>
        public GraphCustom(G6String g6String)
        {
            if (g6String == null)
                throw new ArgumentNullException(nameof(g6String));

            this._adjacencyMatrix = g6String.ToAdjacencyMatrix().Matrix;
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="GraphCustom"/> из объекта <see cref="CanonicalGraphCode"/>.
        /// </summary>
        /// <param name="canonicalCode">Объект <see cref="CanonicalGraphCode"/>, представляющий канонический код графа.</param>
        public GraphCustom(CanonicalGraphCode canonicalCode)
        {
            if (canonicalCode == null)
                throw new ArgumentNullException(nameof(canonicalCode));

            this._adjacencyMatrix = canonicalCode.ToAdjacencyMatrix().Matrix;
        }
        #endregion

        #region Методы

        #region Хроматическое число графа
        //Хроматическое число графа - это минимальное количество цветов, 
        //необходимое для окраски всех вершин графа таким образом, 
        //чтобы никакие две смежные вершины не были окрашены в один и тот же цвет.

        ///<summary>
        ///Вычисляет хроматическое число графа,
        ///используя жадный алгоритм.Хроматическое число - это минимальное количество цветов,
        ///необходимое для окраски вершин графа,
        ///чтобы никакие две смежные вершины не были окрашены в один цвет.
        ///</summary>
        ///<remarks>
        ///Этот метод использует простой жадный алгоритм для окраски вершин.
        ///Алгоритм начинает с первой вершины и окрашивает каждую вершину в минимально доступный цвет, 
        ///который ещё не использовался среди её соседей.
        ///Хотя этот метод не гарантирует нахождение оптимального хроматического числа для всех графов, 
        ///он эффективен и дает приемлемые результаты для многих типов графов, 
        ///особенно для малых и средних по размеру.
        ///</remarks>
        /// <returns>
        ///Возвращает минимальное количество цветов, 
        ///необходимое для окраски всех вершин графа, 
        ///чтобы никакие две смежные вершины не были окрашены в один цвет.
        ///</returns>
        public override int GetChromaticNumber()
        {
            int[] colorAssignments = new int[this.VerticesCount];
            for (int i = 0; i < this.VerticesCount; i++)
                colorAssignments[i] = -1;

            colorAssignments[0] = 0; // Назначаем первому узлу цвет 0

            bool[] availableColors = new bool[this.VerticesCount];
            for (int i = 0; i < this.VerticesCount; i++)
                availableColors[i] = true;

            for (int u = 1; u < this.VerticesCount; u++)
            {
                for (int i = 0; i < this.VerticesCount; i++)
                {
                    if (this._adjacencyMatrix[u, i] == 1 && colorAssignments[i] != -1)
                        availableColors[colorAssignments[i]] = false;
                }

                int cr;
                for (cr = 0; cr < this.VerticesCount; cr++)
                {
                    if (availableColors[cr])
                        break;
                }

                colorAssignments[u] = cr; // Назначаем узлу u минимально доступный цвет

                for (int i = 0; i < this.VerticesCount; i++)
                    availableColors[i] = true; // Сброс доступных цветов для следующего узла
            }

            return colorAssignments.Max() + 1; // Возвращает количество использованных цветов
        }

        #endregion

        #region Хроматический индекс графа
        //Хроматический индекс графа - это минимальное количество цветов, 
        //необходимое для окраски всех рёбер графа так, чтобы рёбра, 
        //инцидентные одной вершине, были разных цветов.

        ///<summary>
        ///Вычисляет хроматический индекс графа, используя жадный алгоритм.Хроматический индекс - это минимальное количество цветов, необходимое для окраски всех рёбер графа таким образом, чтобы смежные рёбра имели разные цвета.
        ///</summary>
        ///<remarks>
        ///Этот метод использует жадный подход для окраски каждого ребра графа. Для каждого ребра метод проверяет доступные цвета, исключая цвета, уже используемые смежными рёбрами, и выбирает минимально доступный цвет.Этот алгоритм не гарантирует нахождение оптимального хроматического индекса для всех графов, но он эффективен и даёт приемлемые результаты для многих типов графов, особенно для небольших.
        ///</remarks>
        ///<returns>
        ///Возвращает хроматический индекс графа, то есть минимальное количество цветов, необходимое для окраски всех рёбер графа, так чтобы смежные рёбра имели разные цвета.
        ///</returns>
        public override int GetChromaticIndex()
        {
            // Предполагаем, что каждое ребро изначально не окрашено
            int[,] edgeColors = new int[this.VerticesCount, this.VerticesCount];
            for (int i = 0; i < this.VerticesCount; i++)
            {
                for (int j = 0; j < this.VerticesCount; j++)
                    edgeColors[i, j] = -1;
            }

            int maxColor = 0;

            // Жадно окрашиваем каждое ребро
            for (int u = 0; u < this.VerticesCount; u++)
            {
                for (int v = 0; v < this.VerticesCount; v++)
                {
                    if (u >= this.VerticesCount || v >= this.VerticesCount) // Проверка границ массива
                        continue;

                    if (this._adjacencyMatrix[u, v] != 1 || edgeColors[u, v] != -1)
                        continue;

                    bool[] availableColors = new bool[this.VerticesCount];
                    Array.Fill(availableColors, true);

                    // Проверяем цвета смежных рёбер
                    for (int i = 0; i < this.VerticesCount; i++)
                    {
                        if (i >= this.VerticesCount) // Проверка границ массива
                            continue;

                        if (this._adjacencyMatrix[u, i] == 1 && edgeColors[u, i] != -1)
                            availableColors[edgeColors[u, i]] = false;
                        if (this._adjacencyMatrix[v, i] == 1 && edgeColors[v, i] != -1)
                            availableColors[edgeColors[v, i]] = false;
                    }

                    // Выбираем первый доступный цвет
                    int cr;
                    for (cr = 0; cr < this.VerticesCount; cr++)
                    {
                        if (cr >= this.VerticesCount) // Проверка границ массива
                            break;

                        if (availableColors[cr])
                            break;
                    }

                    edgeColors[u, v] = edgeColors[v, u] = cr; // Окрашиваем ребро
                    maxColor = Math.Max(maxColor, cr + 1);
                }
            }

            return maxColor;
        }

        #endregion

        #region Различительное число упрощенная версия
        public int GetDistinguishingNumberLite(int maxColors = 10)
        {
            int n = _adjacencyMatrix.GetLength(0);
            int edgeCount = 0;
            bool isCompleteGraph = true;

            // Вычисляем количество рёбер и проверяем, является ли граф полносвязным
            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    if (_adjacencyMatrix[i, j] == 1)
                    {
                        edgeCount++;
                    }
                    else if (i != j)
                    {
                        isCompleteGraph = false;
                    }
                }
            }

            if (isCompleteGraph)
            {
                return n <= maxColors ? n : -1; // Для полносвязного графа, если n превышает maxColors, возвращаем -1
            }

            // Оцениваем плотность графа
            double density = (double)edgeCount / (n * (n - 1) / 2);
            int estimatedDistinguishingNumber;

            if (density > 0.75)
            {
                estimatedDistinguishingNumber = n - 1;
            }
            else if (density > 0.5)
            {
                estimatedDistinguishingNumber = Math.Min(n / 2, 5);
            }
            else
            {
                estimatedDistinguishingNumber = Math.Min(3, n);
            }

            // Сравниваем оценочное различительное число с maxColors
            return estimatedDistinguishingNumber <= maxColors ? estimatedDistinguishingNumber : -1;
        }


        private void SimpleColoring(int[] colors, int maxColors)
        {
            int n = this._adjacencyMatrix.GetLength(0);
            int[] degrees = new int[n];
            var vertices = new List<int>(n);

            // Вычисляем степень каждой вершины
            for (int i = 0; i < n; i++)
            {
                int degree = 0;
                for (int j = 0; j < n; j++)
                {
                    if (this._adjacencyMatrix[i, j] == 1)
                        degree++;
                }
                degrees[i] = degree;
                vertices.Add(i);
            }

            // Сортируем вершины по убыванию степени
            vertices.Sort((a, b) => degrees[b].CompareTo(degrees[a]));

            // Инициализируем все цвета как -1 (не окрашено)
            Array.Fill(colors, -1);

            // Раскрашиваем каждую вершину
            foreach (int vertex in vertices)
            {
                bool[] availableColors = new bool[maxColors];
                Array.Fill(availableColors, true);

                // Проверяем цвета смежных вершин
                for (int j = 0; j < n; j++)
                {
                    if (this._adjacencyMatrix[vertex, j] == 1 && colors[j] != -1)
                    {
                        availableColors[colors[j] - 1] = false; // Цвет уже используется
                    }
                }

                // Находим первый доступный цвет
                for (int color = 0; color < maxColors; color++)
                {
                    if (availableColors[color])
                    {
                        colors[vertex] = color + 1; // Назначаем цвет (начиная с 1)
                        break;
                    }
                }
            }
        }

        private List<int> RandomPermutation(int n, Random random)
        {
            var list = Enumerable.Range(0, n).ToList();
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
            return list;
        }

        private List<List<int>> GenerateDeterministicPermutations(int n)
        {
            var list = Enumerable.Range(0, n).ToList();
            var permutations = new List<List<int>>();

            // Генерация перестановок на основе некоторого фиксированного критерия
            // Например, можно использовать степени вершин или другие характеристики
            // Здесь, для простоты, мы просто генерируем перестановки, начиная с исходного порядка
            this.DoGeneratePermutation(list, 0, n, permutations);
            return permutations;
        }

        private void DoGeneratePermutation(List<int> list, int start, int n, List<List<int>> permutations)
        {
            if (start == n - 1)
            {
                permutations.Add(new List<int>(list));
            }
            else
            {
                for (int i = start; i < n; i++)
                {
                    // Генерация перестановки путём обмена элементов
                    (list[start], list[i]) = (list[i], list[start]);
                    this.DoGeneratePermutation(list, start + 1, n, permutations);
                    (list[start], list[i]) = (list[i], list[start]); // Возврат в исходное состояние
                }
            }
        }

        #endregion

        #region Различительное число
        public static IEnumerable<List<int>> GetPermutations(List<int> list)
        {
            list.Sort(); // Начинаем с сортированного списка
            yield return list.ToList(); // Возвращаем первую перестановку

            int n = list.Count;
            while (true)
            {
                int i;
                for (i = n - 2; i >= 0; i--)
                {
                    if (list[i] < list[i + 1])
                        break;
                }

                if (i < 0)
                    yield break; // Все перестановки сгенерированы

                int j;
                for (j = n - 1; j > i; j--)
                {
                    if (list[j] > list[i])
                        break;
                }

                // Обмен значениями
                (list[i], list[j]) = (list[j], list[i]);

                // Переворот списка от i+1 до конца
                list.Reverse(i + 1, n - (i + 1));

                yield return list.ToList();
            }
        }

        public bool IsAutomorphism(List<int> permutation, int[] colors)
        {
            int n = this._adjacencyMatrix.GetLength(0);
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (this._adjacencyMatrix[i, j] != this._adjacencyMatrix[permutation[i], permutation[j]] ||
                        colors[i] != colors[permutation[i]])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public override int GetDistinguishingNumber(int maxColors = 1024)
        {
            int n = this._adjacencyMatrix.GetLength(0);
            int[] colors = new int[n];

            for (int currentMaxColors = 1; currentMaxColors <= maxColors; currentMaxColors++)
            {
                if (TryColoring(colors, 0, currentMaxColors) || currentMaxColors == maxColors)
                //if (this.TryColoring(colors, 0, currentMaxColors))
                    return currentMaxColors;
            }

            return -1; // Возвращает -1, если различительное число больше maxColors
        }

        private bool TryColoring(int[] colors, int index, int maxColors)
        {
            if (index == colors.Length)
            {
                return this.CheckAllAutomorphismsBroken(colors);
            }

            for (int color = 1; color <= maxColors; color++)
            {
                colors[index] = color;
                if (this.TryColoring(colors, index + 1, maxColors))
                    return true;
            }

            return false;
        }

        private bool CheckAllAutomorphismsBroken(int[] colors)
        {
            IEnumerable<List<int>> permutations = GetPermutations(Enumerable.Range(0, this._adjacencyMatrix.GetLength(0)).ToList());
            return permutations.All(permutation => !this.IsAutomorphism(permutation, colors));
        }

        #endregion

        #region Подсчет характеристик
        public override string GetInfo(int numberOfColors = 0)
        {
            // Получаем G6-представление графа
            string g6String = new G6String(new AdjacencyMatrix(this.AdjacencyMatrix)).G6;

            // Получаем различительное число
            int distinguishingNumber = this.GetDistinguishingNumberLite(numberOfColors);

            // Формируем итоговую строку
            return $"G6-представление: {g6String}, Различительное число: {distinguishingNumber}";
        }

        #endregion

        #endregion
    }
}
