using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Andrei15193.Kepler.AbstractCore;
using Andrei15193.Kepler.Language.Lexis;
using Andrei15193.Kepler.Language.Syntax;
using Andrei15193.Kepler.Relay;

namespace Andrei15193.Kepler
{
    // refactor CilGenerator (naming)
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

        static private void _DefineInvokeMethod(TypeBuilder predicateTypeBuilder, IReadOnlyList<TypeNode> parameterTypes, IReadOnlyList<MethodInfo> predicateDefinitionImplementations)
        {
            Type[] parameterCliTypes = _GetCliTypes(parameterTypes);
            MethodBuilder invokeMethodBuilder = predicateTypeBuilder.DefineMethod("Invoke",
                                                                                  MethodAttributes.Public | MethodAttributes.Final,
                                                                                  CallingConventions.HasThis,
                                                                                  typeof(bool),
                                                                                  parameterCliTypes);
            ILGenerator invokeIlGenerator = invokeMethodBuilder.GetILGenerator();

            invokeIlGenerator.DeclareLocal(typeof(bool));
            invokeIlGenerator.Emit(OpCodes.Ldarg_0);
            invokeIlGenerator.EmitCall(OpCodes.Call, _baseType.GetMethod("ToString", BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, Type.EmptyTypes, new ParameterModifier[0]), Type.EmptyTypes);
            invokeIlGenerator.EmitCall(OpCodes.Call, typeof(Console).GetMethod("WriteLine", BindingFlags.Public | BindingFlags.Static, Type.DefaultBinder, new[] { typeof(string) }, new ParameterModifier[0]), Type.EmptyTypes);
            for (short parameterCliTypeIndex = 0; parameterCliTypeIndex < parameterCliTypes.Length; parameterCliTypeIndex++)
            {
                if (parameterCliTypes[parameterCliTypeIndex].IsValueType)
                    invokeIlGenerator.Emit(OpCodes.Ldarga, parameterCliTypeIndex + 1);
                else
                    invokeIlGenerator.Emit(OpCodes.Ldarg, parameterCliTypeIndex);
                invokeIlGenerator.EmitCall(OpCodes.Call, parameterCliTypes[parameterCliTypeIndex].GetMethod("ToString", BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, Type.EmptyTypes, new ParameterModifier[0]), Type.EmptyTypes);
                invokeIlGenerator.EmitCall(OpCodes.Call, typeof(Console).GetMethod("WriteLine", BindingFlags.Public | BindingFlags.Static, Type.DefaultBinder, new[] { typeof(string) }, new ParameterModifier[0]), Type.EmptyTypes);
            }
            invokeIlGenerator.Emit(OpCodes.Nop);
            invokeIlGenerator.Emit(OpCodes.Ldc_I4_1);
            invokeIlGenerator.Emit(OpCodes.Stloc_0);
            invokeIlGenerator.Emit(OpCodes.Ldloc_0);
            invokeIlGenerator.Emit(OpCodes.Ret);
        }

        private static Type[] _GetCliTypes(IReadOnlyList<TypeNode> parameterTypeNodes)
        {
            IList<Type> cliTypes = new List<Type>();

            foreach (TypeNode parameterTypeNode in parameterTypeNodes)
            {
                Type cliType = typeof(object).Assembly.GetType(parameterTypeNode.ToCliTypeName());

                foreach (int arrayDimension in parameterTypeNode.ArrayDimensions)
                    cliType = cliType.MakeArrayType(arrayDimension);

                cliTypes.Add(cliType);
            }

            return cliTypes.ToArray();
        }

        static private IReadOnlyList<MethodInfo> _DefinePredicates(TypeBuilder predicateTypeBuilder, KeyValuePair<PredicateDeclarationNode, IReadOnlyList<ParsedNode<Lexicon>>> predicateDeclaration)
        {
            return new MethodInfo[0];
        }
        
