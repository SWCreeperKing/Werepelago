using HarmonyLib;
using Werepelago.Archipelago;

namespace Werepelago.Patches;

[PatchAll]
public static class ToolPatch
{
    [HarmonyPatch(typeof(PowerWasherController), "PowerWasherFixedUpdate"), HarmonyPrefix]
    public static bool StopPowerwash(PowerWasherController __instance, bool isHoldingPowerWasher)
    {
        if (!isHoldingPowerWasher)
        {
            __instance.DeactivateVFX();
        }
        
        return WereClient.Items.Contains("Washer");
    }
    
    [HarmonyPatch(typeof(VacuumController), "VacuumFixedUpdate"), HarmonyPrefix]
    public static bool StopVacuum()
    {
        return WereClient.Items.Contains("Vacuum");
    }

    [HarmonyPatch(typeof(Knapper), "UpdateKnapperStatus"), HarmonyPrefix]
    public static bool StopKnapper()
    {
        return WereClient.Items.Contains("Knapper");
    }
}