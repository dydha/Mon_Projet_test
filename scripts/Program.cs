using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

class WinDevCodeExtractor
{
    static void Main(string[] args)
    {
        //string inputFile = "C:\\Users\\DydhaMPANDOU\\Downloads\\FEN_LayoutBoh.wdw";
        //string outputFolder = "C:\\Users\\DydhaMPANDOU\\Downloads\\ExtracteFiles";



        if (args.Length < 2)
        {
            Console.WriteLine("Usage: ExtractCode <inputFile> <outputFile>");
            return;
        }
        string inputFile = args[0];
        string outputFolder =  args[1];

        if (!Directory.Exists(outputFolder))
            Directory.CreateDirectory(outputFolder);

        var lines = File.ReadAllLines(inputFile);
        var outputFile = Path.Combine(outputFolder, "FEN_Layout_Code.txt");
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
        Console.WriteLine($"Code extrait avec numéros de ligne dans : {outputFile}");



    }
}
