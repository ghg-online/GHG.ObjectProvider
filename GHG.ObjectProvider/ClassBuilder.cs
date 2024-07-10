using System;
using System.Reflection;
using System.Reflection.Emit;

namespace GHG.ObjectProvider
{
    public class ClassBuilder
    {
        private readonly string className;
        private readonly Type[] interfaces;
        private readonly IClassImplement implement;
        private readonly ModuleBuilder moduleBuilder;

        public ClassBuilder(
            string className,
            Type[] interfaces,
            IClassImplement implement,
            ModuleBuilder moduleBuilder
        )
        {
            this.className = className;
            this.interfaces = interfaces;
            this.implement = implement;
            this.moduleBuilder = moduleBuilder;
        }

        public Type Build()
        {
            var typeBuilder = moduleBuilder.DefineType(
                className,
                TypeAttributes.Public | TypeAttributes.Class,
                typeof(object),
                interfaces
            );
            var implementFb = typeBuilder.DefineField(
                "~implement",
                typeof(object),
                FieldAttributes.Private | FieldAttributes.InitOnly
            );
            var thisObjFb = typeBuilder.DefineField(
                "~thisObj",
                typeof(object),
                FieldAttributes.Private | FieldAttributes.InitOnly
            );
            var ctorBuilder = typeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.Standard,
                new[] { typeof(object), typeof(object) }
            );
            var il = ctorBuilder.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stfld, thisObjFb);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Stfld, implementFb);
            il.Emit(OpCodes.Ret);
            var methods = implement.Methods;
            foreach (var method in methods)
            {
                var methodBuilder = typeBuilder.DefineMethod(
                    method.Name,
                    MethodAttributes.Public | MethodAttributes.Virtual,
                    method.ReturnType,
                    method.ArgumentTypes
                );

                var methodIL = methodBuilder.GetILGenerator();

                // Load ~thisObj field onto stack
                methodIL.Emit(OpCodes.Ldarg_0);
                methodIL.Emit(OpCodes.Ldfld, thisObjFb);

                // Load ~implement field onto stack
                methodIL.Emit(OpCodes.Ldarg_0);
                methodIL.Emit(OpCodes.Ldfld, implementFb);


                // Load method name onto stack
                methodIL.Emit(OpCodes.Ldstr, method.Name);

                // Create an array to hold arguments
                methodIL.Emit(OpCodes.Ldc_I4, method.ArgumentTypes.Length);
                methodIL.Emit(OpCodes.Newarr, typeof(object));

                // Store arguments in the array
                for (var i = 0; i < method.ArgumentTypes.Length; i++)
                {
                    methodIL.Emit(OpCodes.Dup); // Duplicate the array reference
                    methodIL.Emit(OpCodes.Ldc_I4, i); // Load the index onto the stack
                    methodIL.Emit(OpCodes.Ldarg, i + 1); // Load the argument onto the stack
                    if (method.ArgumentTypes[i].IsValueType)
                    {
                        methodIL.Emit(OpCodes.Box, method.ArgumentTypes[i]); // Box value types
                    }
                    methodIL.Emit(OpCodes.Stelem_Ref); // Store the argument in the array
                }

                // Call IClassImplement.Invoke
                methodIL.Emit(OpCodes.Callvirt, typeof(IClassImplement).GetMethod("Invoke"));

                // Unbox the result if the method returns a value type
                if (method.ReturnType.IsValueType)
                {
                    methodIL.Emit(OpCodes.Unbox_Any, method.ReturnType);
                }

                // Return
                methodIL.Emit(OpCodes.Ret);

            }
            return typeBuilder.CreateType();
        }
    }
}
