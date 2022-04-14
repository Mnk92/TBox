namespace Mnk.TBox.Plugins.Market.Service.Domain
{
    class Plugin
    {
        public ulong PluginId { get; set; }
        public ulong AuthorId { get; set; }
        public virtual Author Author { get; set; }
        public ulong PluginTypeId { get; set; }
        public virtual PluginType PluginType { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public ulong Downloads { get; set; }
        public ulong Uploads { get; set; }
        public ulong Size { get; set; }
        public bool IsPlugin { get; set; }
    };
}
