using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        [ResolveType(TypePrefix = "Evil", TypePostfix = "Spy")]
        public interface IEvilSpy
        {
        }

        public class EvilRussianSpy : IEvilSpy
        {
        }

        public class Settings
        {
            public IInterface Interface { get; set; }

            public IList<IInterface> Interfaces { get; set; }

            public IEvilSpy Spy { get; set; }
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

            var spyImplementations = new List<MemberInfo>()
            {
                new MemberInfo()
                {
                    Type ="EvilRussianSpy",
                    Name = "Russian",
                    MemberType = MemberType.Class,
                    Children = new List<MemberInfo>()
                    {
                    }
                }
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
                    },
                    new MemberInfo()
                    {
                        Name = "Spy",
                        Type = "IEvilSpy",
                        MemberType = MemberType.Class,
                        Implementations = spyImplementations,
                        Children = new List<MemberInfo>()

                    }
                }

            };

            var expectedLines = JsonConvert.SerializeObject(expected, Formatting.Indented).Split("\n");
            var actualLines = JsonConvert.SerializeObject(documentation[0], Formatting.Indented).Split('\n');

            for (int i = 0; i < Math.Min(expectedLines.Length, actualLines.Length); i++)
            {
                Console.WriteLine(expectedLines[i]);
                Assert.AreEqual(expectedLines[i], actualLines[i]);
            }

            Assert.AreEqual(expectedLines.Length, actualLines.Length);


        }
    }
}
