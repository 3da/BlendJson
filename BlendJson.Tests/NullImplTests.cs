using BlendJson.TypeResolving;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace BlendJson.Tests
{
    [TestClass]
    public class NullImplTests
    {
        [ResolveType]
        public interface I
        {

        }

        public class C : I
        {

        }

        public class S
        {
            public I Field { get; set; }
            public I Field2 { get; set; }
        }

        [TestMethod]
        public void Test()
        {
            var manager = new SettingsLoader();

            var s = manager.LoadSettings<S>(JToken.Parse("{\"Field\": null, \"Field2\": {\"@Name\": \"C\"}}"));

            Assert.IsNotNull(s);
            Assert.IsNull(s.Field);
            Assert.IsNotNull(s.Field2);

        }
    }
}
