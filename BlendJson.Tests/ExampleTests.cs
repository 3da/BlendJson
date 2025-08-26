using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlendJson.Tests
{
    [TestClass]
    public class ExampleTests
    {
        [TestMethod]
        public async Task TestExample1()
        {
            var settingsManager = new SettingsManager();

            var settings = await settingsManager.LoadSettingsAsync("Data/Example1/Settings.json");
            var expectedSettings = await settingsManager.LoadSettingsAsync("Data/Example1/ExpectedSettings.json");

            Assert.AreEqual(expectedSettings.ToString(), settings.ToString());
        }
    }
}
