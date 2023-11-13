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

        public int GetAutomorphismGroupSizeWithColors(string graphRepresentation, int[] colors)
        {
            using var process = new Process();
            process.StartInfo.FileName = "dreadnaut";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();

            // Отправляем представление графа
            process.StandardInput.WriteLine(graphRepresentation);

            // Отправляем раскраску
            string colorsString = $"c {string.Join(" ", colors)}";
            process.StandardInput.WriteLine(colorsString);

            // Отправляем команды для получения размера группы автоморфизмов
            process.StandardInput.WriteLine("x y z"); // Команды могут изменяться в зависимости от требований

            // Чтение и обработка вывода
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            // Обработка вывода для получения размера группы автоморфизмов
            // (Необходима дополнительная логика)
            var digits = output.Split('=').Last().TakeWhile(char.IsDigit);
            return int.Parse(new string(digits.ToArray()));
        }


    }
}
