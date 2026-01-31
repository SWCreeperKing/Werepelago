using HarmonyLib;
using UnityEngine.SceneManagement;
using Werepelago.Archipelago;

namespace Werepelago.Patches;

[PatchAll]
public static class LevelWinPatch
{
    [HarmonyPatch(typeof(GameManager), "EndLevel"), HarmonyPostfix]
    public static void LevelWon(bool success)
    {
        if (!success) return;
        WereClient.Client.SendLocation($"Survive {WereClient.DayIdToDay[SceneManager.GetActiveScene().name]} Night");
        WereClient.CompletedLevels.Add(SceneManager.GetActiveScene().name);
        WereClient.Client.SendToStorage("levels_completed", WereClient.CompletedLevels.ToArray());
        
        if (WereClient.Client.HasGoaled || WereClient.CompletedLevels.Count < 7) return;
        WereClient.Client.Goal();
    }
}