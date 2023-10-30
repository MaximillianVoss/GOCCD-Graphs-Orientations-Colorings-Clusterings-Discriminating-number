using CommandLine;

namespace GraphOrientations
{
    internal class Options
    {
        [Option('C', "Colors", Required = true, HelpText = "Colors count.")]
        public int ColorsCount { get; set; }

        [Option('v', "VertexCount", Required = true, HelpText = "Vertex count.")]
        public int VertexCount { get; set; }

        [Option('w', "WriteToFile", Default = false, HelpText = "Print all graphs to file.")]
        public bool WriteGraphsToFile { get; set; }

        [Option('f', "FileName", Default = "Graphs.txt", HelpText = "File name for writing graphs. Will be user if parameter -w is true.")]
        public string FileName { get; set; }

        [Option('c', "CalculateOnly", Default = true, HelpText = "Do not output graphs (calculate total count only).")]
        public bool CalculateOnly { get; set; }

        [Option('n', "UseNauty", Default = false, HelpText = "Use nauty for orient graphs and calculate groupSize of oriented grahps.")]
        public bool NautyCalculation { get; set; }
    }
}
