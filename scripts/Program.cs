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

        var wdwFiles = Directory.GetFiles(inputDirectory, "*.wdw", SearchOption.AllDirectories);

        foreach (var inputFile in wdwFiles)
        {
            var fileName = Path.GetFileNameWithoutExtension(inputFile);
            var outputFile = Path.Combine(outputDirectory, $"{fileName}_Code.txt");

            var lines = File.ReadAllLines(inputFile);
            var currentCode = new StringBuilder();
            bool isInCodeBlock = false;
            var tempBlock = new List<string>();
            int lineNumber = 1;

            foreach (var line in lines)
            {
                if (line.Trim().StartsWith("code : |1+"))
                {
                    isInCodeBlock = true;
                    tempBlock.Clear();
                    continue;
                }

                if (isInCodeBlock)
                {
                    if (string.IsNullOrWhiteSpace(line) || line.Trim().StartsWith("type :"))
                    {
                        isInCodeBlock = false;

                        if (tempBlock.Any(l => !string.IsNullOrWhiteSpace(l)))
                        {
                            foreach (var codeLine in tempBlock)
                            {
                                if (!string.IsNullOrWhiteSpace(codeLine))
                                {
                                    currentCode.AppendLine($"{lineNumber.ToString().PadLeft(4)}: {codeLine}");
                                    lineNumber++;
                                }
                            }
                            currentCode.AppendLine();
                        }
                    }
                    else
                    {
                        tempBlock.Add(line);
                    }
                }
            }

            File.WriteAllText(outputFile, currentCode.ToString());
            Console.WriteLine($"Code extrait depuis {inputFile} vers : {outputFile}");
        }
    }
}

