using BlendJson.TypeResolving;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlendJson.Tests
{
    [TestClass]
    public class DefaultImplTests
    {
        [ResolveType]
        public class BaseClass
        {
            public string A { get; set; }
        }

        public class SomeClass : BaseClass
        {
            public string B { get; set; }
        }


        [ResolveType(DefaultClass = typeof(SomeClass2))]
        public class BaseClass2
        {
            public string A { get; set; }
        }

        public class SomeClass2 : BaseClass2
        {
            public string B { get; set; }
        }


        [TestMethod]
        public void TestBaseClass()
        {
            var manager = new SettingsLoader();

            var json = """
                       {
                        "A": "Hello",
                        "B": "World"
                       }
                       """;

            var s = manager.LoadSettings<BaseClass>(JToken.Parse(json));

            Assert.IsNotNull(s);
            Assert.AreEqual(typeof(BaseClass), s.GetType());
            Assert.AreEqual("Hello", s.A);

        }

        [TestMethod]
        public void TestDefaultClass()
        {
            var manager = new SettingsLoader();

            var json = """
                       {
                        "A": "Hello",
                        "B": "World"
                       }
                       """;

            var s = manager.LoadSettings<BaseClass2>(JToken.Parse(json));

            Assert.IsNotNull(s);
            Assert.AreEqual(typeof(SomeClass2), s.GetType());
            var some = s as SomeClass2;
            Assert.IsNotNull(some);
            Assert.AreEqual("Hello", some.A);
            Assert.AreEqual("World", some.B);
        }
    }
}
