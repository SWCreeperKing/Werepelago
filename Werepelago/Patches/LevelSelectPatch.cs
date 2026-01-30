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
    }
}