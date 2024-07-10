using System;
using System.Linq;

namespace GHG.ObjectProvider
{
    public class ObjectProviderException : System.Exception
    {
        public ObjectProviderException(string message) : base(message) { }
    }

    public class NullMethodListException : ObjectProviderException
    {
        public IClassImplement ClassImplement { get; }
        public NullMethodListException(IClassImplement classImplement) : base("Method list is null")
        {
            ClassImplement = classImplement;
        }
    }

    public class ObjectIdTypeMismatchException : ObjectProviderException
    {
        public IClassImplement ClassImplement { get; }
        public object ObjectId { get; }
        public Type ObjectIdType { get; }
        public Type ExpectedType { get; }
        public ObjectIdTypeMismatchException(IClassImplement classImplement, object objectId, Type objectIdType, Type expectedType)
            : base($"Object id type mismatch: {objectIdType} is not assignable to {expectedType}")
        {
            ClassImplement = classImplement;
            ObjectId = objectId;
            ObjectIdType = objectIdType;
            ExpectedType = expectedType;
        }
    }

    public class MethodNotFoundException : ObjectProviderException
    {
        public IClassImplement ClassImplement { get; }
        public string MethodName { get; }
        public Type[] ArgumentTypes { get; }
        public MethodNotFoundException(IClassImplement classImplement, string methodName, Type[] argumentTypes)
            : base($"Method not found: {methodName}({string.Join(", ", argumentTypes.Select(t => t.ToString()))})")
        {
            ClassImplement = classImplement;
            MethodName = methodName;
            ArgumentTypes = argumentTypes;
        }
    }

    public class ReturnTypeMismatchException : ObjectProviderException
    {
        public IClassImplement ClassImplement { get; }
        public MethodDefinition Method { get; }
        public Type ActualType { get; }
        public ReturnTypeMismatchException(IClassImplement classImplement, MethodDefinition method, Type actualType)
            : base($"Return type mismatch: {actualType} is not assignable to {method.ReturnType}")
        {
            ClassImplement = classImplement;
            Method = method;
            ActualType = actualType;
        }
    }
}
