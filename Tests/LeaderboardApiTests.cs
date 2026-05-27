using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Tests;

public class LeaderboardApiTests
{
    [Fact]
    public async Task GetLeaderboard_EmptyStore_Returns200EmptyArray()
    {
        using var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();
        var response = await client.GetAsync("/api/leaderboard");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<List<object>>();
        Assert.NotNull(body);
        Assert.Empty(body);
    }

    [Fact]
    public async Task PostLeaderboard_ValidPayload_Returns201()
    {
        using var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/leaderboard",
            new { name = "ACE", averageMs = 150.0, rounds = 5 });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task PostLeaderboard_EmptyName_Returns400()
    {
        using var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/leaderboard",
            new { name = "", averageMs = 150.0, rounds = 5 });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostLeaderboard_NullName_Returns400()
    {
        using var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/leaderboard",
            new { name = (string?)null, averageMs = 150.0, rounds = 5 });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostLeaderboard_NegativeAverageMs_Returns400()
    {
        using var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/leaderboard",
            new { name = "ACE", averageMs = -1.0, rounds = 5 });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostLeaderboard_InvalidRounds_Returns400()
    {
        using var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/leaderboard",
            new { name = "ACE", averageMs = 150.0, rounds = 7 }); // valid: 3, 5, or 10
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostLeaderboard_NameTooLong_Returns400()
    {
        using var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/leaderboard",
            new { name = new string('A', 31), averageMs = 150.0, rounds = 5 });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostLeaderboard_ValidPayload_AppearsInGet()
    {
        using var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();
        var postResponse = await client.PostAsJsonAsync("/api/leaderboard",
            new { name = "UNIQUEPLAYER", averageMs = 123.4, rounds = 3 });
        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);
        var getResponse = await client.GetAsync("/api/leaderboard");
        var body = await getResponse.Content.ReadAsStringAsync();
        Assert.Contains("UNIQUEPLAYER", body);
    }
}
