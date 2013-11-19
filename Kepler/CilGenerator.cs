using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Andrei15193.Kepler.AbstractCore;
using Andrei15193.Kepler.Language.Lexis;

namespace Andrei15193.Kepler
{
    internal class CilGenerator
        : ICilGenerator<Lexicon>
    {
        static private readonly Type _baseType = typeof(object);

        static private void _DefineSingletonPart(TypeBuilder predicateTypeBuilder)
        {
            FieldBuilder instanceFieldBuilder = predicateTypeBuilder.DefineField("_instance",
                                                                                 predicateTypeBuilder,
                                                                                 FieldAttributes.Static | FieldAttributes.Private);

            MethodBuilder getInstanceMethodBuilder = predicateTypeBuilder.DefineMethod("get_Instance",
                                                                                       MethodAttributes.Static | MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName,
                                                                                       CallingConventions.Standard,
                                                                                       predicateTypeBuilder,
                                                                                       Type.EmptyTypes);
            ILGenerator getInstanceIlGenerator = getInstanceMethodBuilder.GetILGenerator();

            getInstanceIlGenerator.Emit(OpCodes.Ldsfld, instanceFieldBuilder);
            getInstanceIlGenerator.Emit(OpCodes.Ret);

            predicateTypeBuilder.DefineProperty("Instance",
                                                PropertyAttributes.None,
                                                CallingConventions.Standard,
                                                predicateTypeBuilder,
                                                Type.EmptyTypes)
                                .SetGetMethod(getInstanceMethodBuilder);

            ConstructorBuilder defaultConstructorBuilder = predicateTypeBuilder.DefineDefaultConstructor(MethodAttributes.Private);

            ILGenerator staticConstructorIlGenerator = predicateTypeBuilder.DefineConstructor(MethodAttributes.Static | MethodAttributes.Public,
                                                                                              CallingConventions.Standard,
                                                                                              Type.EmptyTypes).GetILGenerator();

            staticConstructorIlGenerator.Emit(OpCodes.Newobj, defaultConstructorBuilder);
            staticConstructorIlGenerator.Emit(OpCodes.Stsfld, instanceFieldBuilder);
            staticConstructorIlGenerator.Emit(OpCodes.Ret);
        }

        static private void _DefineInvokeMethod(TypeBuilder predicateTypeBuilder, Type[] parameterTypes, IReadOnlyList<MethodInfo> predicateDefinitionImplementations)
        {
            MethodBuilder invokeMethodBuilder = predicateTypeBuilder.DefineMethod("Invoke",
                                                                                  MethodAttributes.Public | MethodAttributes.Final,
                                                                                  CallingConventions.HasThis,
                                                                                  typeof(bool),
                                                                                  parameterTypes);

            ILGenerator invokeIlGenerator = invokeMethodBuilder.GetILGenerator();
            invokeIlGenerator.DeclareLocal(typeof(bool));
            invokeIlGenerator.Emit(OpCodes.Ldarg_0);
            invokeIlGenerator.EmitCall(OpCodes.Call, _baseType.GetMethod("ToString", BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, Type.EmptyTypes, new ParameterModifier[0]), Type.EmptyTypes);
            invokeIlGenerator.EmitCall(OpCodes.Call, typeof(Console).GetMethod("WriteLine", BindingFlags.Public | BindingFlags.Static, Type.DefaultBinder, new[] { typeof(string) }, new ParameterModifier[0]), Type.EmptyTypes);
            for (short parameterTypeIndex = 0; parameterTypeIndex < parameterTypes.Length; parameterTypeIndex++)
            {
                if (parameterTypes[parameterTypeIndex].IsValueType)
                    invokeIlGenerator.Emit(OpCodes.Ldarga, parameterTypeIndex + 1);
                else
                    invokeIlGenerator.Emit(OpCodes.Ldarg, parameterTypeIndex);
                invokeIlGenerator.EmitCall(OpCodes.Call, parameterTypes[parameterTypeIndex].GetMethod("ToString", BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, Type.EmptyTypes, new ParameterModifier[0]), Type.EmptyTypes);
                invokeIlGenerator.EmitCall(OpCodes.Call, typeof(Console).GetMethod("WriteLine", BindingFlags.Public | BindingFlags.Static, Type.DefaultBinder, new[] { typeof(string) }, new ParameterModifier[0]), Type.EmptyTypes);
            }
            invokeIlGenerator.Emit(OpCodes.Nop);
            invokeIlGenerator.Emit(OpCodes.Ldc_I4_1);
            invokeIlGenerator.Emit(OpCodes.Stloc_0);
            invokeIlGenerator.Emit(OpCodes.Ldloc_0);
            invokeIlGenerator.Emit(OpCodes.Ret);
        }

        static private IReadOnlyList<MethodInfo> _DefinePredicates(TypeBuilder predicateTypeBuilder, IGrouping<Type[], ParsedNode<Lexicon>> predicateDefinitions)
        {
            return new MethodInfo[0];
        }

        public void Generate(string assemblyName, string fileName, ParsedNode<Lexicon> root, CilGeneratorSettings settings)
        {
            if (assemblyName != null)
                if (fileName != null)
                    if (root != null)
                    {
                        IReadOnlyList<ParsedNode<Lexicon>> allPredicateDefinitions;
                        AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.Save);
                        ModuleBuilder moduleBuider = assemblyBuilder.DefineDynamicModule(assemblyName, fileName);

                        if (root.TryGetChildNodeGroup("predicateDefinition", out allPredicateDefinitions))
                        {
                            IDictionary<string, TypeBuilder> predicateTypeBuilders = new SortedDictionary<string, TypeBuilder>();

                            foreach (IGrouping<string, ParsedNode<Lexicon>> overloadedPredicateDefinitions in allPredicateDefinitions.GroupBy(predicateDefinitionsNode => predicateDefinitionsNode["name", 0].Atoms[0].Value))
                            {
                                TypeBuilder predicateTypeBuilder = moduleBuider.DefineType(overloadedPredicateDefinitions.Key,
                                                                                           TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Class,
                                                                                           _baseType);

                                predicateTypeBuilders.Add(predicateTypeBuilder.Name, predicateTypeBuilder);
                                _DefineSingletonPart(predicateTypeBuilder);
                                foreach (IGrouping<Type[], ParsedNode<Lexicon>> predicateDefinition in overloadedPredicateDefinitions.GroupBy(overloadedPredicateDefinition =>
                                    {
                                        IReadOnlyList<ParsedNode<Lexicon>> parameters;

                                        if (overloadedPredicateDefinition.TryGetChildNodeGroup("variableDeclaration", out parameters))
                                            return parameters.Select(parameter => typeof(object).Assembly.GetType(string.Join(".", parameter["type", 0]["qualifiedIdentifier", 0]["name"].Select(parameterTypeName => parameterTypeName.Atoms[0].Value)))).ToArray();
                                        else
                                            return Type.EmptyTypes;
                                    }))
                                    _DefineInvokeMethod(predicateTypeBuilder, predicateDefinition.Key, _DefinePredicates(predicateTypeBuilder, predicateDefinition));
                            }

                            foreach (TypeBuilder predicateTypeBuilder in predicateTypeBuilders.Values)
                                predicateTypeBuilder.CreateType();
                        }

                        assemblyBuilder.Save(fileName);
                    }
                    else
                        throw new ArgumentNullException("root");
                else
                    throw new ArgumentNullException("fileName");
            else
                throw new ArgumentNullException("assemblyName");
        }
    }
}
