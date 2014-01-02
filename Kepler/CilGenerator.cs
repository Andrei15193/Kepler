//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Dynamic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Reflection;
//using System.Reflection.Emit;
//using System.Runtime.CompilerServices;
//using System.Text;
//using Andrei15193.Kepler.Language.Lexic;
//using Andrei15193.Kepler.Language.Syntax;
//using Andrei15193.Kepler.Language.Syntax.Parser;

//namespace Andrei15193.Kepler
//{
//	// refactor CilGenerator (naming)
//	internal class CilGenerator
//	{
//		static private readonly Type _baseType = typeof(object);
//		static private readonly IDictionary<OperationName, int> _operations = new SortedDictionary<OperationName, int>();

//		static CilGenerator()
//		{
//			_operations.Add(OperationName.Disjunction, 1);
//			_operations.Add(OperationName.Conjuction, 2);
//			_operations.Add(OperationName.Negation, 3);
//			_operations.Add(OperationName.LessThanComparison, 4);
//			_operations.Add(OperationName.LessThanOrEqualToComparison, 4);
//			_operations.Add(OperationName.EqualComparison, 4);
//			_operations.Add(OperationName.GreaterThanOrEqualToComparison, 4);
//			_operations.Add(OperationName.GreaterThanComparison, 4);
//			_operations.Add(OperationName.Addition, 5);
//			_operations.Add(OperationName.Subtraction, 5);
//			_operations.Add(OperationName.Multiplication, 6);
//			_operations.Add(OperationName.Division, 6);
//			_operations.Add(OperationName.IntegerDivision, 6);
//			_operations.Add(OperationName.Modulo, 6);
//			_operations.Add(OperationName.IntegerPromotion, 7);
//			_operations.Add(OperationName.AdditiveInverse, 7);
//		}

//		private class PredicateBuilder
//		{
//			public PredicateBuilder(TypeBuilder typeBuilder, PropertyInfo instanceProperty)
//			{
//				if (typeBuilder != null)
//					if (instanceProperty != null)
//					{
//						_typeBuilder = typeBuilder;
//						_instanceProperty = instanceProperty;
//					}
//					else
//						throw new ArgumentNullException("instanceProperty");
//				else
//					throw new ArgumentNullException("typeBuilder");
//			}

//			public MethodBuilder GetInvokeMethod(Type[] parameterTypes, bool defineIfMissing = false)
//			{
//				//if (parameterTypes != null)
//				//	if (defineIfMissing)
//				//	{
//				//		MethodBuilder invokeMethodBuilder;

//				//		if (!_invokeMethods.TryGetValue(parameterTypes, out invokeMethodBuilder))
//				//		{
//				//			invokeMethodBuilder = _typeBuilder.DefineMethod("_Invoke",
//				//															MethodAttributes.Private | MethodAttributes.Static,
//				//															CallingConventions.Standard,
//				//															typeof(bool),
//				//															parameterTypes);

//				//			ILGenerator instanceInvokeMethodIlGenerator = _typeBuilder.DefineMethod("Invoke",
//				//																					MethodAttributes.Public,
//				//																					CallingConventions.HasThis,
//				//																					typeof(bool),
//				//																					parameterTypes).GetILGenerator();
//				//			instanceInvokeMethodIlGenerator.Emit(OpCodes.Ret);

//				//			for (int parameterIndex = 0; parameterIndex < parameterTypes.Length; parameterIndex++)
//				//				instanceInvokeMethodIlGenerator.Emit(OpCodes.Ldarg, parameterIndex);
//				//			instanceInvokeMethodIlGenerator.EmitCall(OpCodes.Call, invokeMethodBuilder, parameterTypes);
//				//			instanceInvokeMethodIlGenerator.Emit(OpCodes.Ret);

//				//			_invokeMethods.Add(parameterTypes, invokeMethodBuilder);
//				//		}

//				//		return invokeMethodBuilder;
//				//	}
//				//	else
//				//		return _invokeMethods[parameterTypes];
//				//else
//				//	throw new ArgumentNullException("parameterTypes");
//				return null;
//			}

