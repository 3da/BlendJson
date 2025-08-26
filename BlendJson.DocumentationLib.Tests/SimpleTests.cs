using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace BlendJson.DocumentationLib.Tests
{
    [TestClass]
    public class SimpleTests
    {
        private IList<MemberInfo> TestDocumentationForType(object obj, MemberInfo expected)
        {
            var manager = new DocumentationManager();

            var documentation = manager.GenerateForTypes(obj.GetType());


            Assert.AreEqual(JsonConvert.SerializeObject(expected, Formatting.Indented), JsonConvert.SerializeObject(documentation[0], Formatting.Indented));

            return documentation;
        }

        public class ClassWithEnum
        {
            public MyEnum EnumProperty { get; set; }
        }

        [TestMethod]
        public void TestEnum()
        {


            TestDocumentationForType(new ClassWithEnum { EnumProperty = MyEnum.Hello }, new MemberInfo()
            {
                MemberType = MemberType.Class,
                Name = nameof(ClassWithEnum),
                Type = nameof(ClassWithEnum),
                Children = new List<MemberInfo>()
                {
                   new MemberInfo()
                   {
                       MemberType = MemberType.Enum,
                       Name = "EnumProperty",
                       Type = nameof(MyEnum),
                       Implementations = new List<MemberInfo>()
                       {
                           new MemberInfo()
                           {
                               MemberType = MemberType.Primitive,
                               Name = "Hello",
                               Type = "int",
                               Value = "0",
                           },
                           new MemberInfo()
                           {
                               MemberType = MemberType.Primitive,
                               Name = "World",
                               Type = "int",
                               Value = "1",
                           },
                           new MemberInfo()
                           {
                               MemberType = MemberType.Primitive,
                               Name = "ZXCVB",
                               Type = "int",
                               Value = "2",
                           }
                       }
                   }
                }
            });
        }

        [TestMethod]
        public void TestArray()
        {
            TestDocumentationForType(new byte[0], new MemberInfo()
            {
                Name = "Byte[]",
                Type = "byte[]"
            });
        }

        [TestMethod]
        public void TestSubSettings()
        {
            TestDocumentationForType(new SubSettings(), new MemberInfo()
            {
                MemberType = MemberType.Class,
                Name = "SubSettings",
                Type = "SubSettings",
                Children = new List<MemberInfo>()
                {
                    new MemberInfo()
                    {
                        MemberType = MemberType.Enum,
                        Name = "MyEnum",
                        Type = "MyEnum",
                        Implementations = new List<MemberInfo>()
                        {
                            new MemberInfo()
                            {
                                MemberType = MemberType.Primitive,
                                Name = "Hello",
                                Type = "int",
                                Value = "0",
                            },
                            new MemberInfo()
                            {
                                MemberType = MemberType.Primitive,
                                Name = "World",
                                Type = "int",
                                Value = "1",
                            },
                            new MemberInfo()
                            {
                                MemberType = MemberType.Primitive,
                                Name = "ZXCVB",
                                Type = "int",
                                Value = "2",
                            }
                        }
                    },
                    new MemberInfo()
                    {
                        MemberType = MemberType.Array,
                        Name = "StrArray",
                        Type = "string[]",
                        Children = new List<MemberInfo>()
                        {
                             new MemberInfo()
                             {
                                 Name = "Item",
                                 Type = "string"
                             }
                        }
                    }
                }
            });
        }


        [TestMethod]
        public void TestMethod1()
        {
            var expected = new MemberInfo()
            {
                MemberType = MemberType.Class,
                Name = "Settings",
                Type = "Settings",
                Children = new List<MemberInfo>()
                {
                    new MemberInfo()
                    {
                        MemberType = MemberType.Class,
                        Name = "SubSettings",
                        Type = "SubSettings",
                        Children = new List<MemberInfo>()
                        {
                            new MemberInfo()
                            {
                                MemberType = MemberType.Enum,
                                Name = "MyEnum",
                                Type = "MyEnum",
                                Implementations = new List<MemberInfo>()
                                {
                                    new MemberInfo()
                                    {
                                        MemberType = MemberType.Primitive,
                                        Name = "Hello",
                                        Type = "int",
                                        Value = "0",
                                    },
                                    new MemberInfo()
                                    {
                                        MemberType = MemberType.Primitive,
                                        Name = "World",
                                        Type = "int",
                                        Value = "1",
                                    },
                                    new MemberInfo()
                                    {
                                        MemberType = MemberType.Primitive,
                                        Name = "ZXCVB",
                                        Type = "int",
                                        Value = "2",
                                    }
                                }
                            },
                            new MemberInfo()
                            {
                                MemberType = MemberType.Array,
                                Name = "StrArray",
                                Type = "string[]",
                                Children = new List<MemberInfo>()
                                {
                                    new MemberInfo()
                                    {
                                        MemberType = MemberType.Primitive,
                                        Name = "Item",
                                        Type = "string"
                                    }
                                }
                            }
                        }
                    },
                    new MemberInfo()
                    {
                        MemberType = MemberType.Primitive,
                        Name = "Number",
                        Type = "decimal",
                    },
                    new MemberInfo()
                    {
                        MemberType = MemberType.Primitive,
                        Name = "Str",
                        Type = "string"
                    },
                    new MemberInfo()
                    {
                        MemberType = MemberType.Array,
                        Name = "MyEnums",
                        Type = "MyEnum[]",
                        Children = new List<MemberInfo>()
                        {
                            new MemberInfo()
                            {
                                MemberType = MemberType.Enum,
                                Name = "Item",
                                Type = "MyEnum",
                                Implementations = new List<MemberInfo>()
                                {
                                    new MemberInfo()
                                    {
                                        Name = "Hello",
                                        Type = "int",
                                        Value = "0",
                                    },
                                    new MemberInfo()
                                    {
                                        Name = "World",
                                        Type = "int",
                                        Value = "1",
                                    },
                                    new MemberInfo()
                                    {
                                        Name = "ZXCVB",
                                        Type = "int",
                                        Value = "2",
                                    }
                                }
                            }
                        }
                    }
                }
            };

            TestDocumentationForType(new Settings(), expected);
        }
    }
}
