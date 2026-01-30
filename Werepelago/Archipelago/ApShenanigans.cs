namespace Werepelago.Archipelago;

public static class ApShenanigans
{
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
        
    }
}