//			public TypeBuilder TypeBuilder
//			{
//				get
//				{
//					return _typeBuilder;
//				}
//			}

//			public PropertyInfo InstanceProperty
//			{
//				get
//				{
//					return _instanceProperty;
//				}
//			}

//			private readonly TypeBuilder _typeBuilder;
//			private readonly PropertyInfo _instanceProperty;
//			private readonly IDictionary<Type[], MethodBuilder> _invokeMethods = new Dictionary<Type[], MethodBuilder>(DelegateComparer.Create((x, y) => string.Join(", ", x.Select(type => type.FullName)).CompareTo(string.Join(", ", y.Select(type => type.FullName))),
//																																			(x, y) => x.SequenceEqual(y),
//																																			(Type[] x) => x.Aggregate(0, (currentHash, type) => currentHash ^ type.GetHashCode())));
//		}

//		public void Generate(string assemblyName, ProgramNode programNode)
//		{
//			if (assemblyName != null)
//				if (programNode != null)
//				{
//					AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.Save);
//					IReadOnlyList<PredicateDeclarationNode> predicateDeclarations = programNode.PredicateDeclarations;
//					IDictionary<string, PredicateBuilder> predicateBuilders = _CreatePredicateBuilders(assemblyBuilder.DefineDynamicModule(assemblyName, assemblyName + ".exe"),
//																									   predicateDeclarations);

//					foreach (IGrouping<string, PredicateDeclarationNode> predicateDeclarationsWithSameName in predicateDeclarations.GroupBy(predicateDeclaration => predicateDeclaration.Name))
//					{
//						PredicateBuilder predicateBuilder = predicateBuilders[predicateDeclarationsWithSameName.Key];

//						foreach (IGrouping<Type[], PredicateDeclarationNode> predicateOverloads in predicateDeclarationsWithSameName.GroupBy(predicateDeclaration => _GetCliTypes(predicateDeclaration.Parameters.Select(parameter => parameter.Type)),
//																																			 DelegateEqualityComparer.Create<Type[]>((x, y) => x.SequenceEqual(y),
//																																												  (x => x.Aggregate(0, (hashCode, type) => hashCode ^ type.GetHashCode())))))
//							_WriteInvokeMethod(predicateBuilders,
//											   predicateBuilder.GetInvokeMethod(predicateOverloads.Key,
//																				true),
//											   _DefinePredicates(predicateBuilders,
//																 predicateBuilder.TypeBuilder,
//																 predicateOverloads.OrderBy(predicateDeclaratinNode => predicateDeclaratinNode.IsPredicate).ToList()));
//					}

//					foreach (PredicateBuilder predicateBuilder in predicateBuilders.Values)
//						predicateBuilder.TypeBuilder.CreateType();

//					if (predicateBuilders.ContainsKey("Main"))
//						assemblyBuilder.SetEntryPoint(predicateBuilders["Main"].TypeBuilder.GetMethod("Invoke"), PEFileKinds.ConsoleApplication);
//					assemblyBuilder.Save(assemblyName + ".exe");
//				}
//				else
//					throw new ArgumentNullException("root");
//			else
//				throw new ArgumentNullException("assemblyName");
//		}

//		private IDictionary<string, PredicateBuilder> _CreatePredicateBuilders(ModuleBuilder moduleBuider, IReadOnlyCollection<PredicateDeclarationNode> predicateDeclarations)
//		{
//			IDictionary<string, PredicateBuilder> predicateBuilders = new SortedDictionary<string, PredicateBuilder>();

//			foreach (IGrouping<string, PredicateDeclarationNode> predicatesWithSameName in predicateDeclarations.GroupBy(predicateDeclaration => predicateDeclaration.Name))
//			{
//				List<PredicateBuilder> predicateBuilderOverloads = new List<PredicateBuilder>();
//				PropertyInfo instanceProperty;
//				TypeBuilder predicateTypeBuilder = moduleBuider.DefineType(predicatesWithSameName.Key,
//																		   TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Class,
//																		   _baseType);

