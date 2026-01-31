using HarmonyLib;
using UnityEngine;
using Werepelago.Archipelago;
using Object = UnityEngine.Object;

namespace Werepelago.Patches;

[PatchAll]
public static class CollectiblePatch
{
    [HarmonyPatch(typeof(Collectible), "Start"), HarmonyPrefix]
    public static bool ShowCollectible(Collectible __instance)
    {
        if (WereClient.Client.MissingLocations.Contains(
                WereClient.ItemIdToItem[__instance.GetPrivateField<CollectibleData>("collectible").collectibleID]
            ))
        {
            var marker = __instance.gameObject.AddComponent<MinimapMarker>();
            marker.iconType = IconType.MascotRoom;
            marker.ShowMarker();
            return false;
        }
        Object.Destroy(__instance.gameObject);
        return true;
    }

    [HarmonyPatch(typeof(SaveDataWrapper), "SetCollectibleCollected"), HarmonyPrefix]
    public static void GetCollectible(string collectibleID)
    {
        Core.Log.Msg($"Sending: [{collectibleID}]");
        WereClient.Client.SendLocation(WereClient.ItemIdToItem[collectibleID]);
    }
}