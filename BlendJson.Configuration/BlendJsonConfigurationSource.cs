using BlendJson.DataSources;
using Microsoft.Extensions.Configuration;


namespace BlendJson.Configuration
{
    public class BlendJsonConfigurationSource : IConfigurationSource
    {
        public required SettingsLoader SettingsLoader { get; init; }
        public required IDataSource DataSource { get; init; }
        public bool ReloadOnChange { get; init; }
        public bool Optional { get; init; }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
            => new BlendJsonConfigurationProvider()
            {
                DataSource = DataSource,
                SettingsLoader = SettingsLoader,
                Optional = Optional,
                ReloadOnChange = ReloadOnChange,
            };
    }
}
