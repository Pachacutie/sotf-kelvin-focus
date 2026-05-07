# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/), and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- Initial project scaffold via `dotnet new sotfmod` (RedLoader.Templates 1.2.7).
- Workshop convention files: README, CHANGELOG, LICENSE, .gitignore.
- `DISCOVERY.md` — UnityExplorer workflow doc; Sections 1–7 walked through against a live save on 2026-05-07 and Confirmed Findings populated with architecture notes.
- `Patches/KelvinSkipLooseLogPatch.cs` — Harmony prefix on `Sons.Ai.Vail.Thought.CanRun` returning `false` for the loose-log pickup Thought (matched via `_stimuliTargetId == "LogPickup"`). Stops Kelvin from detouring to ground logs when commanded `Get → Logs → Fill Holder`; he proceeds directly to chop a tree instead. Cross-task verified clean for `Get Sticks` and `Get Rocks`.

### Changed

- `SOTF_KELVIN_FOCUS.csproj`: added `Sons.Ai.Vail.dll` reference (required by patch).
- `SOTF_KELVIN_FOCUS.cs`: enabled `HarmonyPatchAll = true` so RedLoader auto-applies patches in this assembly.

### Notes

- v0 patch target validated live via UnityExplorer manual mute of `Thought._mute` on the loose-log Thought instance. Patched-DLL build/verification still pending.
- Game version tested: SOTF on Unity 2022.2.16f1 (specific SOTF build version not pinned in `manifest.json`).
