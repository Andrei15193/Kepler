using System;
using Andrei15193.Kepler.Language.Lexis;
using Andrei15193.Kepler.Language.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Andrei15193.Kepler.Tests.Language.Syntax
{
    [TestClass]
    public class QualifiedIdentifierTests
    {
        static private readonly ScannedAtom<Lexicon>[] _emptyAtomList = new ScannedAtom<Lexicon>[0];
        static private readonly ScannedAtom<Lexicon>[] _oneIdentifierWithoutValueList = new[]
            {
                new ScannedAtom<Lexicon>(Lexicon.Identifier, 0, 0)
            };
        static private readonly ScannedAtom<Lexicon>[] _oneIdentifierList = new[]
            {
                new ScannedAtom<Lexicon>(Lexicon.Identifier, 1, 1, "a")
            };
        static private readonly ScannedAtom<Lexicon>[] _twoAtomList = new[]
            {
                new ScannedAtom<Lexicon>(Lexicon.Identifier, 1, 1, "a"),
                new ScannedAtom<Lexicon>(Lexicon.Scope, 1, 2)
            };
        static private readonly ScannedAtom<Lexicon>[] _qualifiedIdentifier = new[]
            {
                new ScannedAtom<Lexicon>(Lexicon.Identifier, 1, 1, "a"),
                new ScannedAtom<Lexicon>(Lexicon.Scope, 1, 2),
                new ScannedAtom<Lexicon>(Lexicon.Identifier, 1, 4, "b")
            };
        static private readonly ScannedAtom<Lexicon>[] _identifierBetweenScopes = new[]
            {
                new ScannedAtom<Lexicon>(Lexicon.Scope, 1, 1),
                new ScannedAtom<Lexicon>(Lexicon.Identifier, 1, 3, "a"),
                new ScannedAtom<Lexicon>(Lexicon.Scope, 1, 5)
            };

        [TestMethod]
        public void TestTryCreateWhenAtomListIsEmpty()
        {
            int length;
            QualifiedIdentifier qualifiedIdentifier;

            Assert.IsFalse(QualifiedIdentifier.TryCreate(_emptyAtomList, out length, out qualifiedIdentifier));
        }

        [TestMethod]
        public void TestTryCreateWhenAtomListContainsOneIdentifierWithoutvalue()
        {
            int length;
            QualifiedIdentifier qualifiedIdentifier;

            Assert.IsFalse(QualifiedIdentifier.TryCreate(_oneIdentifierWithoutValueList, out length, out qualifiedIdentifier));
        }

        [TestMethod]
        public void TestTryCreateWhenAtomListContainsOneIdentifier()
        {
            int length;
            QualifiedIdentifier qualifiedIdentifier;

            Assert.IsTrue(QualifiedIdentifier.TryCreate(_oneIdentifierList, out length, out qualifiedIdentifier));
            Assert.AreEqual(1, length);
            Assert.IsNotNull(qualifiedIdentifier);
            Assert.IsTrue(qualifiedIdentifier.IsTerminal);
            Assert.AreEqual(ProductType.QualifiedIdentifier, qualifiedIdentifier.ProductType);
            Assert.AreEqual(_oneIdentifierList[0].Value, qualifiedIdentifier.Name);
            Assert.AreEqual(1, qualifiedIdentifier.Identifiers.Count);
            Assert.AreEqual(_oneIdentifierList[0].Value, qualifiedIdentifier.Identifiers[0]);
            Assert.AreEqual(0, qualifiedIdentifier.Scopes.Count);
            Assert.AreEqual(_oneIdentifierList[0].Line, qualifiedIdentifier.Line);
            Assert.AreEqual(_oneIdentifierList[0].Column, qualifiedIdentifier.Column);
        }

        [TestMethod]
        public void TestTryCreateWhenAtomListContainsOneQualifiedIdentifierMadeOutOfTwoIdentifiers()
        {
            int length;
            QualifiedIdentifier qualifiedIdentifier;

            Assert.IsTrue(QualifiedIdentifier.TryCreate(_qualifiedIdentifier, out length, out qualifiedIdentifier));
            Assert.AreEqual(3, length);
            Assert.IsNotNull(qualifiedIdentifier);
            Assert.IsTrue(qualifiedIdentifier.IsTerminal);
            Assert.AreEqual(ProductType.QualifiedIdentifier, qualifiedIdentifier.ProductType);
            Assert.AreEqual(_qualifiedIdentifier[2].Value, qualifiedIdentifier.Name);
            Assert.AreEqual(2, qualifiedIdentifier.Identifiers.Count);
            Assert.AreEqual(_qualifiedIdentifier[0].Value, qualifiedIdentifier.Identifiers[0]);
            Assert.AreEqual(_qualifiedIdentifier[2].Value, qualifiedIdentifier.Identifiers[1]);
            Assert.AreEqual(1, qualifiedIdentifier.Scopes.Count);
            Assert.AreEqual(_qualifiedIdentifier[0].Value, qualifiedIdentifier.Scopes[0]);
            Assert.AreEqual(_qualifiedIdentifier[0].Line, qualifiedIdentifier.Line);
            Assert.AreEqual(_qualifiedIdentifier[0].Column, qualifiedIdentifier.Column);
        }

        [TestMethod]
        public void TestTryCreateWhenAtomListContainsOneQualifiedIdentifierAndStartIndexDoesNotPointToAnIdentifier()
        {
            int length;
            QualifiedIdentifier qualifiedIdentifier;

            Assert.IsFalse(QualifiedIdentifier.TryCreate(_qualifiedIdentifier, out length, out qualifiedIdentifier, 1));
        }

        [TestMethod]
        public void TestTryCreateWhenAtomListContainsOneQualifiedIdentifierMadeOutOfTwoIdentifiersAndStartIndexPointsToTheLast()
        {
            int length;
            QualifiedIdentifier qualifiedIdentifier;

            Assert.IsTrue(QualifiedIdentifier.TryCreate(_qualifiedIdentifier, out length, out qualifiedIdentifier, 2));
            Assert.AreEqual(1, length);
            Assert.IsNotNull(qualifiedIdentifier);
            Assert.IsTrue(qualifiedIdentifier.IsTerminal);
            Assert.AreEqual(ProductType.QualifiedIdentifier, qualifiedIdentifier.ProductType);
            Assert.AreEqual(_qualifiedIdentifier[2].Value, qualifiedIdentifier.Name);
            Assert.AreEqual(1, qualifiedIdentifier.Identifiers.Count);
            Assert.AreEqual(_qualifiedIdentifier[2].Value, qualifiedIdentifier.Identifiers[0]);
            Assert.AreEqual(0, qualifiedIdentifier.Scopes.Count);
            Assert.AreEqual(_qualifiedIdentifier[2].Line, qualifiedIdentifier.Line);
            Assert.AreEqual(_qualifiedIdentifier[2].Column, qualifiedIdentifier.Column);
        }

        [TestMethod]
        public void TestTryCreateWhenAtomListContainsOneIdentifierFolowedByScope()
        {
            int length;
            QualifiedIdentifier qualifiedIdentifier;

            Assert.IsTrue(QualifiedIdentifier.TryCreate(_twoAtomList, out length, out qualifiedIdentifier));
            Assert.AreEqual(1, length);
            Assert.IsNotNull(qualifiedIdentifier);
            Assert.IsTrue(qualifiedIdentifier.IsTerminal);
            Assert.AreEqual(ProductType.QualifiedIdentifier, qualifiedIdentifier.ProductType);
            Assert.AreEqual(_twoAtomList[0].Value, qualifiedIdentifier.Name);
            Assert.AreEqual(1, qualifiedIdentifier.Identifiers.Count);
            Assert.AreEqual(_twoAtomList[0].Value, qualifiedIdentifier.Identifiers[0]);
            Assert.AreEqual(0, qualifiedIdentifier.Scopes.Count);
            Assert.AreEqual(_twoAtomList[0].Line, qualifiedIdentifier.Line);
            Assert.AreEqual(_twoAtomList[0].Column, qualifiedIdentifier.Column);
        }

        [TestMethod]
        public void TestTryCreateWhenAtomListContainsOneIdentifierBetweenScopes()
        {
            int length;
            QualifiedIdentifier qualifiedIdentifier;

            Assert.IsTrue(QualifiedIdentifier.TryCreate(_identifierBetweenScopes, out length, out qualifiedIdentifier, 1));
            Assert.AreEqual(1, length);
            Assert.IsNotNull(qualifiedIdentifier);
            Assert.IsTrue(qualifiedIdentifier.IsTerminal);
            Assert.AreEqual(ProductType.QualifiedIdentifier, qualifiedIdentifier.ProductType);
            Assert.AreEqual(_identifierBetweenScopes[1].Value, qualifiedIdentifier.Name);
            Assert.AreEqual(1, qualifiedIdentifier.Identifiers.Count);
            Assert.AreEqual(_identifierBetweenScopes[1].Value, qualifiedIdentifier.Identifiers[0]);
            Assert.AreEqual(0, qualifiedIdentifier.Scopes.Count);
            Assert.AreEqual(_identifierBetweenScopes[1].Line, qualifiedIdentifier.Line);
            Assert.AreEqual(_identifierBetweenScopes[1].Column, qualifiedIdentifier.Column);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void TestTryCreateWhenAtomListIsNull()
        {
            int length;
            QualifiedIdentifier qualifiedIdentifier;

            Assert.IsFalse(QualifiedIdentifier.TryCreate(null, out length, out qualifiedIdentifier));
        }

        [TestMethod]
        public void TestTryCreateWhenStartIndexIsNegative()
        {
            int length;
            QualifiedIdentifier qualifiedIdentifier;

            Assert.IsFalse(QualifiedIdentifier.TryCreate(_emptyAtomList, out length, out qualifiedIdentifier, -1));
        }

        [TestMethod]
        public void TestTryCreateWhenStartIndexIsEqualToAtomListLength()
        {
            int length;
            QualifiedIdentifier qualifiedIdentifier;

            Assert.IsFalse(QualifiedIdentifier.TryCreate(_emptyAtomList, out length, out qualifiedIdentifier, _emptyAtomList.Length));
        }

        [TestMethod]
        public void TestTryCreateWhenStartIndexIsGreaterThanAtomListLength()
        {
            int length;
            QualifiedIdentifier qualifiedIdentifier;

            Assert.IsFalse(QualifiedIdentifier.TryCreate(_emptyAtomList, out length, out qualifiedIdentifier, _emptyAtomList.Length + 1));
        }
    }
}
