using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphOrientations
{
    internal class GraphOrientator
    {

        #region Поля

        #endregion

        #region Свойства
        private string PathStandart { set; get; }
        private string PathTempFolder { set; get; }
        private string PathTempGraphs { set; get; }
        private string PathTempDirectGraphs { set; get; }
        private string PathTrash { set; get; }
        private Encoding FileEncoding { set; get; }
        #endregion

        #region Методы
        public IEnumerable<int> OrientWithoutGraphs(string strGraphG6, bool isUsingNauty)
        {
            if (isUsingNauty)
            {
                return this.OrientWithoutGraphs(strGraphG6);
            }
            else
            {
                return this.OrientWithoutGraphs(new Graph(strGraphG6).AdjacencyMatrix);
            }
        }

        #region C использованием nauty
        public IEnumerable<int> OrientWithoutGraphs(string graph6)
        {
            // Генерируем уникальные имена файлов для каждого потока
            var tempGraphsFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var tempDirectGraphsFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var trashFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            try
            {
                File.WriteAllText(tempGraphsFile, graph6 + '\n', this.FileEncoding);
                this.ExecuteProcess("directg.exe", $"-o {tempGraphsFile} {tempDirectGraphsFile}");

                var result = this.ExecuteProcess("pickg.exe", $"--a -V {tempDirectGraphsFile} {trashFile}", true);

                return result
                    .Where(line => line.Contains('='))
                    .Select(line => int.Parse(line.Split('=')[^1]))
                    .ToList();
            }
            finally
            {
                // Удаляем временные файлы после использования
                File.Delete(tempGraphsFile);
                File.Delete(tempDirectGraphsFile);
                File.Delete(trashFile);
            }
        }


        private List<string> ExecuteProcess(string fileName, string arguments, bool readFromErrorStream = false)
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                RedirectStandardError = true
            };

            var result = new List<string>();

            using (var process = Process.Start(processStartInfo))
            {
                var streamReader = readFromErrorStream ? process.StandardError : process.StandardOutput;
                string line;

                while ((line = streamReader.ReadLine()) != null)
                {
                    result.Add(line);
                }
            }

            return result;
        }

        private Encoding GetEncoding(string filename)
        {
            var bom = new byte[4];
            using (var file = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                file.Read(bom, 0, 4);
            }

            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76)
                return Encoding.UTF7;
            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf)
                return Encoding.UTF8;
            if (bom[0] == 0xff && bom[1] == 0xfe && bom[2] == 0 && bom[3] == 0)
                return Encoding.UTF32;
            if (bom[0] == 0xff && bom[1] == 0xfe)
                return Encoding.Unicode;
            if (bom[0] == 0xfe && bom[1] == 0xff)
                return Encoding.BigEndianUnicode;
            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff)
                return new UTF32Encoding(true, true);
            return Encoding.ASCII;
        }
        #endregion

        #region Без использования nauty
        public IEnumerable<(int[] graph, int groupSize)> Orient(int[] graph)
        {
            var codes = new HashSet<long>();
            var substitutions = Utils.EnumerateAllSubstitutions(graph.Length).ToArray();
            var results = new ConcurrentBag<(int[] graph, int groupSize)>();

            Parallel.ForEach(this.OrientInternal(graph), orientedGraph =>
            {
                var code = Utils.GetGraphCode(orientedGraph);

                if (!codes.Add(code))
                {
                    return;
                }

                var maxCode = -1L;
                var groupSize = 1;
                foreach (var substitution in substitutions)
                {
                    var currentGraph = Utils.UseSubstitution(orientedGraph, substitution);
                    var currentCode = Utils.GetGraphCode(currentGraph);

                    if (currentCode > maxCode)
                    {
                        maxCode = currentCode;
                        groupSize = 1;
                    }
                    else if (currentCode == maxCode)
                    {
                        groupSize++;
                    }

                    codes.Add(currentCode);
                }

                results.Add((orientedGraph, groupSize));
            });

            return results;
        }

        public IEnumerable<int> OrientWithoutGraphs(int[] graph)
        {
            var codes = new HashSet<long>();
            var substitutions = Utils.EnumerateAllSubstitutions(graph.Length).ToArray();
            var groupSizes = new ConcurrentBag<int>();

            Parallel.ForEach(this.OrientInternal(graph), orientedGraph =>
            {
                var code = Utils.GetGraphCode(orientedGraph);

                if (!codes.Add(code))
                {
                    return;
                }

                var maxCode = -1L;
                var groupSize = 1;
                foreach (var substitution in substitutions)
                {
                    var currentGraph = Utils.UseSubstitution(orientedGraph, substitution);
                    var currentCode = Utils.GetGraphCode(currentGraph);

                    if (currentCode > maxCode)
                    {
                        maxCode = currentCode;
                        groupSize = 1;
                    }
                    else if (currentCode == maxCode)
                    {
                        groupSize++;
                    }

                    codes.Add(currentCode);
                }

                groupSizes.Add(groupSize);
            });

            return groupSizes;
        }

        private IEnumerable<int[]> OrientInternal(int[] graph, int from = 0, int to = 1)
        {
            var results = new List<int[]>();

            void OrientInternalRecursive(int[] inputGraph, int fromIndex, int toIndex)
            {
                if (fromIndex >= inputGraph.Length)
                {
                    results.Add(inputGraph);
                    return;
                }

                var toMask = 1 << toIndex;
                int nextFrom = toIndex >= inputGraph.Length - 1 ? fromIndex + 1 : fromIndex;
                int nextTo = toIndex == inputGraph.Length - 1 ? nextFrom + 1 : toIndex + 1;

                if ((inputGraph[fromIndex] & toMask) != 0)
                {
                    var fromMask = 1 << fromIndex;
                    inputGraph[toIndex] ^= fromMask;
                    OrientInternalRecursive(inputGraph, nextFrom, nextTo);
                    inputGraph[toIndex] ^= fromMask;

                    inputGraph[fromIndex] ^= toMask;
                    OrientInternalRecursive(inputGraph, nextFrom, nextTo);
                    inputGraph[fromIndex] ^= toMask;
                }
                else
                {
                    OrientInternalRecursive(inputGraph, nextFrom, nextTo);
                }
            }

            OrientInternalRecursive(graph, from, to);

            return results;
        }
        #endregion

        #endregion

        #region Конструкторы/Деструкторы
        public GraphOrientator()
        {
            this.PathStandart = "standard.txt";
            this.FileEncoding = this.GetEncoding(this.PathStandart);
            var processId = Process.GetCurrentProcess().Id;
            this.PathTempFolder = "temp\\";
            this.PathTempGraphs = $"{this.PathTempFolder}temp_graphs_{processId}.txt";
            this.PathTempDirectGraphs = $"{this.PathTempFolder}temp_direct_graphs_{processId}.txt";
            this.PathTrash = $"{this.PathTempFolder}trash_{processId}.txt";
        }
        #endregion

        #region Операторы

        #endregion

        #region Обработчики событий

        #endregion

    }
}

