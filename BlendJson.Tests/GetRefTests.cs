using System.IO;
using System.Threading.Tasks;
using BlendJson.DataSources;
using BlendJson.FsProviders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlendJson.Tests
{
    [TestClass]
    public class GetRefTests
    {
        void CompareRefs(JsonReference[] expected, JsonReference[] actual)
        {
            Assert.AreEqual(expected.Length, actual.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i]);
            }
        }

        [TestMethod]
        public async Task TestGetRefsFromJsonFile()
        {
            var settingsManager = new SettingsLoader();

            var (refs, json) = await settingsManager.LoadRefsAsync("Data/Example1/Settings.json");

            CompareRefs([new JsonReference("Colors", Path.Combine("Data","Example1","Colors.json")),
                new JsonReference("Websites", Path.Combine("Data","Example1","Websites.json")),
                new JsonReference("RemoteCredentials", Path.Combine("Data","Example1","RemoteCredentials.json"))], refs);

            Assert.AreEqual(await File.ReadAllTextAsync("Data/Example1/Settings.json"), json.ToString());


            (refs, json) = await settingsManager.LoadRefsAsync("Data/ComplexTest/Settings.json");

            CompareRefs([new JsonReference("Items", Path.Combine("Data","ComplexTest","Items.json")),
                new JsonReference("Address", Path.Combine("Data","ComplexTest","Address.json")),
                new JsonReference("SomeObject", Path.Combine("Data","ComplexTest","SomeObject.json")),
                new JsonReference("Merge1",Path.Combine("Data","ComplexTest","Merge1.json")),
                new JsonReference("Merge2",Path.Combine("Data","ComplexTest","Merge2.json"))], refs);

        }

        [TestMethod]
        public async Task TestGetRefsFromJsonString()
        {
            var sourcePath = Path.Combine("Data", "Example1", "Settings.json");

            var settingsManager = new SettingsLoader();
            var stringSource = new StringDataSource(await File.ReadAllTextAsync(sourcePath));
            var (refs, json) = await settingsManager.LoadRefsAsync(stringSource);

            CompareRefs([new JsonReference("Colors", null),
                new JsonReference("Websites", null),
                new JsonReference("RemoteCredentials", null)], refs);

            Assert.AreEqual(await File.ReadAllTextAsync(sourcePath), json.ToString());

            var sourceWorkDir = Path.Combine("Data", "Example1");

            stringSource = new StringDataSource(await File.ReadAllTextAsync(sourcePath)) { WorkDir = sourceWorkDir };
            (refs, json) = await settingsManager.LoadRefsAsync(stringSource);

            CompareRefs([new JsonReference("Colors", Path.Combine("Data","Example1","Colors.json")),
                new JsonReference("Websites", Path.Combine("Data","Example1","Websites.json")),
                new JsonReference("RemoteCredentials", Path.Combine("Data","Example1","RemoteCredentials.json"))], refs);

            Assert.AreEqual(await File.ReadAllTextAsync(sourcePath), json.ToString());
        }

        [TestMethod]
        public async Task TestGetRefsForNonExistentFiles()
        {
            var sourcePath = Path.Combine("Data", "Example1", "Settings.json");

            var settingsManager = new SettingsLoader
            {
                FsProvider = NullFsProvider.Instance
            };

            var stringSource = new StringDataSource(await File.ReadAllTextAsync(sourcePath)) { WorkDir = Path.Combine("Some", "FakeDir") };
            var (refs, json) = await settingsManager.LoadRefsAsync(stringSource, false);

            CompareRefs([new JsonReference("Colors", Path.Combine("Some","FakeDir","Colors.json")),
                new JsonReference("Websites", Path.Combine("Some","FakeDir","Websites.json")),
                new JsonReference("RemoteCredentials", Path.Combine("Some","FakeDir","RemoteCredentials.json"))], refs);

            Assert.AreEqual(await File.ReadAllTextAsync(sourcePath), json.ToString());
        }
    }
}
