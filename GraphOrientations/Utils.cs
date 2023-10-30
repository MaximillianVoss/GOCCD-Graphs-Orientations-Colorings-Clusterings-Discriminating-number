using System.Collections.Generic;

namespace GraphOrientations
{
    internal static class Utils
    {
        public static IEnumerable<int[]> EnumerateAllSubstitutions(int n)
        {
            var vertexNumbers = new int[n];
            bool[] used = new bool[n];

            return Enumerate(used, 0);

            IEnumerable<int[]> Enumerate(bool[] used, int deep)
            {
                if (deep == n)
                {
                    yield return (int[])vertexNumbers.Clone();
                }
                else
                {
                    for (int i = 0; i < n; i++)
                    {
                        if (!used[i])
                        {
                            used[i] = true;
                            vertexNumbers[deep] = i;

                            foreach (var val in Enumerate(used, deep + 1))
                                yield return val;

                            used[i] = false;
                        }
                    }
                }
            }
        }

        public static int[] UseSubstitution(int[] graph, int[] substitution)
        {
            int length = graph.Length;
            var result = new int[length];

            for (int i = 0; i < length; i++)
            {
                for (int j = 0, jmask = 1; j < length; j++, jmask <<= 1)
                {
                    if ((graph[i] & jmask) != 0)
                    {
                        result[substitution[i]] |= 1 << substitution[j];
                    }
                }
            }

            return result;
        }

        public static long GetGraphCode(int[] graph)
        {
            int length = graph.Length;
            //if (length > 8)
            //{
            //    throw new ArgumentException("Method doesn't work with graphs 9 or more degree.");
            //}

            long result = 0L;
            long currentMask = 1L;

            for (int i = 0; i < length; i++)
            {
                for (int j = 0, jmask = 1; j < length; j++, jmask <<= 1)
                {
                    if (i != j && (graph[i] & jmask) != 0)
                    {
                        result |= currentMask;
                    }

                    currentMask <<= 1;
                }
            }

            return result;
        }
    }

}
