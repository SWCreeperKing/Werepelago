using HarmonyLib;
using Werepelago.Archipelago;

namespace Werepelago.Patches;

[PatchAll]
public static class NpcPatch
{
    [HarmonyPatch(typeof(NPC), "SetNPCDied"), HarmonyPostfix]
    public static void NpcDied(NPC __instance)
    {
        WereClient.Client.SendLocation($"Kill {__instance.npcInfo.npcName}");
    }

    [HarmonyPatch(typeof(NPC), "Awake"), HarmonyPostfix]
    public static void NpcMarker(NPC __instance)
    {
        if (!WereClient.Client.MissingLocations.Contains($"Kill {__instance.npcInfo.npcName}")) return;
        var marker = __instance.gameObject.AddComponent<MinimapMarker>();
        marker.iconType = IconType.MascotRoom;
        marker.ShowMarker();
    }
}