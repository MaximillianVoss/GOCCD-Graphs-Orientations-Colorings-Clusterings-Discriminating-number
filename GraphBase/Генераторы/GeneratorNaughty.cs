using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var arguments = GetArguments(vertexCount, generatorType);
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

        // Остальные методы и реализации перенесены из предыдущего класса GeneratorBase
        // ...
    }
}
