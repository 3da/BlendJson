using System.IO;
using System.Threading.Tasks;
using BlendJson.ExternalDataSources;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace BlendJson.Tests
{
	[TestClass]
	public class HttpTests
	{

		[TestMethod]
		public async Task Test()
		{
			var settingsManager = new SettingsManager();

            settingsManager.AddDataSource<HttpDataSource>();

			var settings = await settingsManager.LoadSettingsAsync("Data/HttpTest/Settings.json");

			var expectedSettings = JToken.Parse(await File.ReadAllTextAsync("Data/HttpTest/ExpectedSettings.json"));

			Assert.IsTrue(JToken.DeepEquals(expectedSettings, settings));
		}
	}
}
