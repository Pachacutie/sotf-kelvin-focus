# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/), and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Changed

- `manifest.json`: pinned `gameVersion` to Steam buildid `20228174` (the build SOTF was on as of last update 2025-10-10). Resolves the open carry-forward from 0.1.0; was the template default `1.0.0`.
- `README.md`: refreshed "How It Works" to describe the shipped Strategy A patch (Sons.Ai.Vail stimulus-driven scoring + Thought.CanRun prefix on `_stimuliTargetId == "LogPickup"`); the prior text described both strategies as to-evaluate. Pinned game version tested to the buildid. Generalized the Contributing section for public visibility (removed internal-only links and approval references).

## [0.1.0] - 2026-05-07

### Added

- Initial project scaffold via `dotnet new sotfmod` (RedLoader.Templates 1.2.7).
- Workshop convention files: README, CHANGELOG, LICENSE, .gitignore.
- `DISCOVERY.md` — UnityExplorer workflow doc; Sections 1–7 walked through against a live save on 2026-05-07 and Confirmed Findings populated with architecture notes.
- `Patches/KelvinSkipLooseLogPatch.cs` — Harmony prefix on `Sons.Ai.Vail.Thought.CanRun` returning `false` for the loose-log pickup Thought (matched via `_stimuliTargetId == "LogPickup"`). Stops Kelvin from detouring to ground logs when commanded `Get → Logs → Fill Holder`; he proceeds directly to chop a tree instead. Cross-task verified clean for `Get Sticks` and `Get Rocks`.

### Changed

- `SOTF_KELVIN_FOCUS.csproj`: added `Sons.Ai.Vail.dll` reference (required by patch).
- `SOTF_KELVIN_FOCUS.cs`: enabled `HarmonyPatchAll = true` so RedLoader auto-applies patches in this assembly.
- `manifest.json`: `version` set to `0.1.0` to align with this release (was `1.0.0` template default).

### Notes

- v0 patch verified live in-game with patched DLL on 2026-05-07 (test save `6154988860`, 4 loose logs in front of Kelvin → he walked straight to a tree).
- Game version tested: SOTF on Unity 2022.2.16f1 (specific SOTF build version not pinned in `manifest.json` — open carry-forward).
