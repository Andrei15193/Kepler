//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using Andrei15193.Kepler.Language;
//using Andrei15193.Kepler.Language.Lexis;
//using Microsoft.VisualStudio.TestTools.UnitTesting;

//namespace Andrei15193.Kepler.Tests
//{
//    [TestClass]
//    public sealed class StatementTests
//    {
//        [TestClass]
//        public sealed class BodyAtomListTests
//            : Statement
//        {
//            static private readonly IReadOnlyList<ScannedAtom<Lexicon>> _emptySequence = new ScannedAtom<Lexicon>[0];
//            static private readonly IReadOnlyList<ScannedAtom<Lexicon>> _emptyBody = new[]
//                {
//                    new ScannedAtom<Lexicon>(Lexicon.Begin, 1, 1),
//                    new ScannedAtom<Lexicon>(Lexicon.End, 2, 1)
//                };
//            static private readonly IReadOnlyList<ScannedAtom<Lexicon>> _bodyOnlyWithSkip = new[]
//                {
//                    new ScannedAtom<Lexicon>(Lexicon.Begin, 1, 1),
//                    new ScannedAtom<Lexicon>(Lexicon.Skip, 2, 1),
//                    new ScannedAtom<Lexicon>(Lexicon.End, 3, 1)
//                };
//            static private readonly IReadOnlyList<ScannedAtom<Lexicon>> _bodyOnlyWithStop = new[]
//                {
//                    new ScannedAtom<Lexicon>(Lexicon.Begin, 1, 1),
//                    new ScannedAtom<Lexicon>(Lexicon.Stop, 2, 1),
//                    new ScannedAtom<Lexicon>(Lexicon.End, 3, 1)
//                };

//            static private readonly IReadOnlyList<ScannedAtom<Lexicon>> _sequenceWithBegin = new[]
//                {
//                    new ScannedAtom<Lexicon>(Lexicon.Begin, 1, 1),
//                };

//            static private readonly IReadOnlyList<ScannedAtom<Lexicon>> _sequenceWithEnd = new[]
//                {
//                    new ScannedAtom<Lexicon>(Lexicon.End, 1, 1),
//                };

//            [TestMethod]
//            public void TestWithOneEmptyBodyInSequence()
//            {
//                IReadOnlyList<ScannedAtom<Lexicon>> result = new Statement.BodyAtomList(_emptyBody);
//                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(_emptyBody.SequenceEqual(result));
//            }

//            [TestMethod]
//            public void TestWithTwoEmptyBodiesInSequence()
//            {
//                IReadOnlyList<ScannedAtom<Lexicon>> result = new Statement.BodyAtomList(_emptyBody.Concat(_emptyBody).ToList());
//                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(_emptyBody.SequenceEqual(result));
//            }

//            [TestMethod, ExpectedException(typeof(ArgumentException))]
//            public void TestWithUnclosedBody()
//            {
//                IReadOnlyList<ScannedAtom<Lexicon>> result = new Statement.BodyAtomList(_sequenceWithBegin);
//            }

//            [TestMethod, ExpectedException(typeof(ArgumentException))]
//            public void TestWithUnopenedBody()
//            {
//                new Statement.BodyAtomList(_sequenceWithEnd);
//            }

//            [TestMethod, ExpectedException(typeof(ArgumentException))]
//            public void TestWithEmptyBody()
//            {
//                new Statement.BodyAtomList(_emptySequence);
//            }

//            [TestMethod, ExpectedException(typeof(ArgumentNullException))]
//            public void TestWithNullBody()
//            {
//                new Statement.BodyAtomList(null);
//            }

//            public BodyAtomListTests()
//                : base(new ScannedAtom<Lexicon>[0], Statement.Type.Empty)
//            {
//            }

//            public override Expression ToExpression()
//            {
//                throw new NotImplementedException();
//            }
//        }

