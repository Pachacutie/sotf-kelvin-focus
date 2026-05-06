# SOTF_KELVIN_FOCUS

Stops Kelvin from detouring to loose logs when commanded to fill a log holder, so he goes straight to chopping a tree.

## Features

- Cuts the loose-log scan from the `Get > Logs > Fill Holder` task path.
- Falls through to Kelvin's normal tree-chopping behavior.
- No effect on other Get-tasks (sticks, rocks, berries) or combat detection.
- Single-player and self-hosted multiplayer (peers must run the same mod).

## How It Works

When given `Get > Logs > Fill Holder`, Kelvin's AI runs a loose-item scan before deciding where to go. If any loose log is detected within his scan radius, he detours to pick it up first. This mod patches that scan path so the loose-log search is skipped or its radius is collapsed, depending on what the live game exposes (see `DISCOVERY.md` for the discovery workflow).

Two implementation strategies are evaluated in order:

- **Strategy B (preferred):** Reduce the loose-log scan radius to ~0 via a Harmony postfix on the relevant getter, leaving the rest of Kelvin's task path untouched.
- **Strategy A (fallback):** Harmony-prefix the loose-log scan method to return null/empty, forcing Kelvin to fall through to tree-chopping.

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

- **Game version tested:** _(populated when v0 ships)_
- **RedLoader version tested:** 0.8.6
- **Known mod conflicts:** None confirmed. ImmortalCompanions, Restless Kelvin, and LITF - Improved Kelvin all touch Kelvin's components but at different layers; conflicts unlikely but worth verifying.

## Contributing

This project is part of [the Workshop](../../README.md). Branches use `feat/`, `fix/`, `docs/`, `chore/` prefixes; commits follow Conventional Commits; merges to main require ARCHITECT approval (squash merge).
