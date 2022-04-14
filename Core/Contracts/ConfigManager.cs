namespace Mnk.TBox.Core.Contracts
{
    public interface IConfigManager<out TConfig>
    {
        TConfig Config { get; }
    }

    public class ConfigManager<TConfig> : IConfigManager<TConfig>
    {
        public TConfig Config { get; set; }
    }
}