//				_DefineSingletonPart(predicateTypeBuilder, out instanceProperty);
//				predicateBuilders.Add(predicatesWithSameName.Key, new PredicateBuilder(predicateTypeBuilder, instanceProperty));
//			}

//			return predicateBuilders;
//		}

//		private IReadOnlyList<PredicateDeclarationNode> _GetPredicateDeclarations(ParsedNode root)
//		{
//			if (root != null)
//				if (root.Name == KeplerRuleSet.Program)
//				{
//					List<PredicateDeclarationNode> predicateDeclarations = new List<PredicateDeclarationNode>();
//					IReadOnlyList<ParsedNode<AtomCode>> predicateDeclarationParsedNodes;

//					if (root.TryGetChildNodeGroup(KeplerRuleSet.PredicateDefinition, out predicateDeclarationParsedNodes))
//						foreach (ParsedNode<AtomCode> predicateDefinitionParsedNode in predicateDeclarationParsedNodes)
//							predicateDeclarations.Add(new PredicateDeclarationNode(predicateDefinitionParsedNode));

//					if (root.TryGetChildNodeGroup(KeplerRuleSet.FactDefinition, out predicateDeclarationParsedNodes))
//						foreach (ParsedNode<AtomCode> factDefinition in predicateDeclarationParsedNodes)
//							predicateDeclarations.Add(new PredicateDeclarationNode(factDefinition));

//					return predicateDeclarations;
//				}
//				else
//					throw new ArgumentException("Must be program node!", "root");
//			else
//				throw new ArgumentNullException("root");
//		}

//		private void _DefineSingletonPart(TypeBuilder predicateTypeBuilder, out PropertyInfo instanceProperty)
//		{
//			FieldBuilder instanceFieldBuilder = predicateTypeBuilder.DefineField("_instance",
//																				 predicateTypeBuilder,
//																				 FieldAttributes.Static | FieldAttributes.Private);

//			MethodBuilder getInstanceMethodBuilder = predicateTypeBuilder.DefineMethod("get_Instance",
//																					   MethodAttributes.Static | MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName,
//																					   CallingConventions.Standard,
//																					   predicateTypeBuilder,
//																					   Type.EmptyTypes);
//			ILGenerator getInstanceIlGenerator = getInstanceMethodBuilder.GetILGenerator();

//			getInstanceIlGenerator.Emit(OpCodes.Ldsfld, instanceFieldBuilder);
//			getInstanceIlGenerator.Emit(OpCodes.Ret);

//			PropertyBuilder instancePropertyBuilder = predicateTypeBuilder.DefineProperty("Instance",
//																						  PropertyAttributes.None,
//																						  CallingConventions.Standard,
//																						  predicateTypeBuilder,
//																						  Type.EmptyTypes);
//			instancePropertyBuilder.SetGetMethod(getInstanceMethodBuilder);

//			ConstructorBuilder defaultConstructorBuilder = predicateTypeBuilder.DefineDefaultConstructor(MethodAttributes.Private);

//			ILGenerator staticConstructorIlGenerator = predicateTypeBuilder.DefineConstructor(MethodAttributes.Static | MethodAttributes.Public,
//																							  CallingConventions.Standard,
//																							  Type.EmptyTypes).GetILGenerator();

//			staticConstructorIlGenerator.Emit(OpCodes.Newobj, defaultConstructorBuilder);
//			staticConstructorIlGenerator.Emit(OpCodes.Stsfld, instanceFieldBuilder);
//			staticConstructorIlGenerator.Emit(OpCodes.Ret);

//			instanceProperty = instancePropertyBuilder;
//		}

//		private void _WriteInvokeMethod(IDictionary<string, PredicateBuilder> definedPredicates, MethodBuilder invokeMethodBuilder, IReadOnlyList<MethodInfo> predicateDefinitionImplementations)
//		{
//			//ILGenerator ilGenerator = invokeMethodBuilder.GetILGenerator();

//			//ilGenerator.DeclareLocal(typeof(bool?));
//			//foreach (MethodInfo predicateDefinitionImplementation in predicateDefinitionImplementations)
//			//{

