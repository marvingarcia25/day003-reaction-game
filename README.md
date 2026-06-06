# day3_ReactionGame — Reaction Speed Game

Wait for the signal, click as fast as you can, see how you rank.

An ASP.NET Razor Pages reflex game. Play 3, 5, or 10 rounds; it measures your average reaction time in milliseconds and drops you onto a top-10 leaderboard (fastest average wins).

## What it does

- Reaction rounds: wait for the go signal, click, measure the gap in ms
- Average over 3 / 5 / 10 rounds
- In-memory top-10 leaderboard, validated server-side (name, round count, positive times)

## Stack

- ASP.NET Core (Razor Pages, .NET 8)
- In-memory `LeaderboardStore`
- xUnit tests + GitHub Actions deploy workflow

## Running it

```
dotnet run
```

## Tests

```
dotnet test
```

---

Day 3 of building a small thing every day.