//        [TestClass]
//        public class QualifiedIdentifierAtomListTests
//            : Statement
//        {
//            static private readonly IReadOnlyList<ScannedAtom<Lexicon>> _onlyOneIdentifier = new[]
//                {
//                    new ScannedAtom<Lexicon>(Lexicon.Identifier, 1, 1, "a")
//                };
//            static private readonly IReadOnlyList<ScannedAtom<Lexicon>> _qualifiedIdentifier = new[]
//                {
//                    new ScannedAtom<Lexicon>(Lexicon.Identifier, 1, 1, "a"),
//                    new ScannedAtom<Lexicon>(Lexicon.Scope, 1, 2),
//                    new ScannedAtom<Lexicon>(Lexicon.Identifier, 1, 4, "b")
//                };
//            static private readonly IReadOnlyList<ScannedAtom<Lexicon>> _qualifiedIdentifierEndingWithScopeOperator = new[]
//                {
//                    _qualifiedIdentifier[0],
//                    _qualifiedIdentifier[1],
//                    _qualifiedIdentifier[2],
//                    new ScannedAtom<Lexicon>(Lexicon.Scope, 1, 15)
//                };

//            [TestMethod]
//            public void TestWhenSequenceContainsOnlyIdentifier()
//            {
//                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(new Statement.QualifiedIdentifierAtomList(_onlyOneIdentifier).SequenceEqual(_onlyOneIdentifier));
//            }

//            [TestMethod]
//            public void TestWhenSequenceContainsQualifiedIdentifier()
//            {
//                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(new Statement.QualifiedIdentifierAtomList(_qualifiedIdentifier).SequenceEqual(_qualifiedIdentifier));
//            }

//            [TestMethod]
//            public void TestWhenSequenceContainsQualifiedIdentifierEndedWtihScopeOperator()
//            {
//                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(new Statement.QualifiedIdentifierAtomList(_qualifiedIdentifierEndingWithScopeOperator).SequenceEqual(_qualifiedIdentifier));
//            }

//            public QualifiedIdentifierAtomListTests()
//                : base(new ScannedAtom<Lexicon>[0], Statement.Type.Empty)
//            {
//            }

//            public override Expression ToExpression()
//            {
//                throw new NotImplementedException();
//            }
//        }

