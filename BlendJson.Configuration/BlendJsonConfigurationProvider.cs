using BlendJson.DataSources;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace BlendJson.Configuration
{
    public class BlendJsonConfigurationProvider : ConfigurationProvider, IDisposable
    {
        public required SettingsLoader SettingsLoader { get; init; }
        public required IDataSource DataSource { get; init; }
        public bool ReloadOnChange { get; init; }
        public bool Optional { get; init; }

        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public override void Load()
        {
            try
            {
                Data = LoadAsync().GetAwaiter().GetResult();
            }
            catch (FileNotFoundException)
            {
                if (!Optional)
                    throw;
                Data = new Dictionary<string, string?>();
            }
        }

        public async Task<IDictionary<string, string?>> LoadAsync()
        {
            JToken jToken;
            if (DataSource is FileDataSource fileDataSource && ReloadOnChange)
            {
                JsonReference[] references;
                (jToken, references) = await SettingsLoader.LoadSettingsWithRefsAsync(DataSource);
                _ = WatchFilesAsync(references.Select(q => q.FullPath).Prepend(fileDataSource.Path).ToArray(), _cancellationTokenSource.Token);
            }
            else
            {
                jToken = await SettingsLoader.LoadSettingsAsync(DataSource);
            }
            if (jToken is not JObject jObject)
                throw new Exception("JSON file must contains JSON object in root");
            return JsonConfigurationFileParser.Parse(jObject);
        }

        async Task WatchFilesAsync(string[] paths, CancellationToken token = default)
        {
            foreach (var path in paths)
            {
                var dir = Path.GetDirectoryName(path);

                using var watcher = new FileSystemWatcher(dir, Path.GetFileName(path));
                watcher.NotifyFilter = NotifyFilters.LastWrite
                                       | NotifyFilters.FileName
                                       | NotifyFilters.Size;

                var taskCompletionSource = new TaskCompletionSource();
                token.Register(() => taskCompletionSource.SetCanceled(token));

                watcher.Changed += (_, _) => taskCompletionSource.TrySetResult();
                watcher.Created += (_, _) => taskCompletionSource.TrySetResult();
                watcher.Deleted += (_, _) => taskCompletionSource.TrySetResult();
                watcher.Renamed += (_, _) => taskCompletionSource.TrySetResult();

                watcher.EnableRaisingEvents = true;

                await taskCompletionSource.Task;
                try
                {
                    Data = await LoadAsync();
                    OnReload();
                }
                catch (FileNotFoundException)
                {
                    Data = new Dictionary<string, string?>();
                    OnReload();
                }
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }
    }
}
