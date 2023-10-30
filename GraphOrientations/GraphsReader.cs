using System.Collections.Generic;
using System.Diagnostics;

namespace GraphOrientations
{
    internal class GraphsReader
    {
        public IEnumerable<string> ReadGraphs(int vertexCount)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "geng.exe",
                Arguments = $"{vertexCount} -c",
                UseShellExecute = false,
                RedirectStandardOutput = true
            };
            using var compiler = new Process { StartInfo = startInfo };
            compiler.Start();
            while (!compiler.StandardOutput.EndOfStream)
            {
                var current = compiler.StandardOutput.ReadLine();
                if (!string.IsNullOrWhiteSpace(current))
                {
                    yield return current;
                }
            }
        }


    }
}