//			//}

//			//ilGenerator.Emit(OpCodes.Ret);
//		}

//		private Type _GetCliType(TypeNode typeNode)
//		{
//			return typeof(object).Assembly.GetType(typeNode.GetCliTypeName(), throwOnError: true);
//		}

//		public Type _GetCliType(QualifiedIdentifierNode qualifiedIdentifier)
//		{
//			return typeof(object).Assembly.GetType(qualifiedIdentifier.ToString("."), throwOnError: true);
//		}

//		private Type[] _GetCliTypes(IEnumerable<TypeNode> typeNodes)
//		{
//			return typeNodes.Select(typeNode => _GetCliType(typeNode)).ToArray();
//		}

//		private IReadOnlyList<MethodInfo> _DefinePredicates(IDictionary<string, PredicateBuilder> definedPredicates, TypeBuilder predicateTypeBuilder, IReadOnlyList<PredicateDeclarationNode> predicateDeclarations)
//		{
//			List<MethodInfo> predicateImplementations = new List<MethodInfo>();

//			for (int predicateDeclarationIndex = 0; predicateDeclarationIndex < 1; predicateDeclarationIndex++)
//			{
//				List<ParameterExpression> parameters = predicateDeclarations[predicateDeclarationIndex].Parameters
//																									   .Select(parameter => Expression.Parameter(_GetCliType(parameter.Type), parameter.VariableName))
//																									   .ToList();
//				MethodBuilder methodBuilder = predicateTypeBuilder.DefineMethod("Invoke",
//																				MethodAttributes.Public | MethodAttributes.Static,
//																				CallingConventions.Standard,
//																				typeof(bool?),
//																				parameters.Select(parameter => parameter.Type).ToArray());
//				LabelTarget returnLabel = Expression.Label(typeof(bool?));
//				BlockExpression body = _GetBody(returnLabel,
//												predicateDeclarations[predicateDeclarationIndex].Body,
//												parameters,
//												definedPredicates);
//				if (predicateTypeBuilder.Name == "Main")
//					Expression.Lambda(Expression.Block(typeof(void),
//													   body.Variables,
//													   body.Expressions.Concat(new Expression[] { Expression.Label(returnLabel, _GetNullable(false)) })),
//									  parameters)
//							  .CompileToMethod(methodBuilder);
//				else
//					Expression.Lambda(Expression.Block(typeof(bool?),
//													   body.Variables,
//													   body.Expressions.Concat(new Expression[] { Expression.Label(returnLabel, _GetNullable(false)) })),
//									  parameters)
//							  .CompileToMethod(methodBuilder);

//				predicateImplementations.Add(methodBuilder);
//			}

//			return predicateImplementations;
//		}

//		private BlockExpression _GetBody(LabelTarget returnLabel, BodyNode bodyNode, IEnumerable<ParameterExpression> variables, IDictionary<string, PredicateBuilder> definedPredicates)
//		{
//			if (bodyNode.Statements.Count > 0)
//			{
//				List<Expression> expressions = new List<Expression>();
//				List<ParameterExpression> localVariables = new List<ParameterExpression>();

