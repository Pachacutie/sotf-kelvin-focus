# SOTF_KELVIN_FOCUS

Stops Kelvin from detouring to loose logs when commanded to fill a log holder, so he goes straight to chopping a tree.

## Features

- Cuts the loose-log scan from the `Get > Logs > Fill Holder` task path.
- Falls through to Kelvin's normal tree-chopping behavior.
- No effect on other Get-tasks (sticks, rocks, berries) or combat detection.
- Single-player and self-hosted multiplayer (peers must run the same mod).

## How It Works

Kelvin's AI is a stimulus-driven scoring competition under `Sons.Ai.Vail`. When given `Get > Logs > Fill Holder`, the "RobbyGetLog" task evaluates three Thoughts each tick: pickup-loose-log, drop-log-at-holder, and follow-player-with-log. The pickup Thought uses a global stimulus query (not a distance-bounded one), so it scores high whenever any loose log exists in the scene — that's the detour.

This mod Harmony-prefixes `Sons.Ai.Vail.Thought.CanRun` and forces it to return `false` whenever the Thought's `_stimuliTargetId == "LogPickup"`. With that Thought removed from selection, Kelvin's state machine falls through to the sibling `RobbyClearTree` Group and chops a tree; the resulting log is then deposited by the drop-log Thought normally.

Cross-task isolation is architectural: each Get-task is its own `Sons.Ai.Vail.Group` with its own Thoughts list, so the patch cannot affect sticks, rocks, berries, or combat. See `DISCOVERY.md` for the full inspection workflow.

## Installation

1. Install [RedLoader](https://github.com/ToniMacaroni/RedLoader) into your Sons of the Forest install (drop the contents of `Redloader.zip` into the game directory).
2. Place `SOTF_KELVIN_FOCUS.dll` and the `SOTF_KELVIN_FOCUS/` folder (containing `manifest.json`) into `<gameDir>\Mods\`.
3. Pin Steam updates: SOTF → Properties → Updates → "Only update when I launch." (Steam auto-updates routinely break IL2CPP mods.)

## Quick Start

1. Launch SOTF via the RedLoader launcher.
2. Establish a base with a log holder.
3. Drop several loose logs near the holder (this is the trigger for the original buggy behavior).
4. Issue Kelvin: notepad → Get → Logs → Fill Holder.
5. With the mod installed, Kelvin should ignore the loose logs and walk to a tree.

## Building from Source

Requires .NET 8 SDK (builds the project's `net6` target via the .NET 8 toolchain) and RedLoader installed in the game directory referenced by `SOTF_KELVIN_FOCUS.csproj.user`'s `<GameDir>` property.

```powershell
dotnet build
```

On a successful build, the DLL and `manifest.json` are auto-copied to `<gameDir>\Mods\` by `Directory.Build.targets`.

For a redistributable zip (drop-in install for other users):

```powershell
dotnet build -c Release
```

The zip is created at `ReleaseBuild\SOTF_KELVIN_FOCUS.zip`.

## Compatibility

- **Game version tested:** SOTF Steam buildid `20228174` (released 2025-10-10). Should work on any build where `Sons.Ai.Vail.Thought.CanRun` exists and the loose-log Thought's `_stimuliTargetId` is still `"LogPickup"`.
- **RedLoader version tested:** 0.8.6
- **Known mod conflicts:** None confirmed. ImmortalCompanions, Restless Kelvin, and LITF - Improved Kelvin all touch Kelvin's components but at different layers; conflicts unlikely but worth verifying.

## Contributing

Pull requests welcome. Branches use `feat/`, `fix/`, `docs/`, `chore/` prefixes; commits follow [Conventional Commits](https://www.conventionalcommits.org/). PRs squash-merge to main.
