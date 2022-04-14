namespace Mnk.TBox.Plugins.Market.Service.Domain
{
    sealed record Bug(ulong BugId, ulong PluginId, string Description, DateTime Date);
}
