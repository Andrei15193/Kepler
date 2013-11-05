using System;
using Andrei15193.Kepler.Language.Lexis;
using Andrei15193.Kepler.Language.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Andrei15193.Kepler.Tests.Language.Syntax
{
    [TestClass]
    public class ProductTests
         : Product
    {
        static private readonly ScannedAtom<Lexicon>[] _emptyAtomList = new ScannedAtom<Lexicon>[0];
        static private readonly ScannedAtom<Lexicon>[] _oneAtomList = new[]
            {
                new ScannedAtom<Lexicon>(Lexicon.Identifier, 1, 1, "a")
            };
        static private readonly ScannedAtom<Lexicon>[] _twoAtomList = new[]
            {
                new ScannedAtom<Lexicon>(Lexicon.Identifier, 1, 1, "a"),
                new ScannedAtom<Lexicon>(Lexicon.Scope, 1, 2)
            };
        static private readonly ScannedAtom<Lexicon>[] _threeAtomList = new[]
            {
                new ScannedAtom<Lexicon>(Lexicon.Identifier, 1, 1, "a"),
                new ScannedAtom<Lexicon>(Lexicon.Scope, 1, 2),
                new ScannedAtom<Lexicon>(Lexicon.Identifier, 1, 4, "b")
            };

        [TestClass]
        public class ExceptionFactoryTests
        {
            [TestMethod]
            public void TestCreateExpectedAtomWhenAtomTypeIsProvided()
            {
                Assert.IsNotNull(ExceptionFactory.CreateExpectedAtom("atom", 0, 0));
            }

            [TestMethod, ExpectedException(typeof(ArgumentNullException))]
            public void TestCreateExpectedAtomWhenAtomTypeIsNull()
            {
                ExceptionFactory.CreateExpectedAtom(null, 0, 0);
            }

            [TestMethod, ExpectedException(typeof(ArgumentException))]
            public void TestCreateExpectedAtomWhenAtomTypeIsEmpty()
            {
                ExceptionFactory.CreateExpectedAtom("", 0, 0);
            }

            [TestMethod, ExpectedException(typeof(ArgumentException))]
            public void TestCreateExpectedAtomWhenAtomTypeContainsOnlyWhitespaces()
            {
                ExceptionFactory.CreateExpectedAtom("   \n\r\t", 0, 0);
            }

            [TestMethod]
            public void TestCreateExpectedAtomWhenSymbolIsProvided()
            {
                Assert.IsNotNull(ExceptionFactory.CreateExpectedSymbol("atom", 0, 0));
            }

            [TestMethod, ExpectedException(typeof(ArgumentNullException))]
            public void TestCreateExpectedAtomWhenSymbolIsNull()
            {
                ExceptionFactory.CreateExpectedSymbol(null, 0, 0);
            }

            [TestMethod, ExpectedException(typeof(ArgumentException))]
            public void TestCreateExpectedAtomWhenSymbolIsEmpty()
            {
                ExceptionFactory.CreateExpectedSymbol("", 0, 0);
            }

            [TestMethod]
            public void TestCreateExpectedAtomWhenSymbolContainsOnlyWhitespaces()
            {
                Assert.IsNotNull(ExceptionFactory.CreateExpectedSymbol("   \n\r\t", 0, 0));
            }
        }

        [TestMethod]
        public void TestValidateWhenAtomListIsEmptyAndStartIndexZero()
        {
            Assert.IsNotNull(Validate(_emptyAtomList, 0));
        }

        [TestMethod]
        public void TestValidateWhenAtomListIsEmptyAndStartIndexOne()
        {
            Assert.IsNotNull(Validate(_emptyAtomList, 1));
        }

        [TestMethod]
        public void TestValidateWhenAtomListIsEmptyAndStartIndexZeroAndMinimumLengthZero()
        {
            Assert.IsNotNull(Validate(_emptyAtomList, 0, 0));
        }

        [TestMethod]
        public void TestValidateWhenAtomListIsEmptyAndStartIndexZeroAndMinimumLengthOne()
        {
            Assert.IsNotNull(Validate(_emptyAtomList, 0, 1));
        }

        [TestMethod]
        public void TestValidateWhenAtomListHasOneAtomAndStartIndexZero()
        {
            Assert.IsNull(Validate(_oneAtomList, 0));
        }

        [TestMethod]
        public void TestValidateWhenAtomListHasOneAtomAndStartIndexOne()
        {
            Assert.IsNotNull(Validate(_oneAtomList, 1));
        }

        [TestMethod]
        public void TestValidateWhenAtomListHasOneAtomAndStartIndexZeroAndMinimumLengthZero()
        {
            Assert.IsNull(Validate(_oneAtomList, 0, 0));
        }

        [TestMethod]
        public void TestValidateWhenAtomListHasOneAtomAndStartIndexZeroAndMinimumLengthOne()
        {
            Assert.IsNull(Validate(_oneAtomList, 0, 1));
        }

        [TestMethod]
        public void TestValidateWhenAtomListHasOneAtomAndStartIndexOneAndMinimumLengthZero()
        {
            Assert.IsNotNull(Validate(_oneAtomList, 1, 0));
        }

        [TestMethod]
        public void TestValidateWhenAtomListHasOneAtomAndStartIndexOneAndMinimumLengthOne()
        {
            Assert.IsNotNull(Validate(_oneAtomList, 1, 1));
        }

        [TestMethod]
        public void TestValidateWhenAtomListHasTwoAtomsAndStartIndexZero()
        {
            Assert.IsNull(Validate(_twoAtomList, 0));
        }

        [TestMethod]
        public void TestValidateWhenAtomListHasTwoAtomsAndStartIndexOne()
        {
            Assert.IsNull(Validate(_twoAtomList, 1));
        }

        [TestMethod]
        public void TestValidateWhenAtomListHasTwoAtomsAndStartIndexTwo()
        {
            Assert.IsNotNull(Validate(_twoAtomList, 2));
        }

        [TestMethod]
        public void TestValidateWhenAtomListHasTwoAtomsAndStartIndexZeroAndMinimumLengthOne()
        {
            Assert.IsNull(Validate(_twoAtomList, 0, 1));
        }

        [TestMethod]
        public void TestValidateWhenAtomListHasTwoAtomsAndStartIndexOneAndMinimumLengthOne()
        {
            Assert.IsNull(Validate(_twoAtomList, 1, 1));
        }

        [TestMethod]
        public void TestValidateWhenAtomListHasTwoAtomsAndStartIndexTwoAndMinimumLengthOne()
        {
            Assert.IsNotNull(Validate(_twoAtomList, 2, 1));
        }

        [TestMethod]
        public void TestValidateWhenAtomListHasTwoAtomsAndStartIndexZeroAndMinimumLengthTwo()
        {
            Assert.IsNull(Validate(_twoAtomList, 0, 2));
        }

        [TestMethod]
        public void TestValidateWhenAtomListHasTwoAtomsAndStartIndexOneAndMinimumLengthTwo()
        {
            Assert.IsNotNull(Validate(_twoAtomList, 1, 2));
        }

        [TestMethod]
        public void TestValidateWhenAtomListHasTwoAtomsAndStartIndexTwoAndMinimumLengthTwo()
        {
            Assert.IsNotNull(Validate(_twoAtomList, 2, 2));
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void TestValidateWhenAtomListIsNull()
        {
            Validate(null, 0, 0);
        }

        public ProductTests()
            : base(ProductType.Identifier, isTerminal: true)
        {
        }

        public override uint Line
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override uint Column
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
