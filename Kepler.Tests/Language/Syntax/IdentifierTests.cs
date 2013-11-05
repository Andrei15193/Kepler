using System;
using Andrei15193.Kepler.Language.Lexis;
using Andrei15193.Kepler.Language.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Andrei15193.Kepler.Tests.Language.Syntax
{
    [TestClass]
    public class IdentifierTests
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
        public void TestTryCreteWhenAtomListIsEmpty()
        {
            int length;
            Identifier identifier;

            Assert.IsFalse(Identifier.TryCreate(_emptyAtomList, out length, out identifier));
        }

        [TestMethod]
        public void TestTryCreateWhenAtomListContainsOneIdentifierWithoutValue()
        {
            int length;
            Identifier identifier;

            Assert.IsFalse(Identifier.TryCreate(_oneIdentifierWithoutValueList, out length, out identifier));
        }

        [TestMethod]
        public void TestTryCreateWhenAtomListContainsOneIdentifier()
        {
            int length;
            Identifier identifier;

            Assert.IsTrue(Identifier.TryCreate(_oneIdentifierList, out length, out identifier));
            Assert.IsNotNull(identifier);
            Assert.AreEqual(1, length);
            Assert.AreEqual(identifier.Name, _oneIdentifierList[0].Value);
            Assert.IsTrue(identifier.IsTerminal);
            Assert.AreEqual(ProductType.Identifier, identifier.ProductType);
            Assert.AreEqual(_oneIdentifierList[0].Line, identifier.Line);
            Assert.AreEqual(_oneIdentifierList[0].Column, identifier.Column);
        }

        [TestMethod]
        public void TestTryCreateWhenAtomListContainsQualifiedIdentifierAndStartIndexZero()
        {
            int length;
            Identifier identifier;

            Assert.IsTrue(Identifier.TryCreate(_qualifiedIdentifier, out length, out identifier));
            Assert.IsNotNull(identifier);
            Assert.AreEqual(1, length);
            Assert.AreEqual(identifier.Name, _qualifiedIdentifier[0].Value);
            Assert.IsTrue(identifier.IsTerminal);
            Assert.AreEqual(ProductType.Identifier, identifier.ProductType);
            Assert.AreEqual(_qualifiedIdentifier[0].Line, identifier.Line);
            Assert.AreEqual(_qualifiedIdentifier[0].Column, identifier.Column);
        }

        [TestMethod]
        public void TestTryCreateWhenAtomListContainsQualifiedIdentifierAndStartIndexTwoo()
        {
            int length;
            Identifier identifier;

            Assert.IsTrue(Identifier.TryCreate(_qualifiedIdentifier, out length, out identifier, startIndex: 2));
            Assert.AreEqual(1, length);
            Assert.IsNotNull(identifier);
            Assert.AreEqual(identifier.Name, _qualifiedIdentifier[2].Value);
            Assert.IsTrue(identifier.IsTerminal);
            Assert.AreEqual(ProductType.Identifier, identifier.ProductType);
            Assert.AreEqual(_qualifiedIdentifier[2].Line, identifier.Line);
            Assert.AreEqual(_qualifiedIdentifier[2].Column, identifier.Column);
        }

        [TestMethod]
        public void TestTryCreateWhenAtomListContainsQualifiedIdentifierAndStartIndexDoesNotPointToIdentifierAtom()
        {
            int length;
            Identifier identifier;

            Assert.IsFalse(Identifier.TryCreate(_qualifiedIdentifier, out length, out identifier, startIndex: 1));
        }

        [TestMethod]
        public void TestTryCreateWhenAtomListContainsIdentifierFollowedByScope()
        {
            int length;
            Identifier identifier;

            Assert.IsTrue(Identifier.TryCreate(_twoAtomList, out length, out identifier));
            Assert.AreEqual(1, length);
            Assert.IsNotNull(identifier);
            Assert.IsTrue(identifier.IsTerminal);
            Assert.AreEqual(ProductType.Identifier, identifier.ProductType);
            Assert.AreEqual(_twoAtomList[0].Value, identifier.Name);
            Assert.AreEqual(_twoAtomList[0].Line, identifier.Line);
            Assert.AreEqual(_twoAtomList[0].Column, identifier.Column);
        }

        [TestMethod]
        public void TestTryCreateWhenAtomListContainsOneIdentifierBetweenScopes()
        {
            int length;
            Identifier identifier;

            Assert.IsTrue(Identifier.TryCreate(_identifierBetweenScopes, out length, out identifier, 1));
            Assert.AreEqual(1, length);
            Assert.IsNotNull(identifier);
            Assert.IsTrue(identifier.IsTerminal);
            Assert.AreEqual(ProductType.Identifier, identifier.ProductType);
            Assert.AreEqual(_identifierBetweenScopes[1].Value, identifier.Name);
            Assert.AreEqual(_identifierBetweenScopes[1].Line, identifier.Line);
            Assert.AreEqual(_identifierBetweenScopes[1].Column, identifier.Column);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void TestTryCreateWhenAtomListIsNull()
        {
            int length;
            Identifier identifier;

            Assert.IsFalse(Identifier.TryCreate(null, out length, out identifier));
        }

        [TestMethod]
        public void TestTryCreateWhenStartIndexIsNegative()
        {
            int length;
            Identifier identifier;

            Assert.IsFalse(Identifier.TryCreate(_emptyAtomList, out length, out identifier, -1));
        }

        [TestMethod]
        public void TestTryCreateWhenStartIndexEqualToAtomListLength()
        {
            int length;
            Identifier identifier;

            Assert.IsFalse(Identifier.TryCreate(_emptyAtomList, out length, out identifier, _emptyAtomList.Length));
        }

        [TestMethod]
        public void TestTryCreateWhenStartIndexIsGreaterThanAtomListLength()
        {
            int length;
            Identifier identifier;

            Assert.IsFalse(Identifier.TryCreate(_emptyAtomList, out length, out identifier, _emptyAtomList.Length + 1));
        }
    }
}