//				foreach (StatementNode statementNode in bodyNode.Statements)
//					switch (statementNode.StatementType)
//					{
//						case StatementNodeType.When:
//							expressions.Add(_GetWhenExpression((WhenStatementNode)statementNode, definedPredicates, variables.Concat(localVariables), returnLabel));
//							break;
//						case StatementNodeType.While:
//							expressions.Add(_GetWhileExpression((WhileStatementNode)statementNode, definedPredicates, variables.Concat(localVariables), returnLabel));
//							break;
//						case StatementNodeType.VariableDeclaration:
//							VariableDeclarationStatementNode variableDeclaration = (VariableDeclarationStatementNode)statementNode;
//							ParameterExpression variable = Expression.Variable(_GetCliType(variableDeclaration.VariableType), variableDeclaration.VariableName);
//							expressions.Add(Expression.Assign(variable, _GetExpression(variableDeclaration.InitialValue, definedPredicates, variables.Concat(localVariables))));
//							localVariables.Add(variable);
//							break;
//						case StatementNodeType.VariableAssignment:
//							VariableAssignmentStatementNode variableAssignment = (VariableAssignmentStatementNode)statementNode;
//							expressions.Add(Expression.Assign(variables.Concat(localVariables).First(knownVariable => knownVariable.Name == variableAssignment.VariableName.NameParts[0]), _GetExpression(variableAssignment.Value, definedPredicates, localVariables.Concat(variables))));
//							break;
//						case StatementNodeType.Exit:
//							expressions.Add(_GetExitExpression((ExitStatementNode)statementNode, definedPredicates, variables.Concat(localVariables), returnLabel));
//							break;
//						case StatementNodeType.FunctionCall:
//							expressions.Add(_GetFunctionCallExpression((FunctionCallStatementNode)statementNode, definedPredicates, variables.Concat(localVariables)));
//							break;
//						case StatementNodeType.TryCatchFinally:
//							expressions.Add(_GetTryCatchFinallyExpression((TryCatchFinallyStatementNode)statementNode, definedPredicates, variables.Concat(localVariables), returnLabel));
//							break;
//						case StatementNodeType.Throw:
//							expressions.Add(_GetThrowExpression((ThrowStatementNode)statementNode, definedPredicates, variables.Concat(localVariables)));
//							break;
//						default:
//							expressions.Add(Expression.Empty());
//							break;
//					}

//				return Expression.Block(typeof(void), localVariables, expressions);
//			}
//			else
//				return Expression.Block(typeof(void), Expression.Empty());
//		}

//		private Expression _GetTryCatchFinallyExpression(TryCatchFinallyStatementNode tryCatchFinallyStatementNode, IDictionary<string, PredicateBuilder> definedPredicates, IEnumerable<ParameterExpression> variables, LabelTarget returnLabel)
//		{
//			List<CatchBlock> catchBlocks = new List<CatchBlock>();

//			foreach (CatchBlockNode catchBlockNode in tryCatchFinallyStatementNode.CatchBlocks)
//			{
//				ParameterExpression exception = Expression.Variable(_GetCliType(catchBlockNode.Exception.Type), catchBlockNode.Exception.VariableName);
//				catchBlocks.Add(Expression.Catch(exception, _GetBody(returnLabel, catchBlockNode.Body, variables.Concat(new[] { exception }), definedPredicates)));
//			}

//			if (tryCatchFinallyStatementNode.Finally == null)
//				return Expression.TryCatch(_GetBody(returnLabel, tryCatchFinallyStatementNode.Try, variables, definedPredicates),
//										   catchBlocks.ToArray());
//			else
//				return Expression.TryCatchFinally(_GetBody(returnLabel, tryCatchFinallyStatementNode.Try, variables, definedPredicates),
//												  _GetBody(returnLabel, tryCatchFinallyStatementNode.Finally, variables, definedPredicates),
//												  catchBlocks.ToArray());
//		}

//		private Expression _GetWhileExpression(WhileStatementNode whileStatementNode, IDictionary<string, PredicateBuilder> definedPredicates, IEnumerable<ParameterExpression> variables, LabelTarget returnLabel)
//		{
//			LabelTarget startLabel = Expression.Label();

//			return Expression.Block(typeof(void),
//									Expression.Label(startLabel, Expression.Constant(null)),
//									_GetBody(returnLabel, whileStatementNode.Body, variables, definedPredicates),
//									Expression.IfThen(_GetExpression(whileStatementNode.Condition, definedPredicates, variables), Expression.Goto(startLabel, Expression.Constant(null))));
//		}

//		private Expression _GetWhenExpression(WhenStatementNode whenStatementNode, IDictionary<string, PredicateBuilder> definedPredicates, IEnumerable<ParameterExpression> variables, LabelTarget returnLabel)
//		{
//			if (whenStatementNode.Else == null)
//				return Expression.IfThen(_GetExpression(whenStatementNode.Condition, definedPredicates, variables),
//										 _GetBody(returnLabel, whenStatementNode.Then, variables, definedPredicates));
//			else
//				return Expression.IfThenElse(_GetExpression(whenStatementNode.Condition, definedPredicates, variables),
//											 _GetBody(returnLabel, whenStatementNode.Then, variables, definedPredicates),
//											 _GetBody(returnLabel, whenStatementNode.Else, variables, definedPredicates));
//		}

