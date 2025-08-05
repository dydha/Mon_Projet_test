using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;

class WinDevCodeExtractor
{
    static void Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Usage: ExtractCode <inputDirectory> <outputDirectory>");
            return;
        }

        string inputDirectory = args[0];
        string outputDirectory = args[1];

        if (!Directory.Exists(outputDirectory))
            Directory.CreateDirectory(outputDirectory);

        var wdwFiles = Directory.GetFiles(inputDirectory, "*.wdw");

        foreach (var inputFile in wdwFiles)
        {
            var fileName = Path.GetFileNameWithoutExtension(inputFile);
            var outputFile = Path.Combine(outputDirectory, $"{fileName}_Code.txt");

            var lines = File.ReadAllLines(inputFile);
            var currentCode = new StringBuilder();
            bool isInCodeBlock = false;
            int lineNumber = 1;

            foreach (var line in lines)
            {
                var trimmed = line.TrimStart();

                if (trimmed.StartsWith("code : |1+")
                    || trimmed.StartsWith("code : |1-"))
                {
                    isInCodeBlock = true;
                    continue;
                }

                if (isInCodeBlock)
                {
                    // Si on tombe sur une propriété ou une autre section, on arrête
                    if (trimmed.StartsWith("type :") || trimmed.StartsWith("name :") || trimmed.StartsWith("enabled :") || trimmed.StartsWith("procedure_id :"))
                    {
                        isInCodeBlock = false;
                        currentCode.AppendLine(); // séparation entre blocs
                        continue;
                    }

                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        currentCode.AppendLine($"{lineNumber.ToString().PadLeft(4)}: {line.TrimEnd()}");
                        lineNumber++;
                    }
                }
            }

            File.WriteAllText(outputFile, currentCode.ToString());
            Console.WriteLine($"✅ Code extrait depuis {inputFile} vers : {outputFile}");
        }
    }
}

