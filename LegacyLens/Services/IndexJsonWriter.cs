using System.Text.Json;
using LegacyLens.Models;

namespace LegacyLens.Services;

public class IndexJsonWriter
{
    public void Save(CodebaseIndex codebaseIndex, string outputPath)
    {
        JsonSerializerOptions jsonOptions = new JsonSerializerOptions()
        {
            WriteIndented = true
        };

        string? outputDirectoryPath = Path.GetDirectoryName(outputPath);

        if (!string.IsNullOrWhiteSpace(outputDirectoryPath))
        {
            Directory.CreateDirectory(outputDirectoryPath);
        }

        string json = JsonSerializer.Serialize(codebaseIndex, jsonOptions);

        File.WriteAllText(outputPath, json);

        Console.WriteLine($"Index saved to: {outputPath}");
    }
}