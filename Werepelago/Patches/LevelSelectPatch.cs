using HarmonyLib;
using Werepelago.Archipelago;

namespace Werepelago.Patches;

[PatchAll]
public static class LevelSelectPatch
{
    [HarmonyPatch(typeof(LevelSelectTab), "Start"), HarmonyPostfix]
    public static void LevelSelect(LevelSelectTab __instance)
    {
        __instance.gameObject.SetActive(
            WereClient.Items.Contains($"Unlock {WereClient.DayIdToDay[__instance.sceneName]} Night")
        );
        if (!WereClient.Client.MissingLocations.Contains(
                $"Survive {WereClient.DayIdToDay[__instance.sceneName]} Night"
            )) return;
        var child = __instance.GetChild(0);
        child.GetChild(5).SetActive(false);
        child.GetChild(6).SetActive(false);
        child.GetChild(7).SetActive(false);
        child.GetChild(8).SetActive(false);
    }
}