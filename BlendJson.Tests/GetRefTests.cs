using System.IO;
using System.Threading.Tasks;
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
        public async Task TestGetRefsFromJson()
        {
            var settingsManager = new SettingsManager();

            var (refs, json) = await settingsManager.LoadRefsAsync("Data\\Example1\\Settings.json");

            CompareRefs([new JsonReference("Colors", "Data\\Example1\\Colors.json"),
                new JsonReference("Websites", "Data\\Example1\\Websites.json"),
                new JsonReference("RemoteCredentials", "Data\\Example1\\RemoteCredentials.json")], refs);

            Assert.AreEqual(await File.ReadAllTextAsync("Data\\Example1\\Settings.json"), json.ToString());


            (refs, json) = await settingsManager.LoadRefsAsync("Data\\ComplexTest\\Settings.json");

            CompareRefs([new JsonReference("Items", "Data\\ComplexTest\\Items.json"),
                new JsonReference("Address", "Data\\ComplexTest\\Address.json"),
                new JsonReference("SomeObject", "Data\\ComplexTest\\SomeObject.json"),
                new JsonReference("Merge1","Data\\ComplexTest\\Merge1.json"),
                new JsonReference("Merge2","Data\\ComplexTest\\Merge2.json")], refs);

        }
    }
}