//        [TestClass]
//        public class TypeAtomListTests
//            : Statement
//        {
//            private readonly IReadOnlyList<ScannedAtom<Lexicon>> _simpleTypeName = new[]
//                {
//                    new ScannedAtom<Lexicon>(Lexicon.Identifier, 1, 1, "typeName")
//                };
//            private readonly IReadOnlyList<ScannedAtom<Lexicon>> _qualifiedTypeName = new[]
//                {
//                    new ScannedAtom<Lexicon>(Lexicon.Identifier, 1, 1, "typeName1"),
//                    new ScannedAtom<Lexicon>(Lexicon.Scope, 1, 10),
//                    new ScannedAtom<Lexicon>(Lexicon.Identifier, 1, 12, "typeName2")
//                };
//            private readonly IReadOnlyList<ScannedAtom<Lexicon>> _simpleTypeNameWithOneSimpleGenericArgument = new[]
//                {
//                    new ScannedAtom<Lexicon>(Lexicon.Identifier, 1, 1, "typeName1"),
//                    new ScannedAtom<Lexicon>(Lexicon.LessThan, 1, 10),
//                    new ScannedAtom<Lexicon>(Lexicon.Identifier, 1, 12, "typeName2"),
//                    new ScannedAtom<Lexicon>(Lexicon.GreaterThan, 1, 22)
//                };
//            private readonly IReadOnlyList<ScannedAtom<Lexicon>> _simpleTypeNameWithOneQualifiedGenericArgument = new[]
//                {
//                    new ScannedAtom<Lexicon>(Lexicon.Identifier, 1, 1, "typeName1"),
//                    new ScannedAtom<Lexicon>(Lexicon.LessThan, 1, 10),
//                    new ScannedAtom<Lexicon>(Lexicon.Identifier, 1, 12, "typeName2"),
//                    new ScannedAtom<Lexicon>(Lexicon.Scope, 1, 22),
//                    new ScannedAtom<Lexicon>(Lexicon.Identifier, 1, 24, "typeName3"),
//                    new ScannedAtom<Lexicon>(Lexicon.GreaterThan, 1, 34)
//                };
//            private readonly IReadOnlyList<ScannedAtom<Lexicon>> _simpleTypeNameWithTwoQualifiedGenericArguments = new[]
//                {
//                    new ScannedAtom<Lexicon>(Lexicon.Identifier, 1, 1, "typeName1"),
//                    new ScannedAtom<Lexicon>(Lexicon.LessThan, 1, 10),
//                    new ScannedAtom<Lexicon>(Lexicon.Identifier, 1, 12, "typeName2"),
//                    new ScannedAtom<Lexicon>(Lexicon.Scope, 1, 22),
//                    new ScannedAtom<Lexicon>(Lexicon.Identifier, 1, 24, "typeName3"),
//                    new ScannedAtom<Lexicon>(Lexicon.Comma, 1, 34),
//                    new ScannedAtom<Lexicon>(Lexicon.Identifier, 1, 35, "typeName4"),
//                    new ScannedAtom<Lexicon>(Lexicon.Scope, 1, 45),
//                    new ScannedAtom<Lexicon>(Lexicon.Identifier, 1, 47, "typeName5"),
//                    new ScannedAtom<Lexicon>(Lexicon.GreaterThan, 1, 57)
//                };
//            private readonly IReadOnlyList<ScannedAtom<Lexicon>> _simpleTypeNameWithTwoQualifiedGenericArgumentsNotSeparatedWithComma = new[]
//                {
//                    new ScannedAtom<Lexicon>(Lexicon.Identifier, 1, 1, "typeName1"),
//                    new ScannedAtom<Lexicon>(Lexicon.LessThan, 1, 10),
//                    new ScannedAtom<Lexicon>(Lexicon.Identifier, 1, 12, "typeName2"),
//                    new ScannedAtom<Lexicon>(Lexicon.Scope, 1, 22),
//                    new ScannedAtom<Lexicon>(Lexicon.Identifier, 1, 24, "typeName3"),
//                    new ScannedAtom<Lexicon>(Lexicon.Skip, 1, 34),
//                    new ScannedAtom<Lexicon>(Lexicon.Identifier, 1, 35, "typeName4"),
//                    new ScannedAtom<Lexicon>(Lexicon.Scope, 1, 45),
//                    new ScannedAtom<Lexicon>(Lexicon.Identifier, 1, 47, "typeName5"),
//                    new ScannedAtom<Lexicon>(Lexicon.GreaterThan, 1, 57)
//                };

//            [TestMethod]
//            public void TestWithNoGenericParameterAndNoQualifiedName()
//            {
//                TypeAtomList typeAtomList = new TypeAtomList(_simpleTypeName);

//                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, typeAtomList.Count);
//                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(0, typeAtomList.GenericArguments.Count);
//                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(typeAtomList.SequenceEqual(_simpleTypeName));
//            }

//            [TestMethod]
//            public void TestWithNoGenericParameterAndQualifiedName()
//            {
//                TypeAtomList typeAtomList = new TypeAtomList(_qualifiedTypeName);

//                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(3, typeAtomList.Count);
//                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(0, typeAtomList.GenericArguments.Count);
//                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(typeAtomList.SequenceEqual(_qualifiedTypeName));
//            }

//            [TestMethod]
//            public void TestWithOneSimpleGenericArgumentAndSimpleTypeName()
//            {
//                TypeAtomList typeAtomList = new TypeAtomList(_simpleTypeNameWithOneSimpleGenericArgument);

//                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(_simpleTypeNameWithOneSimpleGenericArgument.Count, typeAtomList.Count);
//                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, typeAtomList.GenericArguments.Count);
//                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, typeAtomList.GenericArguments[0].Count);
//                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreSame(typeAtomList.GenericArguments[0][0], _simpleTypeNameWithOneSimpleGenericArgument[2]);
//                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(typeAtomList.SequenceEqual(_simpleTypeNameWithOneSimpleGenericArgument));
//            }

//            [TestMethod]
//            public void TestWithOneQualifiedGenericArgumentAndSimpleTypeName()
//            {
//                TypeAtomList typeAtomList = new TypeAtomList(_simpleTypeNameWithOneQualifiedGenericArgument);

