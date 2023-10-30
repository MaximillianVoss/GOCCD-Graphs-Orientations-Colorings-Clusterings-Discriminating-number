using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace GraphOrientations
{
    internal class NautyGraphOrientator
    {
        private const string Etalon = "etalon.txt";
        private string TempGraphsFileName;
        private string TempDirectGraphsFileName;
        private string TrashFileName;
        private Encoding _encoding;

        public NautyGraphOrientator()
        {
            this._encoding = this.GetEncoding(Etalon);
            var processId = Process.GetCurrentProcess().Id;
            this.TempGraphsFileName = $"temp_graphs_{processId}.txt";
            this.TempDirectGraphsFileName = $"temp_direct_graphs_{processId}.txt";
            this.TrashFileName = $"trash_{processId}.txt";
        }

        public IEnumerable<int> OrientWithoutGrahps(string graph6)
        {
            File.WriteAllText(this.TempGraphsFileName, graph6 + '\n', this._encoding);
            this.ExecuteProcess("directg.exe", $"-o {this.TempGraphsFileName} {this.TempDirectGraphsFileName}");

            var result = this.ExecuteProcess("pickg.exe", $"--a -V {this.TempDirectGraphsFileName} {this.TrashFileName}", true);

            return result
                .Where(line => line.Contains('='))
                .Select(line => int.Parse(line.Split('=')[^1]))
                .ToList();
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
    }
}
