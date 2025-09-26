using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace BlendJson.Tests
{
    [TestClass]
    public class ModesTests
    {
        [TestMethod]
        public async Task Test()
        {
            var manager = new SettingsLoader();
            var settings = await manager.LoadSettingsAsync("Data/LoadModes/Settings.json");

            Assert.AreEqual(JToken.Parse(await File.ReadAllTextAsync("Data/LoadModes/Expected.json")).ToString(), settings.ToString());
        }
    }
}
