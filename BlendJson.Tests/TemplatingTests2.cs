using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BlendJson.Templating;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlendJson.Tests
{
    [TestClass]
    public class TemplatingTests2
    {
        [TestMethod]
        public async Task Test2()
        {
            var globals = new Gl { };

            var manager = new SettingsLoader(new EvalProcessor(globals), new ConditionProcessor(globals));

            Assert.IsTrue(File.Exists("Data/Templating2/Data.bin"));

            var ss1 = (await manager.LoadSettingsAsync("Data/Templating2/Settings.json")).ToString();

            Assert.IsTrue(File.Exists("Data/Templating2/Data.bin"));

        }

        [TestMethod]
        public async Task TestLoadWithParameters()
        {
            var manager = new SettingsLoader(new EvalProcessor(null), new ConditionProcessor(null));

            var settings = await manager.LoadSettingsAsync("Data/LoadWithParams/Settings.json");

            var settings2 = await manager.LoadSettingsAsync("Data/LoadWithParams/Expected.json");

            Assert.AreEqual(settings2.ToString(), settings.ToString());

        }

        [TestMethod]
        public async Task TestLoadWithParameters2()
        {
            var globals = new Dictionary<string, object>()
            {
                ["Var1"] = 10,
                ["Var2"] = true
            };

            var manager = new SettingsLoader(new EvalProcessor(globals), new ConditionProcessor(globals));

            var settings = await manager.LoadSettingsAsync("Data/LoadWithParams/External.json");

            var settings2 = await manager.LoadSettingsAsync("Data/LoadWithParams/Expected2.json");

            Assert.AreEqual(settings2.ToString(), settings.ToString());

        }

        [TestMethod]
        public async Task TestLoadWithParameters3()
        {
            var globals = new Dictionary<string, object>()
            {
                ["Var1"] = 0.33333333333333333f,
                ["Var2"] = true
            };

            var manager = new SettingsLoader(new EvalProcessor(globals), new ConditionProcessor(globals));

            var settings = await manager.LoadSettingsAsync("Data/LoadWithParams/Settings3.json");

            var settings2 = await manager.LoadSettingsAsync("Data/LoadWithParams/Expected3.json");

            Assert.AreEqual(settings2.ToString(), settings.ToString());

        }

        [TestMethod]
        public async Task TestLoadWithParameters4()
        {
            var globals = new Dictionary<string, object>()
            {
            };

            var manager = new SettingsLoader(new EvalProcessor(globals));

            var settings = await manager.LoadSettingsAsync("Data/LoadWithParams/A4.json");

            var settings2 = await manager.LoadSettingsAsync("Data/LoadWithParams/E4.json");

            Assert.AreEqual(settings2.ToString(), settings.ToString());

        }
    }

}
