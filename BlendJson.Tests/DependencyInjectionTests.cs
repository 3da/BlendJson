using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace BlendJson.Tests
{
    [TestClass]
    public class DependencyInjectionTests
    {
        public class MyClass
        {
            public string Field { get; set; }
        }

        public class ClassWithInjection
        {
            private readonly MyClass _class;

            public ClassWithInjection(MyClass @class)
            {
                _class = @class;
            }

            public string Method() => _class.Field + Number;

            public string Text { get; set; }

            public int Number { get; set; }
        }

        public class Settings
        {
            public ClassWithInjection Field { get; set; }
        }

        [TestMethod]
        public void Test()
        {
            var services = new ServiceCollection();

            services.AddSingleton(new MyClass() { Field = "Hello" });

            var provider = services.BuildServiceProvider(new ServiceProviderOptions());

            var manager = new SettingsManager()
            {
                ServiceProvider = provider
            };

            var settings = manager.LoadSettings<Settings>(JToken.Parse("{ \"Field\": { \"Number\": 59, \"Text\": \"zxc\" }}"));

            Assert.AreEqual("Hello59", settings.Field.Method());
        }
    }
}
