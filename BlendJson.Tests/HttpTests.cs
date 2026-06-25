using System.IO;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using BlendJson.ExternalDataSources;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlendJson.Tests
{
	[TestClass]
	public class HttpTests
	{

		[TestMethod]
		public async Task Test()
		{
			var settingsManager = new SettingsLoader();

            settingsManager.AddDataSource<HttpDataSource>();

			var settings = await settingsManager.LoadSettingsAsync("Data/HttpTest/Settings.json");

			var expectedSettings = JsonNode.Parse(await File.ReadAllTextAsync("Data/HttpTest/ExpectedSettings.json"));

			Assert.IsTrue(JsonNode.DeepEquals(expectedSettings, settings));
		}
	}
}
