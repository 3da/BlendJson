using System.IO;
using System.Threading.Tasks;
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

			var settings = await settingsManager.LoadSettingsAsync(@"Data\HttpTest\Settings.json");

			var expectedSettings = JToken.Parse(await File.ReadAllTextAsync(@"Data\HttpTest\ExpectedSettings.json"));

			Assert.IsTrue(JToken.DeepEquals(expectedSettings, settings));
		}
	}
}
