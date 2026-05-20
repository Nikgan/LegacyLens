namespace LegacyLens.Models;

public record CodeChunk
{
	public required string Id { get; init; }
	public required CodeItemKind Kind { get; init; }
	public required string Name { get; init; }
	public required int StartLineNumber { get; init; }
	public required int EndLineNumber { get; init; }
	public required string Text { get; init; }
	public required string ContentHash { get; init; }

	public int LineCount => EndLineNumber - StartLineNumber + 1;
}