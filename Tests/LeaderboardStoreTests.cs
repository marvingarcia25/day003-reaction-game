using ReactionGame.Services;
using Xunit;

namespace Tests;

public class LeaderboardStoreTests
{
    private static LeaderboardStore NewStore() => new();

    [Fact]
    public void Add_ValidEntry_AppearsInTop10()
    {
        var store = NewStore();
        store.Add("ACE", 150.0, 5);
        var top10 = store.GetTop10();
        Assert.Single(top10);
        Assert.Equal("ACE", top10[0].Name);
    }

    [Fact]
    public void Add_EmptyName_ThrowsArgumentException()
    {
        var store = NewStore();
        Assert.Throws<ArgumentException>(() => store.Add("", 150.0, 5));
    }

    [Fact]
    public void Add_WhitespaceName_ThrowsArgumentException()
    {
        var store = NewStore();
        Assert.Throws<ArgumentException>(() => store.Add("   ", 150.0, 5));
    }

    [Fact]
    public void Add_NameTooLong_ThrowsArgumentException()
    {
        var store = NewStore();
        Assert.Throws<ArgumentException>(() => store.Add(new string('A', 31), 150.0, 5));
    }

    [Fact]
    public void Add_NameExactly30Chars_Succeeds()
    {
        var store = NewStore();
        var entry = store.Add(new string('A', 30), 150.0, 5);
        Assert.Equal(30, entry.Name.Length);
    }

    [Fact]
    public void Add_NegativeAverageMs_ThrowsArgumentException()
    {
        var store = NewStore();
        Assert.Throws<ArgumentException>(() => store.Add("ACE", -1.0, 5));
    }

    [Fact]
    public void Add_ZeroAverageMs_ThrowsArgumentException()
    {
        var store = NewStore();
        Assert.Throws<ArgumentException>(() => store.Add("ACE", 0.0, 5));
    }

    [Fact]
    public void Add_InvalidRounds_ThrowsArgumentException()
    {
        var store = NewStore();
        Assert.Throws<ArgumentException>(() => store.Add("ACE", 150.0, 7));
    }

    [Fact]
    public void GetTop10_SortedByAverageMsAscending()
    {
        var store = NewStore();
        store.Add("SLOW", 300.0, 5);
        store.Add("FAST", 100.0, 5);
        store.Add("MID", 200.0, 5);
        var top10 = store.GetTop10();
        Assert.Equal(new[] { 100.0, 200.0, 300.0 }, top10.Select(e => e.AverageMs));
    }

    [Fact]
    public void GetTop10_ReturnsAtMostTen()
    {
        var store = NewStore();
        for (int i = 0; i < 15; i++)
            store.Add($"P{i:D2}", 100.0 + i, 5);
        var top10 = store.GetTop10();
        Assert.Equal(10, top10.Count);
        Assert.Equal(100.0, top10[0].AverageMs);   // fastest included
        Assert.Equal(109.0, top10[9].AverageMs);   // 10th fastest included
    }

    [Fact]
    public void Add_Over1000Entries_DropsHighestAverageMs()
    {
        var store = NewStore();
        for (int i = 0; i < 1000; i++)
            store.Add($"P{i:D4}", 100.0 + i, 5);
        // Store is full at 1000. Slowest is P0999 at 1099ms.
        store.Add("FAST", 50.0, 5);
        var top10 = store.GetTop10();
        Assert.Equal(50.0, top10[0].AverageMs);        // FAST is fastest
        Assert.Equal(1000, store.Count);               // cap enforced — store did not grow to 1001
    }

    [Fact]
    public void TwoEntries_IndependentSortOrder()
    {
        var store = NewStore();
        store.Add("B", 200.0, 3);
        store.Add("A", 150.0, 10);
        var top10 = store.GetTop10();
        Assert.Equal("A", top10[0].Name);
        Assert.Equal("B", top10[1].Name);
    }
}
