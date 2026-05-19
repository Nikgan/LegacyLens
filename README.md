# LegacyLens

LegacyLens is a small .NET console tool for indexing source code directories.

The project scans a directory, collects basic information about source files, builds a summary, and saves the result to a JSON file.

## Current features

- Scan source files by extension
- Recursive or top-level-only directory scan
- Exclude technical directories like `bin`, `obj`, `.git`, `.vs`
- Count total lines and non-empty lines
- Count basic code items such as classes, records, interfaces, Harbour functions/procedures/methods
- Save index result to JSON
- Command-line options
- Unit tests

## Supported file patterns

Currently indexed patterns:

- `*.cs`
- `*.prg`
- `*.ch`
- `*.log`
- `*.txt`
- `*.md`

## Usage

```bash
LegacyLens <rootPath> [--top|--recursive] [--output <filePath>]