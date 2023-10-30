using System.Diagnostics;
using System.Linq;

namespace GraphOrientations
{
    internal class AutomorphismGroupRepository
    {
        public int GetNextAutomorphismGroupSize(string graphRepresentation)
        {
            using var processInfo = new Process();
            processInfo.StartInfo.FileName = "pickg.exe";
            processInfo.StartInfo.Arguments = $"-V --a";
            processInfo.StartInfo.UseShellExecute = false;
            processInfo.StartInfo.RedirectStandardOutput = true;
            processInfo.StartInfo.RedirectStandardInput = true;
            processInfo.StartInfo.RedirectStandardError = true;
            processInfo.Start();

            processInfo.StandardInput.WriteLine(graphRepresentation + '\n');
            processInfo.StandardInput.Flush();

            string errorLine;
            do
            {
                errorLine = processInfo.StandardError.ReadLine();
            } while (!errorLine.Contains('='));

            processInfo.WaitForExit();

            var digits = errorLine.Split('=').Last().TakeWhile(char.IsDigit);
            return int.Parse(new string(digits.ToArray()));
        }

    }
}
