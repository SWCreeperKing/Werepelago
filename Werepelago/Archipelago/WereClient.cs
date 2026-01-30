using Archipelago.MultiClient.Net.Enums;
using CreepyUtil.Archipelago;
using CreepyUtil.Archipelago.ApClient;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Werepelago.Archipelago;

public static class WereClient
{
    public static Dictionary<string, string> FrameIdMap;
    public static Dictionary<string, string> IdFrameMap;
    public static ApClient Client = new(new TimeSpan(0, 1, 0));
    public static ApData Data = new();
    public static HashSet<string> Items = [];
    
    public static void Init()
    {
        if (File.Exists("ApConnection.json"))
        {
            Data = JsonConvert.DeserializeObject<ApData>(File.ReadAllText("ApConnection.json").Replace("\r", ""));
        }

        Client.OnConnectionLost += () =>
        {
            // if (Core.Scene is "Game") GameUI.Instance.IngameMenuReturnToTitle();
            Core.Log.Error("Lost Connection to Ap");
        };

        Client.OnConnectionEvent += _ =>
        {
            try
            {
                Items.Clear();
            }
            catch (Exception e)
            {
                Core.Log.Error(e);
            }
        };

        Client.OnConnectionErrorReceived += (e, s) => { Core.Log.Error(e); };
    }

    [CanBeNull]
    public static string[] TryConnect(string addressPort, string password, string slotName)
    {
        var addressSplit = addressPort.Split(':');

        if (addressSplit.Length != 2) return ["Address Field is incorrect"];
        if (!int.TryParse(addressSplit[1], out var port)) return ["Port is incorrect"];

        var login = new LoginInfo(port, slotName, addressSplit[0], password);

        return Client.TryConnect(login, "Widget Inc", ItemsHandlingFlags.AllItems);
    }
    
    public static void SaveFile() => File.WriteAllText("ApConnection.json", JsonConvert.SerializeObject(Data));
    
    public static void Update()
    {
        try
        {
            if (Client is null) return;
            Client.UpdateConnection();

            if (!Client.IsConnected) return;
            
            foreach (var item in Client.GetOutstandingItems()!)
            {
                Items.Add(item.ItemName);
            }
        }
        catch (Exception e)
        {
            Core.Log.Error(e);
        }
    }
}