//                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(_simpleTypeNameWithOneQualifiedGenericArgument.Count, typeAtomList.Count);
//                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, typeAtomList.GenericArguments.Count);
//                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(3, typeAtomList.GenericArguments[0].Count);
//                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreSame(typeAtomList.GenericArguments[0][0], _simpleTypeNameWithOneQualifiedGenericArgument[2]);
//                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreSame(typeAtomList.GenericArguments[0][1], _simpleTypeNameWithOneQualifiedGenericArgument[3]);
//                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreSame(typeAtomList.GenericArguments[0][2], _simpleTypeNameWithOneQualifiedGenericArgument[4]);
//                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(typeAtomList.SequenceEqual(_simpleTypeNameWithOneQualifiedGenericArgument));
//            }

//            [TestMethod]
//            public void TestWithTwoQualifiedGenericArgumentsAndSimpleTypeName()
//            {
//                TypeAtomList typeAtomList = new TypeAtomList(_simpleTypeNameWithTwoQualifiedGenericArguments);

//                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(_simpleTypeNameWithTwoQualifiedGenericArguments.Count, typeAtomList.Count);
//                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(2, typeAtomList.GenericArguments.Count);
//                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(3, typeAtomList.GenericArguments[0].Count);
//                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(3, typeAtomList.GenericArguments[1].Count);
//                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreSame(typeAtomList.GenericArguments[0][0], _simpleTypeNameWithTwoQualifiedGenericArguments[2]);
//                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreSame(typeAtomList.GenericArguments[0][1], _simpleTypeNameWithTwoQualifiedGenericArguments[3]);
//                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreSame(typeAtomList.GenericArguments[0][2], _simpleTypeNameWithTwoQualifiedGenericArguments[4]);
//                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreSame(typeAtomList.GenericArguments[1][0], _simpleTypeNameWithTwoQualifiedGenericArguments[6]);
//                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreSame(typeAtomList.GenericArguments[1][1], _simpleTypeNameWithTwoQualifiedGenericArguments[7]);
//                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreSame(typeAtomList.GenericArguments[1][2], _simpleTypeNameWithTwoQualifiedGenericArguments[8]);
//                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(typeAtomList.SequenceEqual(_simpleTypeNameWithTwoQualifiedGenericArguments));
//            }

//            [TestMethod, ExpectedException(typeof(ArgumentException))]
//            public void TestWhenGenericArgumentsAreNotSeparatedWithComma()
//            {
//                TypeAtomList typeAtomList = new TypeAtomList(_simpleTypeNameWithTwoQualifiedGenericArgumentsNotSeparatedWithComma);
//            }

//            public TypeAtomListTests()
//                : base(new ScannedAtom<Lexicon>[0], Type.Empty)
//            {
//            }

//            public override Expression ToExpression()
//            {
//                throw new NotImplementedException();
//            }
//        }

