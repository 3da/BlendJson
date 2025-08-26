using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlendJson.Tests
{
    [TestClass]
    public class FsHandlerTests
    {
        [TestMethod]
        public async Task Test()
        {
            var settingsManager = new SettingsManager();

            TestFsProvider fsProvider;
            settingsManager.FsProvider = fsProvider = new TestFsProvider();

            var settings = await settingsManager.LoadSettingsAsync("Data/ComplexTest/Settings.json");

            var expected = new string[]
            {
                "Data/ComplexTest/Settings.json",
                "Data/ComplexTest/Items.json",
                "Data/ComplexTest/AnotherDirectory/TwoItems.json",
                "Data/ComplexTest/AnotherDirectory/SomeItem.json",
                "Data/ComplexTest/AnotherDirectory/JustConst.json",
                "Data/ComplexTest/Address.json",
                "Data/ComplexTest/SomeObject.json",
                "Data/ComplexTest/SomeObject.json",
                "Data/ComplexTest/Merge1.json",
                "Data/ComplexTest/Merge2.json"
            };

            Assert.IsTrue(fsProvider.List1.Select(Path.GetFullPath).OrderBy(q => q).SequenceEqual(expected.Select(Path.GetFullPath).OrderBy(q => q)));
        }
    }
}
