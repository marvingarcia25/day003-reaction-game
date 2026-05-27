using ReactionGame.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddSingleton<LeaderboardStore>();

var app = builder.Build();
app.UseStaticFiles();
app.MapRazorPages();

app.MapGet("/api/leaderboard", (LeaderboardStore store) =>
    Results.Ok(store.GetTop10().Select(e => new
    {
        name = e.Name,
        averageMs = e.AverageMs,
        rounds = e.Rounds,
        playedAt = e.PlayedAt
    })));

app.MapPost("/api/leaderboard", (LeaderboardRequest req, LeaderboardStore store) =>
{
    try
    {
        var entry = store.Add(req.Name ?? string.Empty, req.AverageMs, req.Rounds);
        return Results.Created("/api/leaderboard", new
        {
            name = entry.Name,
            averageMs = entry.AverageMs,
            rounds = entry.Rounds,
            playedAt = entry.PlayedAt
        });
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.Run();

record LeaderboardRequest(string? Name, double AverageMs, int Rounds);
public partial class Program { }
