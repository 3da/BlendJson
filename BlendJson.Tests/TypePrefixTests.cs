using BlendJson.TypeResolving;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BlendJson.Tests
{
    [TestClass]
    public class TypePrefixTests
    {
        [ResolveType(TypePostfix = "Thing")]
        public interface IThing
        {
        }

        public class BigThing : IThing
        {
        }

        public class BiggerThing : IThing
        {
        }

        public class BiggestThing : IThing
        {
        }

        [ResolveType(TypePrefix = "Rich")]
        public interface IRichPerson
        {
        }

        public class RichBoy : IRichPerson
        {
        }

        public class RichGirl : IRichPerson
        {
        }

        public class RichDog : IRichPerson
        {
        }

        [ResolveType(TypePrefix = "Long", TypePostfix = "Rope")]
        public interface ILongRope
        {
        }

        public class LongSilkRope : ILongRope
        {
        }



        public class Settings
        {
            public IList<IThing> Things { get; set; }
            public IList<IRichPerson> Riches { get; set; }
            public IList<ILongRope> Ropes { get; set; }
        }

        [TestMethod]
        public async Task TestPostfixAndPrefix()
        {
            var settingsLoader = new SettingsLoader();
            var settings = await settingsLoader.LoadSettingsAsync<Settings>("Data/TypePrefix/Settings.json");
            Assert.AreEqual(typeof(BigThing), settings.Things[0].GetType());
            Assert.AreEqual(typeof(BiggerThing), settings.Things[1].GetType());
            Assert.AreEqual(typeof(BiggestThing), settings.Things[2].GetType());

            Assert.AreEqual(typeof(RichBoy), settings.Riches[0].GetType());
            Assert.AreEqual(typeof(RichGirl), settings.Riches[1].GetType());
            Assert.AreEqual(typeof(RichDog), settings.Riches[2].GetType());

            Assert.AreEqual(typeof(LongSilkRope), settings.Ropes[0].GetType());
        }

    }
}
