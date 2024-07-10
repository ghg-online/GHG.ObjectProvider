using System;
using System.Linq;

namespace GHG.ObjectProvider
{
    public class ClassImplementWrapper<TObjectId> : IClassImplement
    {
        public IClassImplement Implement { get; }

        public ClassImplementWrapper(IClassImplement implement)
        {
            Implement = implement;
        }

        private MethodDefinition[] methodsCache = null;

        public void EnsureMethodListLoaded()
        {
            if (methodsCache != null)
                return;
            var methods = Implement.Methods ?? throw new NullMethodListException(Implement);
            methodsCache = new MethodDefinition[methods.Length];
            Array.Copy(Implement.Methods, methodsCache, methodsCache.Length);
        }

        public MethodDefinition[] Methods
        {
            get
            {
                EnsureMethodListLoaded();
                return methodsCache;
            }
        }

        public object Invoke(object objId, string methodName, params object[] args)
        {
            EnsureMethodListLoaded();
            if (typeof(TObjectId).IsAssignableFrom(objId.GetType()) == false)
                throw new ObjectIdTypeMismatchException(Implement, objId, typeof(TObjectId), objId.GetType());
            TObjectId castedId = (TObjectId)objId;
            return Invoke(castedId, methodName, args);
        }

        public object Invoke(TObjectId objId, string methodName, params object[] args)
        {
            var argTypes = args.Select(a => a.GetType()).ToList();
            var method = Methods.FirstOrDefault(m =>
            {
                if (m.Name != methodName)
                    return false;
                if (m.ArgumentTypes.Length != argTypes.Count)
                    return false;
                for (int i = 0; i < m.ArgumentTypes.Length; i++)
                    if (m.ArgumentTypes[i].IsAssignableFrom(argTypes[i]) == false)
                        return false;
                return true;
            });
            if (method == null)
                throw new MethodNotFoundException(Implement, methodName, argTypes.ToArray());
            var result = Implement.Invoke(objId, methodName, args);
            if (method.ReturnType.IsAssignableFrom(result.GetType()) == false)
                throw new ReturnTypeMismatchException(Implement, method, result.GetType());
            return result;
        }
    }
}