//		private Expression _GetThrowExpression(ThrowStatementNode throwStatementNode, IDictionary<string, PredicateBuilder> definedPredicates, IEnumerable<ParameterExpression> variables)
//		{
//			if (throwStatementNode.Accessor != null)
//				return Expression.Throw(variables.First(variable => variable.Name == throwStatementNode.Accessor.NameParts[0]));
//			else
//				if (throwStatementNode.FunctionCall != null)
//					return Expression.Throw(_GetFunctionCallExpression(throwStatementNode.FunctionCall, definedPredicates, variables));
//				else
//					return Expression.Throw(null);
//		}

//		private Expression _GetFunctionCallExpression(FunctionCallStatementNode functionCallStatementNode, IDictionary<string, PredicateBuilder> definedPredicates, IEnumerable<ParameterExpression> localVariables)
//		{
//			if (functionCallStatementNode.IsConstructorCall)
//			{
//				Type instanceType = _GetCliType(functionCallStatementNode.FunctionName);

//				if (functionCallStatementNode.Parameters.Count == 0)
//					return Expression.New(instanceType);
//				else
//				{
//					IReadOnlyList<Expression> actualParameters = functionCallStatementNode.Parameters.Select(parameter => _GetExpression(parameter, definedPredicates, localVariables)).ToList();

//					ConstructorInfo constructorInfo = instanceType.GetConstructor(actualParameters.Select(actualParameter => actualParameter.Type).ToArray());
//					if (constructorInfo == null)
//						throw new ArgumentException("Type " + instanceType.Name + " does not have a constructor with parameters " + string.Join(", ", actualParameters.Select(actualParameter => actualParameter.Type.FullName)));
//					else
//						return Expression.New(constructorInfo, actualParameters);
//				}
//			}
//			else
//			{
//				Expression instance;
//				IReadOnlyList<Expression> actualParameters = functionCallStatementNode.Parameters.Select(parameter => _GetExpression(parameter, definedPredicates, localVariables)).ToList();

//				MethodInfo methodInfo = _GetMethod(functionCallStatementNode.FunctionName, actualParameters.Select(actualParameter => actualParameter.Type).ToArray(), out instance, definedPredicates);
//				return Expression.Call(instance, methodInfo, actualParameters);
//			}
//		}

//		private MethodInfo _GetMethod(QualifiedIdentifierNode qualifiedIdentifierNode, Type[] parameterTypes, out Expression instance, IDictionary<string, PredicateBuilder> definedPredicates)
//		{
//			if (qualifiedIdentifierNode.NameParts.Count == 1)
//			{
//				PredicateBuilder predicateBuilder;

//				if (!definedPredicates.TryGetValue(qualifiedIdentifierNode.NameParts[0], out predicateBuilder))
//					throw new ArgumentException("No predicate with the name " + qualifiedIdentifierNode.NameParts[0] + " was found!");

//				instance = Expression.Property(null, predicateBuilder.InstanceProperty);
//				return predicateBuilder.GetInvokeMethod(parameterTypes, defineIfMissing: true);
//			}
//			else
//			{
//				int namespaceDepth = 1;
//				IReadOnlyList<string> typeNameParts = qualifiedIdentifierNode.NameParts.Take(qualifiedIdentifierNode.NameParts.Count - 1).ToList();
//				Type type = typeof(object).Assembly.GetType(string.Join("+", typeNameParts));

//				while (type == null && namespaceDepth < typeNameParts.Count - 1)
//					type = typeof(object).Assembly.GetType(string.Join(".", typeNameParts.Take(namespaceDepth)) + "." + string.Join("+", typeNameParts.Skip(namespaceDepth)));
//				if (type == null)
//					type = typeof(object).Assembly.GetType(string.Join(".", typeNameParts));
//				if (type == null)
//					throw new ArgumentException("Cannot find type " + string.Join("::", typeNameParts) + "!");
//				instance = null;
//				return type.GetMethod(qualifiedIdentifierNode.NameParts[qualifiedIdentifierNode.NameParts.Count - 1], parameterTypes);
//			}
//		}

