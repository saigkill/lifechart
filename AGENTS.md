# AGENTS.md — LifeChart

Cross-platform mood tracker (.NET MAUI, Android + Windows). Clean Architecture + DDD-Lite.

Behavioral guidelines to reduce common LLM coding mistakes. Merge with project-specific instructions as needed.

**Tradeoff:** These guidelines bias toward caution over speed. For trivial tasks, use judgment.

## 1. Think Before Coding

**Don't assume. Don't hide confusion. Surface tradeoffs.**

Before implementing:
- State your assumptions explicitly. If uncertain, ask.
- If multiple interpretations exist, present them - don't pick silently.
- If a simpler approach exists, say so. Push back when warranted.
- If something is unclear, stop. Name what's confusing. Ask.

## 2. Simplicity First

**Minimum code that solves the problem. Nothing speculative.**

- No features beyond what was asked.
- No abstractions for single-use code.
- No "flexibility" or "configurability" that wasn't requested.
- No error handling for impossible scenarios.
- If you write 200 lines and it could be 50, rewrite it.

Ask yourself: "Would a senior engineer say this is overcomplicated?" If yes, simplify.

## 3. Surgical Changes

**Touch only what you must. Clean up only your own mess.**

When editing existing code:
- Don't "improve" adjacent code, comments, or formatting.
- Don't refactor things that aren't broken.
- Match existing style, even if you'd do it differently.
- If you notice unrelated dead code, mention it - don't delete it.

When your changes create orphans:
- Remove imports/variables/functions that YOUR changes made unused.
- Don't remove pre-existing dead code unless asked.

The test: Every changed line should trace directly to the user's request.

## 4. Goal-Driven Execution

**Define success criteria. Loop until verified.**

Transform tasks into verifiable goals:
- "Add validation" → "Write tests for invalid inputs, then make them pass"
- "Fix the bug" → "Write a test that reproduces it, then make it pass"
- "Refactor X" → "Ensure tests pass before and after"

For multi-step tasks, state a brief plan:
```
1. [Step] → verify: [check]
2. [Step] → verify: [check]
3. [Step] → verify: [check]
```

Strong success criteria let you loop independently. Weak criteria ("make it work") require constant clarification.

---

## 5. Solution layout

```
src/LifeChart.slnx                  ← use this, not .sln
src/LifeChart/                      ← MAUI UI, ViewModels, Pages (entry point: MauiProgram.cs)
src/LifeChart.Domain/               ← Entities, Value Objects, Repository interfaces
src/LifeChart.Application/          ← UseCases, DTOs, Interfaces, Localization
src/LifeChart.Infrastructure/       ← EF Core, Repositories, Backup, PDF
src/LifeChart.Linux/                ← EXPERIMENTAL GTK4 host — not in CI, not production-ready
src/LifeChart.Tests/                ← Unit tests (net10.0, no MAUI workload needed)
```

## 6. Commands

```bash
# Tests only — no MAUI workload required
dotnet restore src/LifeChart.Tests/LifeChart.Tests.csproj
dotnet test src/LifeChart.Tests/LifeChart.Tests.csproj --no-restore

# Full solution build — requires MAUI workload first
dotnet workload install maui-android   # or maui-windows
dotnet build src/LifeChart.slnx

# Android publish (Play Store flavor)
dotnet publish src/LifeChart/LifeChart.csproj -f net10.0-android -c Release /p:DefineConstants="GOOGLE_SERVICES"

# Android publish (F-Droid flavor — no Google Drive)
dotnet publish src/LifeChart/LifeChart.csproj -f net10.0-android -c Release /p:DefineConstants="FDROID"

# Windows publish
dotnet publish src/LifeChart/LifeChart.csproj -f net10.0-windows10.0.19041.0 -c Release
```

## 7. EF Core / Migrations

- DbContext: `LifeChartDbContext` in `LifeChart.Infrastructure.Persistence`
- SQLite at runtime: `{LocalApplicationData}/LifeChart/lifechart.db`
- Migrations: `src/LifeChart.Infrastructure/Migrations/`
- Migrations run automatically at startup via `db.Database.Migrate()` in `MauiProgram.cs` — no manual step needed on device
- Design-time factory uses `lifechart_designtime.db`; add migrations from `src/`:
  ```bash
  dotnet ef migrations add <Name> --project LifeChart.Infrastructure --startup-project LifeChart.Infrastructure
  ```
- Entity configurations in `LifeChart.Infrastructure/Persistence/Configurations/` (one file per entity, loaded via `ApplyConfigurationsFromAssembly`)

## 8. DI conventions

- `Application` layer: register services in `LifeChart.Application/DependencyInjection.cs` → `AddApplication()`
- `Infrastructure` layer: register in `LifeChart.Infrastructure/DependencyInjection.cs` → `AddInfrastructure()`
- Platform-specific services (e.g. `IAlarmService`) registered in `MauiProgram.cs` under `#if ANDROID` guards
- All ViewModels and Pages are **Transient**

## 9. Android build flavors

| `DefineConstants` | Effect |
|---|---|
| `GOOGLE_SERVICES` | Enables `GoogleDriveBackupProvider` as `IBackupProvider` |
| `FDROID` | Google Drive excluded at compile time |

The `GOOGLE_SERVICES`/`FDROID` split is wired in `Infrastructure/DependencyInjection.cs`. CI signing requires secrets `ANDROID_KEYSTORE`, `KEY_ALIAS`, `KEY_PASSWORD`.

## 10. Domain rules to preserve

- Value objects `MoodScore` and `FunctionalityScore`: valid range −5 to +5; `IsCritical` = true at −4 or lower
- `DailyEntry.IsCritical` requires **both** `Mood.IsCritical && Functionality.IsCritical` — single-axis crisis is not flagged
- `SleepHours` is a value object (not a primitive), enforces its own range in the constructor

## 11. Testing

- Test framework: xUnit 2.9.3 + FluentAssertions + **NSubstitute** (not Moq)
- `EF InMemory` package present but tests use NSubstitute-mocked repositories — do not add InMemory DB tests without discussion
- No integration tests, no UI tests, no shared fixtures — each class is standalone
- `[Theory]` + `[InlineData]` preferred for boundary/range cases

## 12. Localization

- UI strings go through `LifeChart.Application.Localization.L` (static class)
- Culture set at startup from saved `AppLanguage` setting via `L.SetCulture(...)`

## 13. App startup flow

1. `MauiProgram.CreateMauiApp()` — registers DI, runs `db.Database.Migrate()`, sets culture
2. `App.xaml.cs` — routes to `OnboardingPage` (first run), `LockPage` (biometrics enabled), or `AppShell`

## 14. CI

- PRs to `main`/`develop`: runs tests only
- `v*` tags: tests → android (Play Store + F-Droid, ubuntu) + windows (MSIX, windows-latest) in parallel
- SDK: `dotnet-version: '10.0.x'` (not pinned; no `global.json`)
- No `Directory.Build.props` or `Directory.Packages.props` — each project pins its own package versions
