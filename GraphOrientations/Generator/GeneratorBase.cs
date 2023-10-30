using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GraphOrientations.Generator
{
    public class GeneratorBase
    {


        #region Поля
        private string gengPath;
        #endregion

        #region Свойства

        #endregion

        #region Методы

        #region Абстрактные
        /// <summary>
        /// Генерирует графы в формате G6
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GenerateGraphG6(int vertexCount, bool isNaughy = true) { throw new NotImplementedException(); }
        /// <summary>
        /// Генерирует графы с указанным числом вершин
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Graph> GenerateGraph(int vertexCount, bool isNaughy = true) { throw new NotImplementedException(); }

        protected IEnumerable<Graph> GenerateGraph() { throw new NotImplementedException(); }
        #endregion

        #region Общие
        public static List<int> ToAjentityVector(long longView, int vertexCount)
        {
            var result = new List<int>(new int[vertexCount]);

            int mask = 1 << (vertexCount - 1);
            for (int i = vertexCount - 1; i >= 0; i--, mask >>= 1)
            {
                for (int j = vertexCount - 1, jMask = mask; j > i; j--, jMask >>= 1)
                {
                    if ((longView & 1) == 1)
                    {
                        result[i] |= jMask;
                        result[j] |= mask;
                    }

                    longView >>= 1;
                }
            }

            return result;
        }

        public static List<int> ToAjentityVectorByMaxCode(long longView, int vertexCount)
        {
            var result = new List<int>(new int[vertexCount]);

            for (int i = 0, iMask = 1; i < vertexCount; i++, iMask <<= 1)
            {
                for (int j = i + 1, jMask = iMask << 1; j < vertexCount; j++, jMask <<= 1)
                {
                    if ((longView & 1) == 1)
                    {
                        result[i] |= jMask;
                        result[j] |= iMask;
                    }

                    longView >>= 1;
                }
            }

            return result;
        }

        public static List<int> UseSubstitution(List<int> vector, List<int> substitution)
        {
            var n = vector.Count;
            var result = new List<int>(new int[n]);

            for (int i = 0; i < n; i++)
            {
                for (int j = 0, jMask = 1; j < n; j++, jMask <<= 1)
                {
                    if ((vector[i] & jMask) == 0)
                    {
                        continue;
                    }

                    result[substitution[i]] |= 1 << substitution[j];
                    result[substitution[j]] |= 1 << substitution[i];
                }
            }

            return result;
        }

        public static long GetSimpleCode(List<int> vector)
        {
            var result = 0L;

            var currentMask = 1;
            for (int i = 0; i < vector.Count; i++)
            {
                for (int j = i + 1, jMask = 1 << (i + 1); j < vector.Count; j++, jMask <<= 1)
                {
                    if ((vector[i] & jMask) != 0)
                    {
                        result ^= currentMask;
                    }

                    currentMask <<= 1;
                }
            }

            return result;
        }

        public static long GetMaxCode(List<int> vector, int bitsCount)
        {
            var result = 0L;

            var currentMask = 1L << bitsCount;
            for (int i = 0; i < vector.Count; i++)
            {
                for (int j = i + 1, jMask = 1 << (i + 1); j < vector.Count; j++, jMask <<= 1)
                {
                    if ((vector[i] & jMask) != 0)
                    {
                        result ^= currentMask;
                    }

                    currentMask >>= 1;
                }
            }

            return result;
        }

        #endregion

        #region Geng
        private string GetArguments(int vertexCount, GeneratorType generatorType)
        {
            switch (generatorType)
            {
                case GeneratorType.GENERATOR_BY_CANONICAL_CODE:
                    return $"{vertexCount} -c";
                case GeneratorType.BRUTE_FORCE_ALL_GRAPHS:
                    return $"{vertexCount}";
                case GeneratorType.BRUTE_FORCE_ALL_CODES:
                    // Опции для этого типа генератора будут зависеть от вашего конкретного использования
                    throw new NotImplementedException();
                case GeneratorType.BRUTE_FORCE_ALL_CODES_WITH_FILTER:
                    // Опции для этого типа генератора будут зависеть от вашего конкретного использования
                    throw new NotImplementedException();
                default:
                    throw new ArgumentException("Invalid generator type", nameof(generatorType));
            }
        }
        public IEnumerable<string> GenerateGraphs(int vertexCount, GeneratorType generatorType)
        {
            var arguments = this.GetArguments(vertexCount, generatorType);
            var startInfo = new ProcessStartInfo(this.gengPath, arguments)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(startInfo);
            while (!process.StandardOutput.EndOfStream)
            {
                yield return process.StandardOutput.ReadLine();
            }
        }
        #endregion

        #endregion

        #region Конструкторы/Деструкторы
        public GeneratorBase(string gengPath = "geng.exe")
        {
            this.gengPath = gengPath;
        }
        #endregion

        #region Операторы

        #endregion

        #region Обработчики событий

        #endregion


    }
}
