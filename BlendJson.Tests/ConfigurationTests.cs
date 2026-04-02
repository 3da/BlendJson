using BlendJson.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BlendJson.Tests
{
    [TestClass]
    public class ConfigurationTests
    {

        [TestMethod]
        public void TestLoadLocalFile()
        {
            var builder = new ConfigurationBuilder();
            builder.AddBlendJsonFile("Data\\Example1\\Settings.json");

            var config = builder.Build();

            var colors = config.GetSection("Colors").Get<string[]>();

            Assert.AreEqual(3, colors.Length);
            Assert.AreEqual("Red", colors[0]);
            Assert.AreEqual("Green", colors[1]);
            Assert.AreEqual("Blue", colors[2]);
        }

        [TestMethod]
        public void TestLoadOptionalFile()
        {
            var builder = new ConfigurationBuilder();
            builder.AddBlendJsonFile("Data\\not_exists.json", optional: true);

            var config = builder.Build();

            Assert.AreEqual(false, config.GetChildren().Any());
        }

        [TestMethod]
        public void TestNotExistFile()
        {
            var builder = new ConfigurationBuilder();
            builder.AddBlendJsonFile("Data\\not_exists.json", optional: false);

            Assert.ThrowsExactly<FileNotFoundException>(() => builder.Build());
        }

        [TestMethod]
        public async Task TestReload()
        {
            var testDir = new DirectoryInfo("ConfigurationTests_Temp");
            if (testDir.Exists)
                testDir.Delete(true);

            testDir.Create();

            var mainFilePath = Path.Combine(testDir.FullName, "main.json");
            var subFilePath = Path.Combine(testDir.FullName, "sub.json");

            async Task UpdateMainFileAsync(string content)
            {
                await File.WriteAllTextAsync(mainFilePath, content);
            }

            async Task UpdateSubFileAsync(string content)
            {
                await File.WriteAllTextAsync(subFilePath, content);
            }

            var mainFileContent = @"{""A"":""B"", ""Sub"":{""@LoadFrom"": ""sub.json""}}";
            var mainFileContent2 = @"{""A"":""C"", ""Sub"":{""@LoadFrom"": ""sub.json""}}";
            var subContent = "100";
            var subContent2 = "999";




            var builder = new ConfigurationBuilder();
            builder.AddBlendJsonFile(mainFilePath, optional: false, reloadOnChange: true);

            Assert.ThrowsExactly<FileNotFoundException>(() => builder.Build());

            await UpdateMainFileAsync(mainFileContent);
            await UpdateSubFileAsync(subContent);

            var config = builder.Build();
            Assert.AreEqual("B", config.GetValue<string>("A"));
            Assert.AreEqual(100, config.GetValue<int>("Sub"));

            await UpdateMainFileAsync(mainFileContent2);
            await Task.Delay(100);
            Assert.AreEqual("C", config.GetValue<string>("A"));
            Assert.AreEqual(100, config.GetValue<int>("Sub"));

            await UpdateSubFileAsync(subContent2);
            await Task.Delay(100);
            Assert.AreEqual("C", config.GetValue<string>("A"));
            Assert.AreEqual(999, config.GetValue<int>("Sub"));

            File.Delete(mainFilePath);
            await Task.Delay(100);
            Assert.AreEqual(false, config.GetChildren().Any());

        }
    }
}