//        [TestClass]
//        public sealed class AssertStatementTests
//        {
//            static private readonly IReadOnlyList<ScannedAtom<Lexicon>> _onlyAssertAtom = new[]
//            {
//                new ScannedAtom<Lexicon>(Lexicon.Assert, 1, 1)
//            };
//            static private readonly IReadOnlyList<ScannedAtom<Lexicon>> _onlyAssertAndDotAtoms = new[]
//            {
//                new ScannedAtom<Lexicon>(Lexicon.Assert, 1, 1),
//                new ScannedAtom<Lexicon>(Lexicon.Dot, 1, 7)
//            };
//            static private readonly IReadOnlyList<ScannedAtom<Lexicon>> _assertWithSimpleConditionEndedWithoutDot = new[]
//            {
//                new ScannedAtom<Lexicon>(Lexicon.Assert, 1, 1),
//                new ScannedAtom<Lexicon>(Lexicon.True, 1, 7)
//            };
//            static private readonly IReadOnlyList<ScannedAtom<Lexicon>> _assertWithCompoundConditionEndedWithoutDot = new[]
//            {
//                new ScannedAtom<Lexicon>(Lexicon.Assert, 1, 1),
//                new ScannedAtom<Lexicon>(Lexicon.True, 1, 7),
//                new ScannedAtom<Lexicon>(Lexicon.Or, 1, 12),
//                new ScannedAtom<Lexicon>(Lexicon.False, 1, 15)
//            };
//            static private readonly IReadOnlyList<ScannedAtom<Lexicon>> _assertWithSimpleConditionEndedWithDot = new[]
//            {
//                new ScannedAtom<Lexicon>(Lexicon.Assert, 1, 1),
//                new ScannedAtom<Lexicon>(Lexicon.True, 1, 7),
//                new ScannedAtom<Lexicon>(Lexicon.Dot, 1, 8)
//            };
//            static private readonly IReadOnlyList<ScannedAtom<Lexicon>> _assertWithCompoundConditionEndedWithDot = new[]
//            {
//                new ScannedAtom<Lexicon>(Lexicon.Assert, 1, 1),
//                new ScannedAtom<Lexicon>(Lexicon.True, 1, 7),
//                new ScannedAtom<Lexicon>(Lexicon.Or, 1, 12),
//                new ScannedAtom<Lexicon>(Lexicon.False, 1, 15),
//                new ScannedAtom<Lexicon>(Lexicon.Dot, 1, 16)
//            };
//            static private readonly IReadOnlyList<ScannedAtom<Lexicon>> _whenStatement = new[]
//            {
//                new ScannedAtom<Lexicon>(Lexicon.When, 1, 1),
//                new ScannedAtom<Lexicon>(Lexicon.True, 1, 5),
//                new ScannedAtom<Lexicon>(Lexicon.Then, 1, 20),
//                new ScannedAtom<Lexicon>(Lexicon.Skip, 2, 1)
//            };

//            [TestMethod, ExpectedException(typeof(ArgumentException))]
//            public void TestWhenSequenceContainsAssertOnly()
//            {
//                Statement.Assert(_onlyAssertAtom);
//            }

//            [TestMethod, ExpectedException(typeof(ArgumentException))]
//            public void TestWhenSequenceContainsAssertFollowedByDot()
//            {
//                Statement.Assert(_onlyAssertAndDotAtoms);
//            }

//            [TestMethod, ExpectedException(typeof(ArgumentException))]
//            public void TestWhenSequenceContainsAConstantAndNotEndedWithDot()
//            {
//                Statement.Assert(_assertWithSimpleConditionEndedWithoutDot);
//            }

//            [TestMethod, ExpectedException(typeof(ArgumentException))]
//            public void TestWhenSequenceContainsCompoundExpressionAndNotEndedWithDot()
//            {
//                Statement.Assert(_assertWithCompoundConditionEndedWithoutDot);
//            }

//            [TestMethod, ExpectedException(typeof(ArgumentException))]
//            public void TestWhenSequenceIsNotAssert()
//            {
//                Statement.Assert(_whenStatement);
//            }

//            [TestMethod]
//            public void TestWhenSequenceContainsAConstantAndEndedWithDot()
//            {
//                Assert.IsTrue(Statement.Assert(_assertWithSimpleConditionEndedWithDot).SequenceEqual(_assertWithSimpleConditionEndedWithDot));
//            }

//            [TestMethod]
//            public void TestWhenSequenceContainsCompoundExpressionAndEndedWithDot()
//            {
//                Assert.IsTrue(Statement.Assert(_assertWithCompoundConditionEndedWithDot).SequenceEqual(_assertWithCompoundConditionEndedWithDot));
//            }

//            [TestMethod]
//            public void TestWhenThereAreTwoAssertSequences()
//            {
//                Assert.IsTrue(Statement.Assert(_assertWithCompoundConditionEndedWithDot.Concat(_assertWithCompoundConditionEndedWithDot).ToList()).SequenceEqual(_assertWithCompoundConditionEndedWithDot));
//            }
//        }

