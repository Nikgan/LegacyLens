using LegacyLens.Models;

namespace LegacyLens.Services;

public class FileScanner
{
    public List<FileIndexEntry> Scan(
        string rootPath,
        string[] allowedPatterns,
        string[] excludedDirectoryNames)
    {
        List<FileIndexEntry> entries = new List<FileIndexEntry>();

        foreach (string pattern in allowedPatterns)
        {
            string[] matchedFiles = Directory.GetFiles(rootPath, pattern, SearchOption.AllDirectories);

            foreach (string matchedFile in matchedFiles)
            {
                if (ShouldSkipFile(matchedFile, excludedDirectoryNames))
                {
                    continue;
                }

                FileIndexEntry entry = CreateFileIndexEntry(rootPath, matchedFile);
                entries.Add(entry);
            }
        }

        return entries;
    }

    private static bool ShouldSkipFile(string filePath, string[] excludedDirectoryNames)
    {
        foreach (string excludedDirectoryName in excludedDirectoryNames)
        {
            string excludedPart = Path.DirectorySeparatorChar + excludedDirectoryName + Path.DirectorySeparatorChar;

            if (filePath.Contains(excludedPart, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private static FileIndexEntry CreateFileIndexEntry(string rootPath, string filePath)
    {
        FileInfo fileInfo = new FileInfo(filePath);

        string relativePath = Path.GetRelativePath(rootPath, filePath);
        string extension = fileInfo.Extension.ToLowerInvariant();

        try
        {
            string[] lines = File.ReadAllLines(filePath);

            int lineCount = lines.Length;
            int nonEmptyLineCount = CountNonEmptyLines(lines);
            int functionLikeCount = CountFunctionLikeItems(extension, lines);

            FileIndexEntry entry = new FileIndexEntry()
            {
                FullPath = filePath,
                RelativePath = relativePath,
                Extension = extension,
                SizeBytes = fileInfo.Length,
                LineCount = lineCount,
                NonEmptyLineCount = nonEmptyLineCount,
                FunctionLikeCount = functionLikeCount,
                ErrorMessage = null
            };
            return entry;
        }
        catch (Exception exception)
        {
            FileIndexEntry entry = new FileIndexEntry()
            {
                FullPath = filePath,
                RelativePath = relativePath,
                Extension = extension,
                SizeBytes = fileInfo.Exists ? fileInfo.Length : 0,
                LineCount = 0,
                NonEmptyLineCount = 0,
                FunctionLikeCount = 0,
                ErrorMessage = exception.Message
            };
            return entry;
        }
    }

    private static int CountNonEmptyLines(string[] lines)
    {
        int count = 0;

        foreach (string line in lines)
        {
            if (!string.IsNullOrWhiteSpace(line))
            {
                count++;
            }
        }

        return count;
    }

    private static int CountFunctionLikeItems(string extension, string[] lines)
    {
        int count = 0;

        foreach (string line in lines)
        {
            string trimmedLine = line.Trim();

            if (string.IsNullOrWhiteSpace(trimmedLine))
            {
                continue;
            }

            if (trimmedLine.StartsWith("//"))
            {
                continue;
            }

            if (extension == ".prg")
            {
                if (IsHarbourFunctionLikeLine(trimmedLine))
                {
                    count++;
                }
            }

            if (extension == ".cs")
            {
                if (IsCSharpTypeDeclarationLine(trimmedLine))
                {
                    count++;
                }
            }
        }

        return count;
    }

    private static bool IsHarbourFunctionLikeLine(string line)
    {
        return line.StartsWith("function ", StringComparison.OrdinalIgnoreCase) ||
               line.StartsWith("procedure ", StringComparison.OrdinalIgnoreCase) ||
               line.StartsWith("method ", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsCSharpTypeDeclarationLine(string line)
    {
        return line.StartsWith("public class ") ||
               line.StartsWith("internal class ") ||
               line.StartsWith("private class ") ||
               line.StartsWith("protected class ") ||
               line.StartsWith("class ") ||
               line.StartsWith("public record ") ||
               line.StartsWith("internal record ") ||
               line.StartsWith("private record ") ||
               line.StartsWith("record ") ||
               line.StartsWith("public interface ") ||
               line.StartsWith("internal interface ") ||
               line.StartsWith("interface ");
    }
}