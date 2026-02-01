using CreepyUtil.Archipelago;
using CreepyUtil.Archipelago.WorldFactory;
using static CreepyUtil.Archipelago.WorldFactory.PremadePython;

namespace Werepelago.Archipelago;

public static class ApShenanigans
{
    // Spreadsheet used for logic:
    // https://docs.google.com/spreadsheets/d/1wrzYGdzRh6-fmsBK-dAzhe72PdNrhvEZKUcQJCXDaJ4/edit?usp=sharing

    public const string Spreadsheet = "The WereCleaner - Sheet1.csv";
    public const string DataFolder = "Mods/SW_CreeperKing.Werepelago/Data";

    public const string FileLink
        = "https://github.com/SWCreeperKing/Werepelago/blob/master/Werepelago/Archipelago/ApShenanigans.cs";

    public const string ApWorldOutput = "E:/coding projects/python/Deathipelago/worlds/the_werecleaner";

    public static void RunShenanigans()
    {
        if (!File.Exists(Spreadsheet)) return;
        if (!Directory.Exists("output")) Directory.CreateDirectory("output");
        if (!Directory.Exists(DataFolder)) Directory.CreateDirectory(DataFolder);

        new CsvParser(Spreadsheet, 1, 0).ToFactory()
                                        .ReadTable(new LevelDataCreator(), 3, out var levelData).SkipColumn()
                                        .ReadTable(new ItemDataCreator(), 3, out var itemData).SkipColumn()
                                        .ReadTable(new NpcDataCreator(), 3, out var npcData);

        var rawNpcs = npcData.SelectMany(data => data.Npcs).Where(s => s.Trim() is not "").ToHashSet().ToArray();
        var allNpcs = rawNpcs.Select(data => $"Kill {data}").ToArray();
        var days = levelData.Select(data => $"Survive {data.LevelName} Night").ToArray();
        var dayUnlocks = levelData.Select(data => $"Unlock {data.LevelName} Night").ToArray();
        var collectibles = itemData.Select(data => data.Collectible).ToArray();
        var abilities = itemData.Select(data => data.Ability).Where(s => s is not "").ToArray();

        var worldFactory = new WorldFactory("The WereCleaner")
                          .SetOnCompilerError((e, s) => Core.Log.Error(s, e))
                          .SetOutputDirectory(ApWorldOutput);

        worldFactory.GetOptionsFactory(FileLink)
                    // .AddOption("Kill Sanity", "Killing each unique npc sends a check", new Toggle())
                    .AddCheckOptions()
                    .GenerateOptionFile();

        worldFactory.GetLocationFactory(FileLink)
                    .AddLocations("starting_checks", [["Starting Check (Washer)", "Menu"], ["Starting Check (Unlock Monday Night)", "Menu"]])
                    .AddLocations("collectibles", collectibles.Select(s => (string[])[s, "Collectibles"]))
                    .AddLocations("npcs", allNpcs.Select(s => (string[])[s, "Killsanity"]))
                    .AddLocations("levels", days.Select(s => (string[])[s, "Levels"]))
                    .GenerateLocationFile();

        worldFactory.GetItemFactory(FileLink)
                    .AddItems(ItemFactory.ItemClassification.Progression, items: abilities)
                    .AddItems(ItemFactory.ItemClassification.Progression, items: dayUnlocks)
                    .AddItem("Floor Penny", ItemFactory.ItemClassification.Filler)
                    .AddCreateItems(method => method
                                             .AddCode(CreateItemsFromClassificationList())
                                             // .AddCode("""
                                             //                 for item, classification in item_table.items():
                                             //                     world.location_count -= 1
                                             //                     if item != "Unlock Monday Night":
                                             //                         pool.append(world.create_item(item))
                                             //                 """)
                                                    .AddCode(CreateItemsFillRemainingWithItem("Floor Penny"))
                     )
                    .GenerateItemsFile();

        worldFactory.GetRuleFactory(FileLink)
                    .AddLogicFunction("level", "has_level", StateHasR("f\"Unlock {level} Night\""), "level")
                    .AddLogicFunction("Washer", "has_washer", StateHasSR("Washer"))
                    .AddLogicFunction("Vacuum", "has_vacuum", StateHasSR("Vacuum"))
                    .AddLogicFunction("Knapper", "has_knapper", StateHasSR("Knapper"))
                    .AddLogicRules(
                         collectibles.ToDictionary(
                             s => s, s => string.Join(
                                 " or ", levelData.Where(data => data.Collectibles.Contains(s))
                                                   .Select(s => $"level[\"{s.LevelName}\"]")
                             )
                         )
                     ).AddLogicRules(
                         rawNpcs.ToDictionary(
                             s => $"Kill {s}", s => string.Join(
                                 " or ", npcData.Where(data => data.Npcs.Contains(s))
                                                 .Select(s => $"level[\"{s.LevelName}\"]")
                             )
                         )
                     ).AddLogicRules(
                         levelData.ToDictionary(
                             data => $"Survive {data.LevelName} Night", data => $"level[\"{data.LevelName}\"] and {string.Join(" and ", data.Abilities)}"
                         )
                     )
                    .GenerateRulesFile();

        worldFactory.GetRegionFactory(FileLink)
                    .AddRegions("Levels", "Collectibles", "Killsanity")
                    .AddConnection("Menu", "Levels")
                    .AddConnection("Menu", "Collectibles")
                    // .AddConnection("Menu", "Killsanity", condition: "world.options.kill_sanity")
                    .AddConnection("Menu", "Killsanity")
                    .AddLocationsFromList("starting_checks")
                    .AddLocationsFromList("collectibles")
                    .AddLocationsFromList("levels")
                    // .AddLocationsFromList("npcs", condition: "world.options.kill_sanity")
                    .AddLocationsFromList("npcs")
                    .AddEventLocationsFromList(
                         "levels", "f\"Beat: {location[0]}\"", "\"Nights Survived\""
                     )
                    .GenerateRegionFile();

        worldFactory.GetInitFactory(FileLink)
                    .UseInitFunction(method => method.AddCode(new Variable("self.starting_stage", "\"\"")))
                    .AddUseUniversalTrackerPassthrough(yamlNeeded: false)
                    // .UseGenerateEarly(method => method.AddCode(CreatePushPrecollected("Unlock Monday Night")))
                    .UseCreateRegions()
                    .AddCreateItems()
                    .UseSetRules(method => method.AddCode(
                             "self.multiworld.completion_condition[self.player] = lambda state: state.has(\"Nights Survived\", self.player, 7)"
                         )
                     )
                    .UseFillSlotData(new Dictionary<string, string>{["Kyle"] = "str(\"Best Boi\")"})
                    .InjectCodeIntoWorld(world => world.AddVariable(new Variable("gen_puml", "False")))
                    .UseGenerateOutput(method => method.AddCode(PumlGenCode()))
                    .GenerateInitFile();

        worldFactory.GenerateArchipelagoJson("0.6.5", Core.VersionNumber, "SW_CreeperKing");

        File.WriteAllLines($"{DataFolder}/levelIds.txt", npcData.Select(data => $"{data.LevelName}:{data.LevelId}"));
        File.WriteAllLines(
            $"{DataFolder}/itemIds.txt", itemData.Select(data => $"{data.Collectible}:{data.CollectibleId}")
        );

        if (File.Exists($"output/{Spreadsheet}")) { File.Delete($"output/{Spreadsheet}"); }

        File.Move(Spreadsheet, $"output/{Spreadsheet}");
    }
}

public readonly struct LevelData(string[] param)
{
    public readonly string LevelName = param[0];
    public readonly string[] Collectibles = param[1].Split(',').Select(s => s.Trim()).ToArray();
    public readonly string[] Abilities = param[2].Split(',').Select(s => s.Trim()).ToArray();
}

public readonly struct ItemData(string[] param)
{
    public readonly string Collectible = param[0];
    public readonly string CollectibleId = param[1];
    public readonly string Ability = param[2];
}

public readonly struct NpcData(string[] param)
{
    public readonly string LevelName = param[0];
    public readonly string LevelId = param[1];
    public readonly string[] Npcs = param[2].Split(',').Select(s => s.Trim()).ToArray();
}

public class LevelDataCreator : CsvTableRowCreator<LevelData>
{

    public override LevelData CreateRowData(string[] param) => new(param);
}

public class ItemDataCreator : CsvTableRowCreator<ItemData>
{

    public override ItemData CreateRowData(string[] param) => new(param);
}

public class NpcDataCreator : CsvTableRowCreator<NpcData>
{
    public override NpcData CreateRowData(string[] param) => new(param);
}