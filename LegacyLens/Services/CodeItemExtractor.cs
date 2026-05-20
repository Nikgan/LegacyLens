using LegacyLens.Models;

namespace LegacyLens.Services;

public class CodeItemExtractor
{
    public List<CodeItem> Extract(string extension, string[] lines)
    {
        List<CodeItem> codeItems = new List<CodeItem>();

        bool isInsideHarbourClassDeclarationBlock = false;

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
                if (IsHarbourClassEndLine(trimmedLine))
                {
                    isInsideHarbourClassDeclarationBlock = false;
                    continue;
                }

                codeItem = TryCreateHarbourCodeItem(
                    trimmedLine,
                    i + 1,
                    isInsideHarbourClassDeclarationBlock
                );

                if (IsHarbourClassStartLine(trimmedLine))
                {
                    isInsideHarbourClassDeclarationBlock = true;
                }
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

        return ApplyEndLineNumbers(codeItems, lines.Length);
    }

    private static CodeItem? TryCreateHarbourCodeItem(
    string line,
    int lineNumber,
    bool isInsideClassDeclarationBlock
)
    {
        if (IsHarbourClassStartLine(line))
        {
            return CreateCodeItem(CodeItemKind.Class, line, lineNumber);
        }

        if (line.StartsWith("function ", StringComparison.OrdinalIgnoreCase))
        {
            return CreateCodeItem(CodeItemKind.Function, line, lineNumber);
        }

        if (line.StartsWith("procedure ", StringComparison.OrdinalIgnoreCase))
        {
            return CreateCodeItem(CodeItemKind.Procedure, line, lineNumber);
        }

        if (line.StartsWith("method ", StringComparison.OrdinalIgnoreCase) &&
            !isInsideClassDeclarationBlock)
        {
            return CreateCodeItem(CodeItemKind.Method, line, lineNumber);
        }

        return null;
    }

    private static bool IsHarbourClassStartLine(string line)
    {
        return line.StartsWith("class ", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsHarbourClassEndLine(string line)
    {
        return string.Equals(line, "endclass", StringComparison.OrdinalIgnoreCase);
    }

    private static CodeItem? TryCreateCSharpCodeItem(string line, int lineNumber)
    {
        string[] parts = SplitSignature(line);

        foreach (string part in parts)
        {
            if (part == "class")
            {
                return CreateCodeItem(CodeItemKind.Class, line, lineNumber);
            }

            if (part == "record")
            {
                return CreateCodeItem(CodeItemKind.Record, line, lineNumber);
            }

            if (part == "interface")
            {
                return CreateCodeItem(CodeItemKind.Interface, line, lineNumber);
            }
        }

        return null;
    }
    private static string[] SplitSignature(string signature)
    {
        char[] separators = [' ', '\t'];

        return signature.Split(
            separators,
            StringSplitOptions.RemoveEmptyEntries
        );
    }

    private static CodeItem CreateCodeItem(CodeItemKind kind, string signature, int lineNumber)
    {
        string name = ExtractNameFromSignature(kind, signature);

        CodeItem codeItem = new CodeItem()
        {
            Kind = kind,
            Name = name,
            LineNumber = lineNumber,
            EndLineNumber = lineNumber,
            Signature = signature
        };

        return codeItem;
    }

    private static string ExtractNameFromSignature(CodeItemKind kind, string signature)
    {
        string kindText = GetKindText(kind);
        string[] parts = SplitSignature(signature);

        for (int i = 0; i < parts.Length; i++)
        {
            if (!string.Equals(parts[i], kindText, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            int nameIndex = i + 1;

            if (kind == CodeItemKind.Record &&
                nameIndex < parts.Length &&
                (parts[nameIndex] == "class" || parts[nameIndex] == "struct"))
            {
                nameIndex++;
            }

            if (nameIndex >= parts.Length)
            {
                return "";
            }

            return CleanCodeItemName(parts[nameIndex]);
        }

        return "";
    }

    private static string GetKindText(CodeItemKind kind)
    {
        return kind switch
        {
            CodeItemKind.Class => "class",
            CodeItemKind.Record => "record",
            CodeItemKind.Interface => "interface",
            CodeItemKind.Function => "function",
            CodeItemKind.Procedure => "procedure",
            CodeItemKind.Method => "method",
            _ => ""
        };
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
    private static List<CodeItem> ApplyEndLineNumbers(List<CodeItem> codeItems, int totalLineCount)
    {
        List<CodeItem> result = new List<CodeItem>();

        for (int i = 0; i < codeItems.Count; i++)
        {
            CodeItem currentCodeItem = codeItems[i];

            int endLineNumber = totalLineCount;

            if (i + 1 < codeItems.Count)
            {
                CodeItem nextCodeItem = codeItems[i + 1];
                endLineNumber = nextCodeItem.LineNumber - 1;
            }

            CodeItem updatedCodeItem = currentCodeItem with
            {
                EndLineNumber = endLineNumber
            };

            result.Add(updatedCodeItem);
        }

        return result;
    }
}