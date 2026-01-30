using HarmonyLib;

namespace Werepelago.Patches;

// [PatchAll]
public static class TimerPatch
{
    [HarmonyPatch(typeof(GameManager), "Update"), HarmonyPostfix]
    public static void Timer(GameManager __instance)
    {
        __instance.SetPrivateField("timer", __instance.GetPrivateField<float>("startTime"));
    }
}