//        [TestClass]
//        public class SkipStatementTests
//        {
//            static private readonly IReadOnlyList<ScannedAtom<Lexicon>> _onlySkipAtom = new[]
//            {
//                new ScannedAtom<Lexicon>(Lexicon.Skip, 1, 1)
//            };
//            static private readonly IReadOnlyList<ScannedAtom<Lexicon>> _onlySkipEndedWithDotAtoms = new[]
//            {
//                new ScannedAtom<Lexicon>(Lexicon.Skip, 1, 1),
//                new ScannedAtom<Lexicon>(Lexicon.Dot, 1, 5)
//            };
//            static private readonly IReadOnlyList<ScannedAtom<Lexicon>> _whenStatement = new[]
//            {
//                new ScannedAtom<Lexicon>(Lexicon.When, 1, 1),
//                new ScannedAtom<Lexicon>(Lexicon.True, 1, 5),
//                new ScannedAtom<Lexicon>(Lexicon.Then, 1, 20),
//                new ScannedAtom<Lexicon>(Lexicon.Skip, 2, 1)
//            };

//            [TestMethod, ExpectedException(typeof(ArgumentException))]
//            public void TestWhenSequenceContainsOnlySkipAtom()
//            {
//                Statement.Skip(_onlySkipAtom);
//            }

//            [TestMethod]
//            public void TestWhenSequenceContainsOnlySkipEndedWithDotAtom()
//            {
//                Assert.IsTrue(Statement.Skip(_onlySkipEndedWithDotAtoms).SequenceEqual(_onlySkipEndedWithDotAtoms));
//            }

//            [TestMethod, ExpectedException(typeof(ArgumentException))]
//            public void TestWhenSequenceIsNotSkipStatement()
//            {
//                Statement.Skip(_whenStatement);
//            }

//            [TestMethod]
//            public void TestWhenSequenceContainsMoreThanOneSkipStatement()
//            {
//                Assert.IsTrue(Statement.Skip(_onlySkipEndedWithDotAtoms.Concat(_onlySkipEndedWithDotAtoms).ToList()).SequenceEqual(_onlySkipEndedWithDotAtoms));
//            }
//        }

//        [TestClass]
//        public class StopStatementTests
//        {
//            static private readonly IReadOnlyList<ScannedAtom<Lexicon>> _onlyStopAtom = new[]
//            {
//                new ScannedAtom<Lexicon>(Lexicon.Stop, 1, 1)
//            };
//            static private readonly IReadOnlyList<ScannedAtom<Lexicon>> _onlyStopEndedWithDotAtoms = new[]
//            {
//                new ScannedAtom<Lexicon>(Lexicon.Stop, 1, 1),
//                new ScannedAtom<Lexicon>(Lexicon.Dot, 1, 5)
//            };
//            static private readonly IReadOnlyList<ScannedAtom<Lexicon>> _whenStatement = new[]
//            {
//                new ScannedAtom<Lexicon>(Lexicon.When, 1, 1),
//                new ScannedAtom<Lexicon>(Lexicon.True, 1, 5),
//                new ScannedAtom<Lexicon>(Lexicon.Then, 1, 20),
//                new ScannedAtom<Lexicon>(Lexicon.Skip, 2, 1)
//            };

//            [TestMethod, ExpectedException(typeof(ArgumentException))]
//            public void TestWhenSequenceContainsOnlyStopAtom()
//            {
//                Statement.Stop(_onlyStopAtom);
//            }

//            [TestMethod]
//            public void TestWhenSequenceContainsOnlyStopEndedWithDotAtom()
//            {
//                Assert.IsTrue(Statement.Stop(_onlyStopEndedWithDotAtoms).SequenceEqual(_onlyStopEndedWithDotAtoms));
//            }

//            [TestMethod, ExpectedException(typeof(ArgumentException))]
//            public void TestWhenSequenceIsNotStopStatement()
//            {
//                Statement.Stop(_whenStatement);
//            }

//            [TestMethod]
//            public void TestWhenSequenceContainsMoreThanOneStopStatement()
//            {
//                Assert.IsTrue(Statement.Stop(_onlyStopEndedWithDotAtoms.Concat(_onlyStopEndedWithDotAtoms).ToList()).SequenceEqual(_onlyStopEndedWithDotAtoms));
//            }
//        }
//    }
//}