//		private Expression _GetExitExpression(ExitStatementNode exitStatementNode, IDictionary<string, PredicateBuilder> definedPredicates, IEnumerable<ParameterExpression> localVariables, LabelTarget returnLabel)
//		{
//			if (exitStatementNode.IsSkip)
//				return Expression.Return(returnLabel, _GetNullable(false));
//			else
//				if (exitStatementNode.IsStop)
//					return Expression.Return(returnLabel, _GetNullable<bool>());
//				else
//					return Expression.Return(returnLabel, _GetNullable(_GetExpression(exitStatementNode.Assertion, definedPredicates, localVariables)));
//		}

//		private Expression _GetNullable<T>(T value)
//			where T : struct
//		{
//			return Expression.New(typeof(T?).GetConstructor(new[] { typeof(T) }), Expression.Constant(value, typeof(T)));
//		}

//		private Expression _GetNullable<T>()
//			where T : struct
//		{
//			return Expression.New(typeof(T?));
//		}

//		private Expression _GetNullable(Expression expression)
//		{
//			return Expression.New(typeof(bool?).GetConstructor(new[] { typeof(bool) }), expression);
//		}

//		private Expression _GetExpression(ExpressionNode expressionNode, IDictionary<string, PredicateBuilder> definedPredicates, IEnumerable<ParameterExpression> localVariables)
//		{
//			Stack<Expression> operands = new Stack<Expression>();

//			foreach (ExpressionElement expressionElement in _ToPostFixedForm(expressionNode.Elements))
//				if (expressionElement.ElementType == ExpressionElementType.Operand)
//				{
//					OperandElement operand = (OperandElement)expressionElement;

//					switch (operand.OperandType)
//					{
//						case OperandElementType.FunctionCall:
//							operands.Push(_GetFunctionCallExpression(((OperandElement.FunctionCall)operand).FunctionCallNode, definedPredicates, localVariables));
//							break;
//						case OperandElementType.Accessor:
//							operands.Push(_GetValue((OperandElement.Accessor)operand, localVariables));
//							break;
//						case OperandElementType.Constant:
//							operands.Push(_GetConstant((OperandElement.Constant)operand));
//							break;
//					}
//				}
//				else
//				{
//					OperatorElement operatorElement = (OperatorElement)expressionElement;

//					if (operatorElement.Location == OperatorLocation.Infixed)
//					{
//						Expression operand2 = operands.Pop();
//						Expression operand1 = operands.Pop();

//						switch (operatorElement.OperationName)
//						{
//							case OperationName.Addition:
//								operands.Push(Expression.Add(operand1, operand2));
//								break;
//							case OperationName.Subtraction:
//								operands.Push(Expression.Subtract(operand1, operand2));
//								break;
//							case OperationName.Multiplication:
//								operands.Push(Expression.Multiply(operand1, operand2));
//								break;
//							case OperationName.Division:
//								operands.Push(Expression.Divide(operand1, operand2));
//								break;
//							case OperationName.IntegerDivision:
//								operands.Push(Expression.Convert(Expression.Divide(operand1, operand2), typeof(int)));
//								break;
//							case OperationName.Modulo:
//								operands.Push(Expression.Modulo(operand1, operand2));
//								break;
//							case OperationName.LessThanComparison:
//								operands.Push(Expression.LessThan(operand1, operand2));
//								break;
//							case OperationName.LessThanOrEqualToComparison:
//								operands.Push(Expression.LessThanOrEqual(operand1, operand2));
//								break;
//							case OperationName.EqualComparison:
//								operands.Push(Expression.Equal(operand1, operand2));
//								break;
//							case OperationName.GreaterThanOrEqualToComparison:
//								operands.Push(Expression.GreaterThanOrEqual(operand1, operand2));
//								break;
//							case OperationName.GreaterThanComparison:
//								operands.Push(Expression.GreaterThan(operand1, operand2));
//								break;
//							case OperationName.Disjunction:
//								operands.Push(Expression.And(operand1, operand2));
//								break;
//							case OperationName.Conjuction:
//								operands.Push(Expression.Or(operand1, operand2));
//								break;
//						}
//					}
//					else
//					{
//						Expression operand = operands.Pop();

