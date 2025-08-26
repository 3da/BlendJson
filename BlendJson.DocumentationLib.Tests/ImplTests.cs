using System.Collections.Generic;
using BlendJson.TypeResolving;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace BlendJson.DocumentationLib.Tests
{
    [TestClass]
    public class ImplTests
    {
        [ResolveType]
        public interface IInterface
        {
            bool CommonField { get; set; }
        }

        public class Class1 : IInterface
        {
            public bool CommonField { get; set; }

            public string Field1 { get; set; }
        }

        public class Class2 : IInterface
        {
            public bool CommonField { get; set; }

            public string Field2 { get; set; }
        }

        public class Settings
        {
            public IInterface Interface { get; set; }

            public IList<IInterface> Interfaces { get; set; }
        }

        [TestMethod]
        public void Test()
        {
            var manager = new DocumentationManager();

            var documentation = manager.GenerateForTypes(typeof(Settings));

            var implementations = new List<MemberInfo>()
            {
                new MemberInfo()
                {
                    Type ="Class1",
                    Name = "Class1",
                    MemberType = MemberType.Class,
                    Children = new List<MemberInfo>()
                    {
                        new MemberInfo()
                        {
                            Type = "bool",
                            Name = "CommonField"
                        },
                        new MemberInfo()
                        {
                            Type = "string",
                            Name = "Field1"
                        },
                    }
                },
                new MemberInfo()
                {
                    Type ="Class2",
                    Name = "Class2",
                    MemberType = MemberType.Class,
                    Children = new List<MemberInfo>()
                    {
                        new MemberInfo()
                        {
                            Type = "bool",
                            Name = "CommonField"
                        },
                        new MemberInfo()
                        {
                            Type = "string",
                            Name = "Field2"
                        },
                    }
                },
            };

            var expected = new MemberInfo()
            {
                Type = nameof(Settings),
                Name = nameof(Settings),
                MemberType = MemberType.Class,
                Children = new List<MemberInfo>()
                {
                    new MemberInfo()
                    {
                        Name = "Interface",
                        Type = "IInterface",
                        MemberType = MemberType.Class,
                        Implementations = implementations,
                        Children = new List<MemberInfo>()
                        {
                            new MemberInfo()
                            {
                                Type = "bool",
                                Name = "CommonField"
                            }
                        }
                    },
                    new MemberInfo()
                    {
                        Name = "Interfaces",
                        Type = "IInterface[]",
                        MemberType = MemberType.Array,
                        Children = new List<MemberInfo>()
                        {
                            new MemberInfo()
                            {
                                Name = "Item",
                                Type = "IInterface",
                                MemberType = MemberType.Class,
                                Implementations = implementations,
                                Children = new List<MemberInfo>()
                                {
                                    new MemberInfo()
                                    {
                                        Type = "bool",
                                        Name = "CommonField"
                                    }
                                }
                            }
                        }
                    }
                }

            };

            Assert.AreEqual(JsonConvert.SerializeObject(expected, Formatting.Indented), JsonConvert.SerializeObject(documentation[0], Formatting.Indented));

        }
    }
}
