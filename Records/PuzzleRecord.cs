using System.Text.Json.Serialization;

namespace auth11.Records;

public record DailyPuzzle(
    [property:JsonPropertyName("puzzle")] Puzzle Puzzle
);

public record Puzzle(
    [property:JsonPropertyName("solution")]List<string> Solution,
    [property:JsonPropertyName("fen")]string Fen
);