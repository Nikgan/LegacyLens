using System.Text.Json;
using LegacyLens.Models;

namespace LegacyLens.Services;

public class IndexJsonWriter
{
    public void Save(CodebaseIndex codebaseIndex, string rootPath)
    {
        JsonSerializerOptions jsonOptions = new JsonSerializerOptions()
        {
            WriteIndented = true
        };

        string outputPath = Path.Combine(rootPath, "index.json");

        string json = JsonSerializer.Serialize(codebaseIndex, jsonOptions);

        File.WriteAllText(outputPath, json);

        Console.WriteLine($"Index saved to: {outputPath}");
    }
}