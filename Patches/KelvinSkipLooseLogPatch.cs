using HarmonyLib;
using Sons.Ai.Vail;

namespace SOTF_KELVIN_FOCUS.Patches;

// Stops Kelvin from detouring to loose ground logs when commanded to fill a log holder.
//
// Kelvin's "Get Logs" task is a Sons.Ai.Vail.Group containing 3 Thoughts:
//   [0] "pickup log" — scans for LogPickup stimuli (loose logs on the ground) ← the detour
//   [1] "drop log"   — deposits a carried log at a Robby Drop point (the holder)
//   [2] "follow player with log" — idle/transit while carrying
//
// Tree-chopping lives in a separate Group (RobbyClearTree) and is untouched. Suppressing
// Thought [0] forces Kelvin's StateSet to fall through to the chop-tree path, which still
// produces logs that Thought [1] then deposits — same outcome, no detour.
//
// We identify Thought [0] by its _stimuliTargetId == "LogPickup" rather than by reference
// equality so the patch survives save/load and asset reloads without lookup at startup.
[HarmonyPatch(typeof(Thought), nameof(Thought.CanRun))]
public static class KelvinSkipLooseLogPatch
{
    [HarmonyPrefix]
    public static bool Prefix(Thought __instance, ref bool __result)
    {
        if (__instance._stimuliTargetId == "LogPickup")
        {
            __result = false;
            return false;
        }
        return true;
    }
}
