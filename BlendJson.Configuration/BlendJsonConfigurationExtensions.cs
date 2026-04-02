using BlendJson.DataSources;
using Microsoft.Extensions.Configuration;

namespace BlendJson.Configuration
{
    public static class BlendJsonConfigurationExtensions
    {
        public static IConfigurationBuilder AddBlendJsonFile(this IConfigurationBuilder builder, string path)
        {
            return builder.AddBlendJsonFile(new FileDataSource() { Path = path }, optional: false, reloadOnChange: false);
        }

        public static IConfigurationBuilder AddBlendJsonFile(this IConfigurationBuilder builder, string path, bool optional)
        {
            return builder.AddBlendJsonFile(new FileDataSource() { Path = path }, optional, reloadOnChange: false);
        }

        public static IConfigurationBuilder AddBlendJsonFile(this IConfigurationBuilder builder, string path, bool optional, bool reloadOnChange)
        {
            return builder.AddBlendJsonFile(new FileDataSource() { Path = path }, optional, reloadOnChange);
        }

        public static IConfigurationBuilder AddBlendJsonFile(this IConfigurationBuilder builder, IDataSource dataSource, bool optional, bool reloadOnChange, SettingsLoader? settingsLoader = null)
        {
            ArgumentNullException.ThrowIfNull(builder);
            ArgumentNullException.ThrowIfNull(dataSource);

            if (settingsLoader == null)
                settingsLoader = new SettingsLoader();


            return builder.Add(new BlendJsonConfigurationSource()
            {
                DataSource = dataSource,
                SettingsLoader = settingsLoader,
                Optional = optional,
                ReloadOnChange = reloadOnChange
            });
        }
    }
}
