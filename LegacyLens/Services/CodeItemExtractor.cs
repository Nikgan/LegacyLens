using LegacyLens.Models;

namespace LegacyLens.Services;

public class CodeItemExtractor
{
    public List<CodeItem> Extract(string extension, string[] lines)
    {
        List<CodeItem> codeItems = new List<CodeItem>();

        for (int i = 0; i < lines.Length; i++)
        {
            string trimmedLine = lines[i].Trim();

            if (string.IsNullOrWhiteSpace(trimmedLine))
            {
                continue;
            }

            if (trimmedLine.StartsWith("//"))
            {
                continue;
            }

            CodeItem? codeItem = null;

            if (extension == ".prg")
            {
                codeItem = TryCreateHarbourCodeItem(trimmedLine, i + 1);
            }

            if (extension == ".cs")
            {
                codeItem = TryCreateCSharpCodeItem(trimmedLine, i + 1);
            }

            if (codeItem is not null)
            {
                codeItems.Add(codeItem);
            }
        }

        return codeItems;
    }

    private static CodeItem? TryCreateHarbourCodeItem(string line, int lineNumber)
    {
        if (line.StartsWith("function ", StringComparison.OrdinalIgnoreCase))
        {
            return CreateCodeItem("function", line, lineNumber);
        }

        if (line.StartsWith("procedure ", StringComparison.OrdinalIgnoreCase))
        {
            return CreateCodeItem("procedure", line, lineNumber);
        }

        if (line.StartsWith("method ", StringComparison.OrdinalIgnoreCase))
        {
            return CreateCodeItem("method", line, lineNumber);
        }

        return null;
    }

    private static CodeItem? TryCreateCSharpCodeItem(string line, int lineNumber)
    {
        if (line.StartsWith("public class ") ||
            line.StartsWith("internal class ") ||
            line.StartsWith("private class ") ||
            line.StartsWith("protected class ") ||
            line.StartsWith("class "))
        {
            return CreateCodeItem("class", line, lineNumber);
        }

        if (line.StartsWith("public record ") ||
            line.StartsWith("internal record ") ||
            line.StartsWith("private record ") ||
            line.StartsWith("record "))
        {
            return CreateCodeItem("record", line, lineNumber);
        }

        if (line.StartsWith("public interface ") ||
            line.StartsWith("internal interface ") ||
            line.StartsWith("interface "))
        {
            return CreateCodeItem("interface", line, lineNumber);
        }

        return null;
    }

    private static CodeItem CreateCodeItem(string kind, string signature, int lineNumber)
    {
        string name = ExtractNameFromSignature(kind, signature);

        CodeItem codeItem = new CodeItem()
        {
            Kind = kind,
            Name = name,
            LineNumber = lineNumber,
            Signature = signature
        };

        return codeItem;
    }

    private static string ExtractNameFromSignature(string kind, string signature)
    {
        string[] parts = signature.Split(
            ' ',
            StringSplitOptions.RemoveEmptyEntries
        );

        for (int i = 0; i < parts.Length; i++)
        {
            if (!string.Equals(parts[i], kind, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            int nameIndex = i + 1;

            if (nameIndex >= parts.Length)
            {
                return "";
            }

            return CleanCodeItemName(parts[nameIndex]);
        }

        return "";
    }

    private static string CleanCodeItemName(string name)
    {
        int parenthesisIndex = name.IndexOf('(');

        if (parenthesisIndex >= 0)
        {
            name = name[..parenthesisIndex];
        }

        int genericIndex = name.IndexOf('<');

        if (genericIndex >= 0)
        {
            name = name[..genericIndex];
        }

        return name.Trim('{', ':', ';');
    }
}