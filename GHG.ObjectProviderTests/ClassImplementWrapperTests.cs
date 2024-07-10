using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.Reflection.Emit;

namespace GHG.ObjectProvider.Tests
{
    [TestClass()]
    public class ClassImplementWrapperTests
    {
        [TestMethod()]
        public void ClassImplementWrapperTest()
        {
            var implement = new ClassImplementWrapper<Adder>(new LocalImplement<Adder>());
            var moduleBuilder = AssemblyBuilder.DefineDynamicAssembly(
                new AssemblyName("TestAssembly"),
                AssemblyBuilderAccess.RunAndCollect
            ).DefineDynamicModule("TestModule");
            var builder = new ClassBuilder("Adder", new[] { typeof(IAdder) }, implement, moduleBuilder);
            Type type = builder.Build();
            Adder underlyingInstance = new Adder();
            var ctor = type.GetConstructor(new[] { typeof(object), typeof(object) });
            var instance = (IAdder)ctor!.Invoke(new object[] { implement, underlyingInstance });
            Assert.AreEqual("a", instance.ApplySuffix("a"));
            Assert.AreEqual("ab", instance.CreateSuffix("b").ApplySuffix("a"));
            Assert.AreEqual("abc", instance.Compose(instance.CreateSuffix("b"), instance.CreateSuffix("c")).ApplySuffix("a"));
        }
    }
}