        public void Generate(string assemblyName, string fileName, ParsedNode<Lexicon> root, CilGeneratorSettings settings)
        {
            if (assemblyName != null)
                if (fileName != null)
                    if (root != null)
                    {
                        AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.Save);
                        ModuleBuilder moduleBuider = assemblyBuilder.DefineDynamicModule(assemblyName, fileName);
                        IDictionary<string, TypeBuilder> predicateTypeBuilders = new SortedDictionary<string, TypeBuilder>();
                        IReadOnlyDictionary<PredicateDeclarationNode, IReadOnlyList<ParsedNode<Lexicon>>> predicateDeclarations = _GetPredicateDeclarations(root);

                        foreach (var predicateDeclarationOverloads in predicateDeclarations.GroupBy(predicateDeclaration => predicateDeclaration.Key.Name))
                        {
                            TypeBuilder predicateTypeBuilder = moduleBuider.DefineType(predicateDeclarationOverloads.Key,
                                                                                       TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Class,
                                                                                       _baseType);

                            predicateTypeBuilders.Add(predicateTypeBuilder.Name, predicateTypeBuilder);
                            _DefineSingletonPart(predicateTypeBuilder);
                            foreach (var predicateDeclarationOverload in predicateDeclarationOverloads)
                                _DefineInvokeMethod(predicateTypeBuilder, predicateDeclarationOverload.Key.ParameterTypes.ToArray(), _DefinePredicates(predicateTypeBuilder, predicateDeclarationOverload));
                        }

                        foreach (TypeBuilder predicateTypeBuilder in predicateTypeBuilders.Values)
                            predicateTypeBuilder.CreateType();

                        assemblyBuilder.Save(fileName);
                    }
                    else
                        throw new ArgumentNullException("root");
                else
                    throw new ArgumentNullException("fileName");
            else
                throw new ArgumentNullException("assemblyName");
        }

        private IReadOnlyDictionary<PredicateDeclarationNode, IReadOnlyList<ParsedNode<Lexicon>>> _GetPredicateDeclarations(ParsedNode<Lexicon> root)
        {
            if (root != null)
                if (root.Name == KeplerRuleSet.Program)
                {
                    IReadOnlyList<ParsedNode<Lexicon>> predicateDeclarationParsedNodes;
                    var predicateDeclarations = new Dictionary<PredicateDeclarationNode, List<ParsedNode<Lexicon>>>(RelayComparer.Create((x, y) => x.FullName.CompareTo(y.FullName),
                                                                                                                                        (x, y) => x.FullName == y.FullName,
                                                                                                                                        (PredicateDeclarationNode predicateDeclaration) => predicateDeclaration.FullName.GetHashCode()));

                    if (root.TryGetChildNodeGroup(KeplerRuleSet.PredicateDefinition, out predicateDeclarationParsedNodes))
                        foreach (ParsedNode<Lexicon> predicateDefinitionParsedNode in predicateDeclarationParsedNodes)
                        {
                            List<ParsedNode<Lexicon>> overloads;
                            PredicateDeclarationNode predicateDefinitionNode = new PredicateDeclarationNode(predicateDefinitionParsedNode);

                            if (predicateDeclarations.TryGetValue(predicateDefinitionNode, out overloads))
                                overloads.Add(predicateDefinitionParsedNode);
                            else
                                predicateDeclarations.Add(predicateDefinitionNode, new List<ParsedNode<Lexicon>> { predicateDefinitionParsedNode });
                        }
                    //foreach (ParsedNode<Lexicon> factDefinition in root[KeplerRuleSet.FactDefinition])
                    //{
                    //    List<ParsedNode<Lexicon>> overloads;
                    //    PredicateDefinitionNode predicateDefinitionNode = new PredicateDefinitionNode(factDefinition);

                    //    if (predicateDeclarations.TryGetValue(predicateDefinitionNode, out overloads))
                    //        overloads.Add(factDefinition);
                    //    else
                    //        predicateDeclarations.Add(predicateDefinitionNode, new List<ParsedNode<Lexicon>> { factDefinition });
                    //}

                    return new ReadOnlyDictionary<PredicateDeclarationNode, IReadOnlyList<ParsedNode<Lexicon>>>(predicateDeclarations.ToDictionary(pair => pair.Key, pair => (IReadOnlyList<ParsedNode<Lexicon>>)pair.Value));
                }
                else
                    throw new ArgumentException("Must be program node!", "root");
            else
                throw new ArgumentNullException("root");
        }
    }
}
