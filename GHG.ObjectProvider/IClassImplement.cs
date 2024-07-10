using System;

namespace GHG.ObjectProvider
{
    public class MethodDefinition
    {
        public string Name { get; set; }
        public Type ReturnType { get; set; }
        public Type[] ArgumentTypes { get; set; } // this pointer is not included
    }

    public interface IClassImplement
    {
        MethodDefinition[] Methods { get; }
        object Invoke(object objId, string methodName, params object[] args);
    }
}
