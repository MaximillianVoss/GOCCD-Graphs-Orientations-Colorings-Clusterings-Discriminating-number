using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GraphBase.Генераторы
{
    public class GeneratorNaughty : GraphGeneratorBase
    {
        #region Поля
        private string gengPath;
        #endregion

        #region Конструкторы/Деструкторы
        public GeneratorNaughty(int vertexCount, string gengPath = "geng.exe")
            : base(vertexCount)
        {
            this.gengPath = gengPath;
        }
        #endregion

        public override IEnumerable<string> GenerateAllGraphsG6()
        {
            throw new NotImplementedException();
        }

        // Остальные методы и реализации перенесены из предыдущего класса GeneratorBase
        // ...

        public override IEnumerable<string> GenerateAllGraphsG6(int vertexCount, GeneratorType generatorType)
        {
            string arguments = this.GetArguments(vertexCount, generatorType);
            var startInfo = new ProcessStartInfo(this.gengPath, arguments)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = Process.Start(startInfo))
            {
                while (!process.StandardOutput.EndOfStream)
                {
                    yield return process.StandardOutput.ReadLine();
                }
            }
        }

        private string GetArguments(int vertexCount, GeneratorType generatorType)
        {
            switch (generatorType)
            {
                case GeneratorType.BY_CANONICAL_CODE:
                    return $"{vertexCount} -c";
                case GeneratorType.CONNECTED_GRAPHS:
                    return $" -c {vertexCount}";
                case GeneratorType.BRUTE_FORCE_ALL_GRAPHS:
                    return $"{vertexCount}";
                case GeneratorType.BRUTE_FORCE_ALL_CODES:
                    // Пример опций для BRUTE_FORCE_ALL_CODES
                    return $"{vertexCount} -b -optionX";
                case GeneratorType.BRUTE_FORCE_ALL_CODES_WITH_FILTER:
                    // Пример опций для BRUTE_FORCE_ALL_CODES_WITH_FILTER
                    return $"{vertexCount} -bf -filterOption1 -filterOption2";
                default:
                    throw new ArgumentException("Invalid generator type", nameof(generatorType));
            }
        }
    }
}
