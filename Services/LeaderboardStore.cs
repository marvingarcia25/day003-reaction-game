using ReactionGame.Models;

namespace ReactionGame.Services;

public class LeaderboardStore
{
    private readonly List<LeaderboardEntry> _entries = new();
    private readonly object _lock = new();

    public int Count
    {
        get { lock (_lock) { return _entries.Count; } }
    }

    public IReadOnlyList<LeaderboardEntry> GetTop10()
    {
        lock (_lock)
        {
            return _entries.OrderBy(e => e.AverageMs).Take(10).ToList();
        }
    }

    public LeaderboardEntry Add(string name, double averageMs, int rounds)
    {
        name = name?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(name))
            throw new ArgumentException("Name is required.", nameof(name));
        if (name.Length > 30)
            throw new ArgumentException("Name must be 30 characters or fewer.", nameof(name));
        if (averageMs <= 0)
            throw new ArgumentException("AverageMs must be positive.", nameof(averageMs));
        if (rounds != 3 && rounds != 5 && rounds != 10)
            throw new ArgumentException("Rounds must be 3, 5, or 10.", nameof(rounds));

        var entry = new LeaderboardEntry(name, averageMs, rounds, DateTime.UtcNow);
        lock (_lock)
        {
            if (_entries.Count >= 1000)
            {
                var slowest = _entries.MaxBy(e => e.AverageMs)!;
                _entries.Remove(slowest);
            }
            _entries.Add(entry);
        }
        return entry;
    }
}
