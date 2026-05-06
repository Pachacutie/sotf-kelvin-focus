# DISCOVERY.md — Identifying Kelvin's Loose-Log Scan Logic

This is the playbook for the discovery phase. Execute Sections 1–7 in order, then fill in the **Confirmed Findings** template at the bottom. Once findings are in, write the actual Harmony patch in `Patches/`.

**Goal:** find the class/method/field that controls Kelvin's loose-log detection during a `Get > Logs > Fill Holder` task, so we can short-circuit it.

**Two strategies, evaluated in order:**

- **Strategy B (radius):** find a float field controlling scan range; collapse it to ~0 via Harmony postfix.
- **Strategy A (method patch):** find the method returning the loose-log target; Harmony-prefix to return null.

Strategy B is simpler and lower-risk. Try it first.

## Pre-Flight

Before opening the game:

- [ ] RedLoader 0.8.6 installed at `D:\Games\SteamLibrary\steamapps\common\Sons Of The Forest\_RedLoader\`
- [ ] UnityExplorer 4.9.4 installed at `<gameDir>\Mods\UnityExplorer\`
- [ ] Steam set to "Only update when I launch" for SOTF
- [ ] **Test save** loaded into a backup slot — never use the main save for discovery work
- [ ] Other mods that touch Kelvin (none confirmed-conflicting, but if behavior gets weird, disable `ImmortalCompanions` first)

## Section 1 — Reproduction Setup

Reliably trigger the loose-log detour. Without a reliable repro, hooks can't tell which methods fire on the bad path.

1. Load test save where Kelvin is alive and trust is high enough for him to obey commands.
2. Stand at base near a log holder and a stand of choppable trees (within ~50m).
3. Drop **4–6 loose logs** on the ground near the log holder. (Pull from inventory, drop, do NOT store in the holder. The looser/messier they are, the more reliable the trigger.)
4. Verify Kelvin's task queue is empty (no orange chevron above his head). Cancel any active task with the notepad if needed.
5. Issue command: notepad → Kelvin → **Get → Logs → Fill Holder**.
6. **Observe:** Kelvin walks to the loose logs first, picks them up, deposits in the holder, _then_ walks to a tree to chop. This is the behavior we want to eliminate.

If Kelvin doesn't reproduce the detour reliably, drop more logs / move them closer / try a different approach angle. Repro stability matters more than speed.

## Section 2 — UnityExplorer Mouse-Inspect Drilldown

1. Press **F7** (default UE hotkey) to open UnityExplorer overlay.
2. Click the **Inspector** tab → **Mouse Inspect** dropdown → select **World** mode.
3. Click on Kelvin's body model in the world. UE opens a GameObject Inspector for him.
4. In the GameObject Inspector, scroll the **Components** list. Catalog every component name. Particular interest:
   - Any component with `Ai`, `Task`, `Behavior`, `Robby`, `Vail`, `Pickup`, `Carry`, `Decision`, `Goal`, `Need` in the name.
   - The `Robby` component (Kelvin's TypeId is `VailActorTypeId.Robby` per existing modding research).
5. For each candidate component, click it to expand fields/properties. Note:
   - **Float fields** with names suggesting distance/radius: write down name + current value.
   - **List/array fields** holding GameObjects or generic targets: especially anything looking like "currentTarget", "pickupTarget", "scanResults".
   - **Method names** visible in the inspector (UE shows them under Members) that take or return types like `LogPickup`, `VailLooseItem`, `Pickup`, `Carryable`.

Write findings into the Findings template (bottom of this doc). Don't filter aggressively yet — over-collect, prune later.

## Section 3 — Hook-Panel Method Tracing

UnityExplorer's **Hooks** panel auto-generates Harmony stubs at the click of a button. Use it to discover which methods actually fire during the detour.

1. Open the **Hooks** tab.
2. Enter a candidate class name from Section 2 (e.g., the AI/task component you flagged).
3. UE lists its methods. Hook these patterns one at a time:
   - `Find...`, `FindNearby...`, `FindClosest...`
   - `Get...Target`, `GetClosest...`, `GetCurrent...`
   - `Scan...`, `Search...`, `Look...`, `Detect...`
   - `Update...Target`, `Update...State`, `Tick`
   - `Pickup...`, `Carry...`
4. For each hook UE generates, edit the source and add at the top of the Postfix:
   ```csharp
   RedLoader.RLog.Msg($"[KFOCUS] {methodName} fired -> __result = {__result}");
   ```
   (Replace `methodName` with the literal name; `__result` only works on Postfix and only if the method has a return value.)
5. With hooks active, **reproduce Section 1**.
6. Tail the RedLoader console (visible in-game, or check `<gameDir>\_RedLoader\Latest.log`).
7. Note the sequence of `[KFOCUS]` log lines fired during the loose-log walk. The method that consistently fires _while Kelvin is heading toward a loose log_ and returns a `LogPickup`-ish object is the patch target.

Tip: if too many methods fire to make sense of the log, narrow to one component at a time and reproduce again.

## Section 4 — Strategy B Target (Radius)

If Section 3 found a firing method on a component that has a float radius field nearby:

1. In UnityExplorer's GameObject Inspector for Kelvin → expand the firing component.
2. Find a float field that looks like a distance (`*Radius`, `*Range`, `*Distance`, `*Reach`).
3. Note its current value, then **edit it live** — set to `0.0` or `0.5`.
4. Reproduce Section 1.
5. **If Kelvin no longer detours → confirmed.** Record the field's full path (`Namespace.Class.fieldName`) and current value in Findings. Strategy B is viable. Skip Section 5.
6. **If Kelvin still detours →** the radius isn't the gate (might be binary "is anything pickable in scene" not radial). Fall through to Section 5.

## Section 5 — Strategy A Target (Method Patch)

If Strategy B failed:

1. Identify the firing method from Section 3 that returns the loose-log target.
2. In UE's hook editor for that method, replace the body with a Prefix that nulls the result:
   ```csharp
   // method signature filled in from Section 3
   static bool Prefix(ref SomeReturnType __result) {
       __result = null;     // or default(SomeReturnType), or empty list
       return false;        // skip original method body
   }
   ```
3. Reproduce Section 1. Verify Kelvin walks past the loose logs to a tree.
4. Record the full method signature (`Namespace.Class.MethodName(params...)`) in Findings.

## Section 6 — Cross-Task Side-Effect Check

The scan logic might be **generic** — same method used for sticks, rocks, berries. Test that the patch only affects logs:

- [ ] Issue Kelvin: `Get > Sticks > Fill Holder` near loose sticks. Sticks should still be picked up.
- [ ] Issue Kelvin: `Get > Rocks > Drop Here` near loose rocks. Rocks should still be picked up.
- [ ] Issue Kelvin: `Get > Berries > Give to Me` near a berry bush. Berries should still be eaten/picked.
- [ ] Spawn or wait for an enemy near Kelvin. Combat detection should still work.

If the patch leaks to other tasks, the scanner is generic. Two fixes:

- **Filter by item type:** in your Prefix, check the active task's target item; only short-circuit when the type is `Log`/`LogPickup`. Note the active-task field/enum name in Findings.
- **Filter by active task type:** check Kelvin's current task object; only short-circuit when task is `GetLogsFillHolder` (or whatever the enum/class is called). Also note this in Findings.

## Section 7 — Documentation

Once you have a working live patch via UE:

1. Fill in the Findings template below.
2. Save UE's edited hook source somewhere (copy/paste into a scratch file in `_research/` — the folder is gitignored).
3. Note the game version from `Steam → SOTF → Properties → General` (or `<gameDir>\info.json`).
4. Move to the implementation phase: branch `feat/v0-radius-patch` (or `feat/v0-method-patch`), drop a Harmony class into `Patches/`, register via `HarmonyPatchAll = true` in `SOTF_KELVIN_FOCUS.cs`, build, redeploy, retest from scratch (no UE patches active to avoid double-patching).

## Confirmed Findings

_(Fill these in during/after live discovery. Until populated, no patch is written in source.)_

- **Game version tested:** `__________`
- **Kelvin AI component class (full namespace):** `__________`
- **Loose-log scan method signature:** `__________::__________(...)`
- **Radius field (Strategy B viable?):** `__________` (current value: `____`)
- **Active-task enum/property:** `__________`
- **Active-task value when "Get Logs Fill Holder" is running:** `__________`
- **Patch strategy chosen:** [ ] B (radius)  [ ] A (method prefix)
- **Cross-task side-effects observed:** `__________`
- **Required filter (if scanner is generic):** `__________`

## References

- [RedLoader README](https://github.com/ToniMacaroni/RedLoader) — loader install/usage
- [UnityExplorer_Sons](https://github.com/ToniMacaroni/UnityExplorer_Sons) — in-game inspector
- [SonsSdk source tree](https://github.com/ToniMacaroni/RedLoader/tree/master/SonsSdk) — confirmed helper APIs (`ActorTools`, `GameCommands`, etc.)
- [LITF - Improved Kelvin source](https://github.com/caiomadeira/LifeInTheForest) — confirmed game classes used: `VailWorldSimulation`, `VailActorTypeId.Robby`, `ActorTools`
- [KelvinCloner source](https://github.com/jakzo/EndnightMods/tree/main/projects/Sons/KelvinCloner) — confirmed game class `Sons.Characters.CharacterManager`
- [SOTF AI System wiki](https://sonsoftheforest.fandom.com/wiki/Ai_System) — high-level "thoughts" decision system overview
