using HarmonyLib;
using Werepelago.Archipelago;

namespace Werepelago.Patches;

[PatchAll]
public static class NpcPatch
{
    [HarmonyPatch(typeof(NPC), "SetNPCDied"), HarmonyPostfix]
    public static void NpcDied(NPC __instance)
    {
        WereClient.SendLocation($"Kill {__instance.npcInfo.npcName}");
    }
}