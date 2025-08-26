using System.Threading.Tasks;
using BlendJson.Templating;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlendJson.Tests
{
    [TestClass]
    public class TemplatingTests
    {
        [TestMethod]
        public async Task Test()
        {
            var globals = new Gl { SomeVariable = 1, AnotherVariable = 0 };

            var manager = new SettingsManager(new EvalProcessor(globals), new ConditionProcessor(globals));

            Assert.AreEqual((await manager.LoadSettingsAsync("Data/Templating/ExpectedSettings2.json")).ToString(),
                (await manager.LoadSettingsAsync("Data/Templating/Settings.json")).ToString());

            globals.SomeVariable = 6;
            globals.AnotherVariable = 0;

            Assert.AreEqual((await manager.LoadSettingsAsync("Data/Templating/ExpectedSettings1.json")).ToString(),
                (await manager.LoadSettingsAsync("Data/Templating/Settings.json")).ToString());

            globals.AnotherVariable = 1;

            Assert.AreEqual((await manager.LoadSettingsAsync("Data/Templating/ExpectedSettings3.json")).ToString(),
                (await manager.LoadSettingsAsync("Data/Templating/Settings.json")).ToString());


        }
    }

    public class Gl
    {
        public int SomeVariable { get; set; }
        public int AnotherVariable { get; set; }
    }
}
