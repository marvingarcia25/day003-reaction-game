namespace ReactionGame.Models;

public record LeaderboardEntry(
    string Name,
    double AverageMs,
    int Rounds,
    DateTime PlayedAt
);
