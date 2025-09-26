using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlendJson.DataSources;
using BlendJson.Serialization;
using BlendJson.Serialization.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlendJson.Tests
{
    [TestClass]
    public class LargeBinTests
    {
        public class Settings
        {
            [ToExternal(Mode = LoadMode.LargeBin, Path = "Content_{id}.bin")]
            public byte[][] Content { get; set; }
            public string Message { get; set; }
        }

        [TestMethod]
        public async Task MakeLargeZip()
        {
            var megabyteBuf = Enumerable.Range(0, 1024 * 1024).Select(q => (byte)(q % 256)).ToArray();

            var megabyteCount = 1024 * 5; //5GB

            var totalSize = megabyteCount * 1024L * 1024L;

            var settings = new Settings()
            {
                Message = "Total size: " + totalSize,
                Content = Enumerable.Range(0, megabyteCount).Select(_ => megabyteBuf).ToArray()
            };

            var serializer = new SettingsSerializer();
            await serializer.SaveZipAsync(settings, "Large.zip");


            var settingsManager = new BlendJson.SettingsLoader();

            var result = await settingsManager.LoadSettingsZipAsync<Settings>("Large.zip");

            Assert.AreEqual(settings.Message, result.Message);
            Assert.AreEqual(totalSize, result.Content.Sum(q => q.LongLength));
            Assert.AreEqual(0, result.Content[0][0]);
            Assert.AreEqual(255, result.Content.Last().Last());
        }

        public class Settings2
        {
            public byte[][] Binary { get; set; }
        }

        public class Settings3
        {
            public byte[] Binary { get; set; }
        }

        [TestMethod]
        public async Task LoadLargeBin()
        {
            var expected = Encoding.UTF8.GetBytes("test1488");

            var manager = new SettingsLoader();
            var settings = await manager.LoadSettingsAsync<Settings2>("Data/LoadModes/Settings2.json");

            Assert.IsTrue(expected.SequenceEqual(settings.Binary[0]));

            var settings3 = await manager.LoadSettingsAsync<Settings3>("Data/LoadModes/Settings2.json");

            Assert.IsTrue(expected.SequenceEqual(settings3.Binary));

            settings = await manager.LoadSettingsAsync<Settings2>("Data/LoadModes/Settings4.json");

            Assert.IsTrue(expected.SequenceEqual(settings.Binary[0]));

        }
    }
}
