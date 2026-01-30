using HarmonyLib;

namespace Werepelago.Patches;

[PatchAll]
public static class CollectiblePatch
{
    [HarmonyPatch(typeof(Collectible), "Start"), HarmonyPrefix]
    public static bool ShowCollectible(Collectible __instance)
    {
        return false;
    }
    
    [HarmonyPatch(typeof(Collectible), "OnInteract"), HarmonyPostfix]
    public static void GetCollectible(Collectible __instance)
    {
    }
}