//						switch (operatorElement.OperationName)
//						{
//							case OperationName.Negation:
//								operands.Push(Expression.Not(operand));
//								break;
//							case OperationName.IntegerPromotion:
//								operands.Push(Expression.Convert(operand, typeof(int)));
//								break;
//							case OperationName.AdditiveInverse:
//								operands.Push(Expression.Negate(operand));
//								break;
//						}
//					}
//				}

//			return operands.Pop();
//		}

//		private Expression _GetValue(OperandElement.Accessor accessor, IEnumerable<ParameterExpression> localVariables)
//		{
//			IReadOnlyList<string> nameParts = accessor.AccessorPath.NameParts;

//			if (nameParts.Count == 1)
//			{
//				Expression variable = localVariables.FirstOrDefault(localVariable => localVariable.Name == nameParts[0]);

//				if (variable == null)
//					throw new ArgumentException("Undefined local variable " + nameParts[0]);
//				return variable;
//			}
//			else
//				// do accessor resolution
//				throw new InvalidOperationException("Accessors except local variables are not available at this time.");
//		}

//		private Expression _GetConstant(OperandElement.Constant constant)
//		{
//			switch (constant.ConstantType)
//			{
//				case ConstantNodeType.Integer:
//					return Expression.Constant(int.Parse(constant.Value), typeof(int));
//				case ConstantNodeType.Float:
//					return Expression.Constant(float.Parse(constant.Value), typeof(float));
//				case ConstantNodeType.Character:
//					return Expression.Constant(constant.Value[0], typeof(char));
//				case ConstantNodeType.String:
//					return Expression.Constant(constant.Value, typeof(string));
//				case ConstantNodeType.Boolean:
//					return Expression.Constant(bool.Parse(constant.Value), typeof(bool));
//				default:
//					throw new ArgumentException("Expected constant!");
//			}
//		}

//		private IReadOnlyList<ExpressionElement> _ToPostFixedForm(IReadOnlyList<ExpressionElement> expressionElements)
//		{
//			List<ExpressionElement> postfixedExpressionElements = new List<ExpressionElement>();
//			Stack<OperatorElement> operators = new Stack<OperatorElement>();
//			Queue<ExpressionElement> operands = new Queue<ExpressionElement>();

//			foreach (ExpressionElement expressionElement in expressionElements)
//			{
//				if (expressionElement.ElementType == ExpressionElementType.Operand)
//					operands.Enqueue(expressionElement);
//				else
//					if (expressionElement.ElementType == ExpressionElementType.Parenthesis)
//					{
//						if (!((ParenthesisElement)expressionElement).IsOpenning)
//						{
//							while (operands.Count > 0)
//								postfixedExpressionElements.Add(operands.Dequeue());
//							while (operators.Count > 0)
//								postfixedExpressionElements.Add(operators.Pop());
//						}
//					}
//					else
//					{
//						OperatorElement operatorElement = (OperatorElement)expressionElement;

//						if (operators.Count > 0
//							&& _operations[operatorElement.OperationName] < _operations[operators.Peek().OperationName])
//						{
//							while (operands.Count > 0)
//								postfixedExpressionElements.Add(operands.Dequeue());
//							while (operators.Count > 0)
//								postfixedExpressionElements.Add(operators.Pop());
//						}
//						operators.Push(operatorElement);
//					}
//			}
//			while (operands.Count > 0)
//				postfixedExpressionElements.Add(operands.Dequeue());
//			while (operators.Count > 0)
//				postfixedExpressionElements.Add(operators.Pop());

//			return postfixedExpressionElements;
//		}
//	}
//}
