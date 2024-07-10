using System;
using System.Linq;
using System.Reflection;

namespace GHG.ObjectProvider
{
    public class LocalImplement<TImplement> : IClassImplement
        where TImplement : class
    {
        private readonly Type implementType;

        public LocalImplement()
        {
            implementType = typeof(TImplement);
            if (implementType.GetCustomAttribute<GhgClassAttribute>() == null)
                throw new Exception(string.Format("{0} is not a ghg class", implementType.FullName));
        }

        public MethodDefinition[] Methods =>
            implementType.GetMethods()
                .Where(m => m.GetCustomAttribute<GhgMethodAttribute>() != null)
                .Select(m => new MethodDefinition
                {
                    Name = m.Name,
                    ReturnType = m.ReturnType,
                    ArgumentTypes = m.GetParameters().Select(p => p.ParameterType).ToArray()
                })
                .ToArray();

        public object Invoke(object objId, string methodName, params object[] args)
        {
            var argTypes = args.Select(a => a.GetType()).ToArray();
            var method = implementType.GetMethod(methodName, argTypes);
            if (method == null || method.GetCustomAttribute<GhgMethodAttribute>() == null)
                throw new Exception(string.Format("Ghg method {0} is not found", methodName));
            return method.Invoke(objId, args);
        }
    }
}
