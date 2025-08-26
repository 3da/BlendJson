using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BlendJson.DataSources;
using BlendJson.Serialization;
using BlendJson.Serialization.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlendJson.Tests
{
    [TestClass]
    public class SerializeTests
    {
        class Settings
        {
            public string Title { get; set; }

            [ToExternal(Path = "Users")]
            public string[] Users { get; set; }

            [ToExternal(Path = "Roles")]
            public string[] Roles { get; set; }

            [ToExternal(Path = "Data", Mode = LoadMode.Bin)]
            public byte[] Data { get; set; }

            [ToExternal(Path = "Text", Mode = LoadMode.Text)]
            public string Text { get; set; }

            [ToExternal(Path = "Lines", Mode = LoadMode.Lines)]
            public string[] Lines { get; set; }
        }

        public static void CompareJsons(string path1, string path2)
        {
            Assert.IsTrue(JToken.DeepEquals(JToken.Parse(File.ReadAllText(path1)), JToken.Parse(File.ReadAllText(path2))));

        }

        public static void CompareBytes(string path1, string path2)
        {
            Assert.IsTrue(File.ReadAllBytes(path1).SequenceEqual(File.ReadAllBytes(path2)));
        }

        public static void CompareLines(string path1, string path2)
        {
            Assert.IsTrue(File.ReadAllLines(path1).SequenceEqual(File.ReadAllLines(path2)));
        }

        private Settings _settings = new Settings()
        {
            Title = "Hello",
            Roles = new[] { "Admin", "User", "Guest" },
            Users = new[] { "John", "Jane", "Jet" },
            Lines = new[] { "zxc", "123", "..." },
            Text = @"M
u
l
t
i",
            Data = new byte[] { 0, 1, 2, 3, 4, 5 }
        };

        [TestMethod]
        public async Task TestSerializeJson()
        {
            var serializer = new SettingsSerializer();

            var tmpDir = new DirectoryInfo("Tmp");
            if (tmpDir.Exists)
            {
                tmpDir.Delete(true);
                Thread.Sleep(100);
            }

            await serializer.SaveJsonAsync(_settings, "Tmp/settings.json");

            Assert.IsTrue(File.Exists("Tmp/settings.json"));
            Assert.IsTrue(File.Exists("Tmp/Users.json"));
            Assert.IsTrue(File.Exists("Tmp/Roles.json"));
            Assert.IsTrue(File.Exists("Tmp/Data.bin"));
            Assert.IsTrue(File.Exists("Tmp/Text.txt"));
            Assert.IsTrue(File.Exists("Tmp/Lines.txt"));

            CompareJsons("Data/Serialization/ExpectedSettings.json", "Tmp/settings.json");
            CompareJsons("Data/Serialization/Users.json", "Tmp/Users.json");
            CompareJsons("Data/Serialization/Roles.json", "Tmp/Roles.json");
            CompareBytes("Data/Serialization/Data.bin", "Tmp/Data.bin");
            CompareLines("Data/Serialization/Text.txt", "Tmp/Text.txt");
            CompareLines("Data/Serialization/Lines.txt", "Tmp/Lines.txt");

            var manager = new SettingsManager();

            var jsonSettings = await manager.LoadSettingsAsync("Tmp/settings");
            var typedSettings = jsonSettings.ToObject<Settings>();

            Assert.AreEqual(JsonConvert.SerializeObject(_settings), JsonConvert.SerializeObject(typedSettings));
        }

        [TestMethod]
        public async Task TestSerializeZip()
        {
            var serializer = new SettingsSerializer();

            await serializer.SaveZipAsync(_settings, "Tmp.zip");

            var tmpDir = new DirectoryInfo("Tmp");
            if (tmpDir.Exists)
            {
                tmpDir.Delete(true);
                Thread.Sleep(100);
            }

            ZipFile.ExtractToDirectory("Tmp.zip", "Tmp");

            Assert.IsTrue(File.Exists("Tmp/Main.json"));
            Assert.IsTrue(File.Exists("Tmp/Users.json"));
            Assert.IsTrue(File.Exists("Tmp/Roles.json"));
            Assert.IsTrue(File.Exists("Tmp/Data.bin"));
            Assert.IsTrue(File.Exists("Tmp/Text.txt"));
            Assert.IsTrue(File.Exists("Tmp/Lines.txt"));

            CompareJsons("Data/Serialization/Main.json", "Tmp/Main.json");
            CompareJsons("Data/Serialization/Users.json", "Tmp/Users.json");
            CompareJsons("Data/Serialization/Roles.json", "Tmp/Roles.json");
            CompareBytes("Data/Serialization/Data.bin", "Tmp/Data.bin");
            CompareLines("Data/Serialization/Text.txt", "Tmp/Text.txt");
            CompareLines("Data/Serialization/Lines.txt", "Tmp/Lines.txt");

            var manager = new SettingsManager();

            var jsonSettings = await manager.LoadSettingsAsync("Tmp.zip");
            var typedSettings = jsonSettings.ToObject<Settings>();

            Assert.AreEqual(JsonConvert.SerializeObject(_settings), JsonConvert.SerializeObject(typedSettings));
        }
    }
}
