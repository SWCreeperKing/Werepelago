using JetBrains.Annotations;
using MelonLoader;
using UnityEngine;
using Werepelago;
using Werepelago.Archipelago;

[assembly: MelonInfo(typeof(Core), "Werepelago", Core.VersionNumber, "SW_CreeperKing", null)]
[assembly: MelonGame("USCGames", "The WereCleaner")]

namespace Werepelago;

public class Core : MelonMod
{
    public const string VersionNumber = "0.1.2";
    public static MelonLogger.Instance Log;

    public static GameObject PlayButton;

    public override void OnInitializeMelon()
    {
        Log = LoggerInstance;

        Log.Msg("Running ApShenanigans");

        ApShenanigans.RunShenanigans();

        Log.Msg("Ran ApShenanigans");

        var classesToPatch = MelonAssembly.Assembly.GetTypes()
                                          .Where(t => t.GetCustomAttributes(typeof(PatchAllAttribute), false).Any())
                                          .ToArray();

        Log.Msg($"Loading [{classesToPatch.Length}] Class patches");

        foreach (var patch in classesToPatch)
        {
            HarmonyInstance.PatchAll(patch);

            Log.Msg($"Loaded: [{patch.Name}]");
        }

        Log.Msg("Loading Data");

        WereClient.DayIdToDay = File.ReadAllLines($"{ApShenanigans.DataFolder}/levelIds.txt")
                                    .Select(s => s.Split(':'))
                                    .ToDictionary(arr => arr[1], arr => arr[0]);
        
        WereClient.ItemIdToItem = File.ReadAllLines($"{ApShenanigans.DataFolder}/itemIds.txt")
                                      .Select(s => s.Split(':'))
                                      .ToDictionary(arr => arr[1], arr => arr[0]);

        LoggerInstance.Msg("Setting up Client");

        WereClient.Init();

        LoggerInstance.Msg("Initialized.");

        ToolManager.SetVacuumUnlock(true);
        ToolManager.SetKnapperUnlock(true);
    }

    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        switch (sceneName)
        {
            case "MainMenu":
                GameObject.Find("Canvas/MainMenu/Buttons/NoTutorials").SetActive(false);
                GameObject.Find("Canvas/MainMenu/Buttons/WithTutorials").SetActive(true);

                PlayButton = GameObject.Find("Canvas/MainMenu/Buttons/WithTutorials/Play Button");
                var obj = new GameObject("AP Menu");
                obj.AddComponent<APGui>();
                break;
            case "ScoreCard":
                GameObject.Find("Canvas/ScoringPanel/Base/Content/ProgressContent/DefaultButtons/Next/Button").SetActive(false);
                break;
            default:
                if (sceneName.StartsWith("Level_") && sceneName.EndsWith("_V2")) { return; }

                Log.Msg($"Scene loaded: [{sceneName}]");
                break;
        }
    }


    public override void OnApplicationQuit() => WereClient.Client.TryDisconnect();

    public override void OnUpdate()
    {
        WereClient.Update();
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class PatchAllAttribute : Attribute;