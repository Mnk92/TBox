using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using Mnk.Library.Common.Log;
using Mnk.Library.Common.MT;
using Mnk.TBox.Plugins.Market.Contracts;

namespace Mnk.TBox.Plugins.Market.Client.Code;

internal static class Synchronizer
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(Synchronizer));

    public static readonly PluginFiles PluginFiles = new();
    private static Uri EndPoint { get; set; }

    public static void Init(Config.ClientInfo info)
    {
        EndPoint = new Uri(info.EndPoint);
    }

    public static event Action<MarketService.MarketServiceClient> OnReloadData;

    public static IEnumerable<string> Types { get; private set; }
    public static IEnumerable<string> Authors { get; private set; }

    public static bool RefreshTables(IUpdater updater)
    {
        updater.Update("Connecting..", 1);
        var ret = false;
        Do(client =>
        {
            try
            {
                updater.Update("Refresh tables", 1);
                Types = client.GetTypeList(new Empty()).Items.OrderBy(x => x.ToLower());
                Authors = client.GetAuthorList(new Empty()).Items.OrderBy(x => x.ToLower());
                OnReloadData(client);
                ret = true;
            }
            catch (Exception ex)
            {
                Log.Write(ex, "Error refreshing data.");
            }
        });
        return ret;
    }

    public static void Do(Action<MarketService.MarketServiceClient> worker)
    {
        using var channel = GrpcChannel.ForAddress(EndPoint);
        var client = new MarketService.MarketServiceClient(channel);
        worker(client);
    }

    public delegate void InstallDelegate(Plugin plugin);

    public static event InstallDelegate OnInstall;

    public static void DoOnInstall(Plugin plugin)
    {
        OnInstall?.Invoke(plugin);
    }

    public static event InstallDelegate OnUninstall;

    public static void DoOnUninstall(Plugin plugin)
    {
        OnUninstall?.Invoke(plugin);
    }
}