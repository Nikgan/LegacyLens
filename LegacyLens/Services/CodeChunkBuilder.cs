using LegacyLens.Models;

namespace LegacyLens.Services;

public class CodeChunkBuilder
{
    public List<CodeChunk> Build(string relativePath, List<CodeItem> codeItems, string[] lines)
    {
        List<CodeChunk> codeChunks = new List<CodeChunk>();

        foreach (CodeItem codeItem in codeItems)
        {
            string text = GetTextRange(
                lines,
                codeItem.LineNumber,
                codeItem.EndLineNumber
            );

            CodeChunk codeChunk = new CodeChunk()
            {
                Id = BuildChunkId(relativePath, codeItem),
                Kind = codeItem.Kind,
                Name = codeItem.Name,
                StartLineNumber = codeItem.LineNumber,
                EndLineNumber = codeItem.EndLineNumber,
                Text = text
            };

            codeChunks.Add(codeChunk);
        }

        return codeChunks;
    }

    private static string BuildChunkId(string relativePath, CodeItem codeItem)
    {
        string normalizedPath = NormalizePath(relativePath);
        string kindText = codeItem.Kind.ToString().ToLowerInvariant();
        string nameText = string.IsNullOrWhiteSpace(codeItem.Name)
            ? "unknown"
            : codeItem.Name;

        return $"{normalizedPath}#{kindText}:{nameText}:{codeItem.LineNumber}-{codeItem.EndLineNumber}";
    }

    private static string NormalizePath(string path)
    {
        return path.Replace('\\', '/');
    }

    private static string GetTextRange(string[] lines, int startLineNumber, int endLineNumber)
    {
        int startIndex = Math.Max(startLineNumber - 1, 0);
        int endIndex = Math.Min(endLineNumber - 1, lines.Length - 1);

        if (startIndex > endIndex)
        {
            return "";
        }

        List<string> selectedLines = new List<string>();

        for (int i = startIndex; i <= endIndex; i++)
        {
            selectedLines.Add(lines[i]);
        }

        return string.Join(Environment.NewLine, selectedLines);
    }
}