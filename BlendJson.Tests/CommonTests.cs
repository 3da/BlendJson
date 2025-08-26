using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace BlendJson.Tests
{
    [TestClass]
    public class CommonTests
    {
        private async Task TestAsync(string expectedPath, string actualPath)
        {
            var settingsManager = new SettingsManager();

            var settings = await settingsManager.LoadSettingsAsync(actualPath);

            var expectedSettings = JToken.Parse(await File.ReadAllTextAsync(expectedPath));

            Assert.IsTrue(JToken.DeepEquals(expectedSettings, settings));
        }

        [TestMethod]
        public async Task TestComplexSettings()
        {
            await TestAsync(Path.Combine("Data", "ComplexTest", "ExpectedSettings.json"), Path.Combine("Data", "ComplexTest", "Settings.json"));
        }

        [TestMethod]
        public async Task TestMergeTwoTrees()
        {
            await TestAsync(Path.Combine("Data", "TwoTrees", "ExpectedSettings.json"), Path.Combine("Data", "TwoTrees", "Settings.json"));
        }
    }
}
