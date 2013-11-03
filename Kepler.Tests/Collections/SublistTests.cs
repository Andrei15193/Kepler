using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Andrei15193.Kepler.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Andrei15193.Kepler.Tests.Collections
{
    [TestClass]
    public sealed class SublistTests
    {
        static private readonly string[] _emptyArray = new string[0];
        static private readonly string[] _oneElementArray = new[] { "1" };
        static private readonly string[] _twoElementArray = new[] { "1", "2" };
        static private readonly string[] _threeElementArray = new[] { "1", "2", "3" };
        static private readonly string[] _fourElementArray = new[] { "1", "2", "3", "4" };
        static private readonly string[] _fiveElementArray = new[] { "1", "2", "3", "4", "5" };

        [TestMethod]
        public void TestWhenArrayIsEmpty()
        {
            IReadOnlyList<string> sublist = new Sublist<string>(_emptyArray);

            Assert.AreEqual(0, sublist.Count);
        }

        [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestWhenArrayIsEmptyAndStartIndexOne()
        {
            IReadOnlyList<string> sublist = new Sublist<string>(_emptyArray, startIndex: 1);
        }

        [TestMethod]
        public void TestWhenArrayIsEmptyAndLengthOne()
        {
            IReadOnlyList<string> sublist = new Sublist<string>(_emptyArray, length: 1);

            Assert.AreEqual(0, sublist.Count);
        }

        [TestMethod]
        public void TestWhenArrayHasOneElement()
        {
            int sourceIndex;
            IReadOnlyList<string> sublist = new Sublist<string>(_oneElementArray);

            Assert.AreEqual(1, sublist.Count);
            using (IEnumerator<string> sublistEnumerator = sublist.GetEnumerator())
                for (sourceIndex = 0; sourceIndex < _oneElementArray.Length && sublistEnumerator.MoveNext(); sourceIndex++)
                    Assert.AreSame(_oneElementArray[sourceIndex], sublistEnumerator.Current);
            Assert.AreEqual(1, sourceIndex);

            {
                IEnumerator sublistEnumerator = ((IEnumerable)sublist).GetEnumerator();

                for (sourceIndex = 0; sourceIndex < _oneElementArray.Length && sublistEnumerator.MoveNext(); sourceIndex++)
                    Assert.AreSame(_oneElementArray[sourceIndex], (string)sublistEnumerator.Current);
                Assert.AreEqual(1, sourceIndex);
            }
        }

        [TestMethod]
        public void TestWhenArrayHasOneElementAndStartIndexOne()
        {
            IReadOnlyList<string> sublist = new Sublist<string>(_oneElementArray, startIndex: 1);

            Assert.AreEqual(0, sublist.Count);
        }

        [TestMethod]
        public void TestWhenArrayHasOneElementAndStartIndexOneAndLengthOne()
        {
            IReadOnlyList<string> sublist = new Sublist<string>(_oneElementArray, startIndex: 1, length: 1);

            Assert.AreEqual(0, sublist.Count);
        }

        [TestMethod]
        public void TestWhenArrayHasTwoElements()
        {
            int sourceIndex;
            IReadOnlyList<string> sublist = new Sublist<string>(_twoElementArray);

            Assert.AreEqual(2, sublist.Count);
            using (IEnumerator<string> sublistEnumerator = sublist.GetEnumerator())
                for (sourceIndex = 0; sourceIndex < _twoElementArray.Length && sublistEnumerator.MoveNext(); sourceIndex++)
                    Assert.AreSame(_twoElementArray[sourceIndex], sublistEnumerator.Current);
            Assert.AreEqual(2, sourceIndex);

            {
                IEnumerator sublistEnumerator = ((IEnumerable)sublist).GetEnumerator();

                for (sourceIndex = 0; sourceIndex < _twoElementArray.Length && sublistEnumerator.MoveNext(); sourceIndex++)
                    Assert.AreSame(_twoElementArray[sourceIndex], (string)sublistEnumerator.Current);
                Assert.AreEqual(2, sourceIndex);
            }
        }

        [TestMethod]
        public void TestWhenArrayHasTwoElementsAndStartIndexOne()
        {
            int sourceIndex;
            IReadOnlyList<string> sublist = new Sublist<string>(_twoElementArray, startIndex: 1);

            Assert.AreEqual(1, sublist.Count);
            using (IEnumerator<string> sublistEnumerator = sublist.GetEnumerator())
                for (sourceIndex = 1; sourceIndex < _twoElementArray.Length && sublistEnumerator.MoveNext(); sourceIndex++)
                    Assert.AreSame(_twoElementArray[sourceIndex], sublistEnumerator.Current);
            Assert.AreEqual(2, sourceIndex);

            {
                IEnumerator sublistEnumerator = ((IEnumerable)sublist).GetEnumerator();

                for (sourceIndex = 1; sourceIndex < _twoElementArray.Length && sublistEnumerator.MoveNext(); sourceIndex++)
                    Assert.AreSame(_twoElementArray[sourceIndex], (string)sublistEnumerator.Current);
                Assert.AreEqual(2, sourceIndex);
            }
        }

        [TestMethod]
        public void TestWhenArrayHasTwoElementsAndLengthOne()
        {
            int sourceIndex;
            IReadOnlyList<string> sublist = new Sublist<string>(_twoElementArray, length: 1);

            Assert.AreEqual(1, sublist.Count);
            using (IEnumerator<string> sublistEnumerator = sublist.GetEnumerator())
                for (sourceIndex = 0; sourceIndex < _twoElementArray.Length && sublistEnumerator.MoveNext(); sourceIndex++)
                    Assert.AreSame(_twoElementArray[sourceIndex], sublistEnumerator.Current);
            Assert.AreEqual(1, sourceIndex);

            {
                IEnumerator sublistEnumerator = ((IEnumerable)sublist).GetEnumerator();

                for (sourceIndex = 0; sourceIndex < _twoElementArray.Length && sublistEnumerator.MoveNext(); sourceIndex++)
                    Assert.AreSame(_twoElementArray[sourceIndex], (string)sublistEnumerator.Current);
                Assert.AreEqual(1, sourceIndex);
            }
        }

        [TestMethod]
        public void TestWhenArrayHasTwoElementsAndStartIndexOneAndLengthOne()
        {
            int sourceIndex;
            IReadOnlyList<string> sublist = new Sublist<string>(_twoElementArray, startIndex: 1, length: 1);

            Assert.AreEqual(1, sublist.Count);
            using (IEnumerator<string> sublistEnumerator = sublist.GetEnumerator())
                for (sourceIndex = 1; sourceIndex < _twoElementArray.Length && sublistEnumerator.MoveNext(); sourceIndex++)
                    Assert.AreSame(_twoElementArray[sourceIndex], sublistEnumerator.Current);
            Assert.AreEqual(2, sourceIndex);

            {
                IEnumerator sublistEnumerator = ((IEnumerable)sublist).GetEnumerator();

                for (sourceIndex = 1; sourceIndex < _twoElementArray.Length && sublistEnumerator.MoveNext(); sourceIndex++)
                    Assert.AreSame(_twoElementArray[sourceIndex], (string)sublistEnumerator.Current);
                Assert.AreEqual(2, sourceIndex);
            }
        }

        [TestMethod]
        public void TestWhenArrayHasThreeElements()
        {
            int sourceIndex;
            IReadOnlyList<string> sublist = new Sublist<string>(_threeElementArray);

            Assert.AreEqual(3, sublist.Count);
            using (IEnumerator<string> sublistEnumerator = sublist.GetEnumerator())
                for (sourceIndex = 0; sourceIndex < _threeElementArray.Length && sublistEnumerator.MoveNext(); sourceIndex++)
                    Assert.AreSame(_threeElementArray[sourceIndex], sublistEnumerator.Current);
            Assert.AreEqual(3, sourceIndex);

            {
                IEnumerator sublistEnumerator = ((IEnumerable)sublist).GetEnumerator();

                for (sourceIndex = 0; sourceIndex < _threeElementArray.Length && sublistEnumerator.MoveNext(); sourceIndex++)
                    Assert.AreSame(_threeElementArray[sourceIndex], (string)sublistEnumerator.Current);
                Assert.AreEqual(3, sourceIndex);
            }
        }

        [TestMethod]
        public void TestWhenArrayHasThreeElementsAndStartIndexOne()
        {
            int sourceIndex;
            IReadOnlyList<string> sublist = new Sublist<string>(_threeElementArray, startIndex: 1);

            Assert.AreEqual(2, sublist.Count);
            using (IEnumerator<string> sublistEnumerator = sublist.GetEnumerator())
                for (sourceIndex = 1; sourceIndex < _threeElementArray.Length && sublistEnumerator.MoveNext(); sourceIndex++)
                    Assert.AreSame(_threeElementArray[sourceIndex], sublistEnumerator.Current);
            Assert.AreEqual(3, sourceIndex);

            {
                IEnumerator sublistEnumerator = ((IEnumerable)sublist).GetEnumerator();

                for (sourceIndex = 1; sourceIndex < _threeElementArray.Length && sublistEnumerator.MoveNext(); sourceIndex++)
                    Assert.AreSame(_threeElementArray[sourceIndex], (string)sublistEnumerator.Current);
                Assert.AreEqual(3, sourceIndex);
            }
        }

        [TestMethod]
        public void TestWhenArrayHasThreeElementsAndLengthOne()
        {
            int sourceIndex;
            IReadOnlyList<string> sublist = new Sublist<string>(_threeElementArray, length: 1);

            Assert.AreEqual(1, sublist.Count);
            using (IEnumerator<string> sublistEnumerator = sublist.GetEnumerator())
                for (sourceIndex = 0; sourceIndex < _threeElementArray.Length && sublistEnumerator.MoveNext(); sourceIndex++)
                    Assert.AreSame(_threeElementArray[sourceIndex], sublistEnumerator.Current);
            Assert.AreEqual(1, sourceIndex);

            {
                IEnumerator sublistEnumerator = ((IEnumerable)sublist).GetEnumerator();

                for (sourceIndex = 0; sourceIndex < _threeElementArray.Length && sublistEnumerator.MoveNext(); sourceIndex++)
                    Assert.AreSame(_threeElementArray[sourceIndex], (string)sublistEnumerator.Current);
                Assert.AreEqual(1, sourceIndex);
            }
        }

        [TestMethod]
        public void TestWhenArrayHasThreeElementsAndStartIndexOneAndLengthOne()
        {
            int sourceIndex;
            IReadOnlyList<string> sublist = new Sublist<string>(_threeElementArray, startIndex: 1, length: 1);

            Assert.AreEqual(1, sublist.Count);
            using (IEnumerator<string> sublistEnumerator = sublist.GetEnumerator())
                for (sourceIndex = 1; sourceIndex < _threeElementArray.Length && sublistEnumerator.MoveNext(); sourceIndex++)
                    Assert.AreSame(_threeElementArray[sourceIndex], sublistEnumerator.Current);
            Assert.AreEqual(2, sourceIndex);

            {
                IEnumerator sublistEnumerator = ((IEnumerable)sublist).GetEnumerator();

                for (sourceIndex = 1; sourceIndex < _threeElementArray.Length && sublistEnumerator.MoveNext(); sourceIndex++)
                    Assert.AreSame(_threeElementArray[sourceIndex], (string)sublistEnumerator.Current);
                Assert.AreEqual(2, sourceIndex);
            }
        }

        [TestMethod]
        public void TestWhenSublistFromSublist()
        {
            int sourceIndex;
            IReadOnlyList<string> sublist = new Sublist<string>(new Sublist<string>(_threeElementArray, startIndex: 1), length: 1);

            Assert.AreEqual(1, sublist.Count);
            using (IEnumerator<string> sublistEnumerator = sublist.GetEnumerator())
                for (sourceIndex = 1; sourceIndex < _threeElementArray.Length && sublistEnumerator.MoveNext(); sourceIndex++)
                    Assert.AreSame(_threeElementArray[sourceIndex], sublistEnumerator.Current);
            Assert.AreEqual(2, sourceIndex);

            {
                IEnumerator sublistEnumerator = ((IEnumerable)sublist).GetEnumerator();

                for (sourceIndex = 1; sourceIndex < _threeElementArray.Length && sublistEnumerator.MoveNext(); sourceIndex++)
                    Assert.AreSame(_threeElementArray[sourceIndex], (string)sublistEnumerator.Current);
                Assert.AreEqual(2, sourceIndex);
            }
        }

        [TestMethod]
        public void TestSublistFromSublistFor1000000Times()
        {
            int currentSublist = 0;
            Sublist<string> sublist = new Sublist<string>(_fourElementArray);

            do
            {
                int sourceIndex;

                sublist = new Sublist<string>((IReadOnlyList<string>)sublist);
                Assert.AreEqual(4, sublist.Count);
                using (IEnumerator<string> sublistEnumerator = sublist.GetEnumerator())
                    for (sourceIndex = 0; sourceIndex < _fiveElementArray.Length && sublistEnumerator.MoveNext(); sourceIndex++)
                        Assert.AreSame(_fiveElementArray[sourceIndex], sublistEnumerator.Current);
                Assert.AreEqual(4, sourceIndex);

                {
                    IEnumerator sublistEnumerator = ((IEnumerable)sublist).GetEnumerator();

                    for (sourceIndex = 0; sourceIndex < _fourElementArray.Length && sublistEnumerator.MoveNext(); sourceIndex++)
                        Assert.AreSame(_fourElementArray[sourceIndex], (string)sublistEnumerator.Current);
                    Assert.AreEqual(4, sourceIndex);
                }

                currentSublist++;
            } while (currentSublist < 1000000);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void TestWhenListIsNull()
        {
            new Sublist<string>(null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestWhenStartIndexIsNegative()
        {
            new Sublist<string>(_fiveElementArray, startIndex: -1);
        }

        [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestWhenStartIndexIsGreaterThanTheListLength()
        {
            new Sublist<string>(_fiveElementArray, startIndex: 6);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void TestWhenLengthIsNegative()
        {
            new Sublist<string>(_fiveElementArray, length: -1);
        }

        [TestMethod, ExpectedException(typeof(IndexOutOfRangeException))]
        public void TestWhenIndexIsIsNegative()
        {
            string value = new Sublist<string>(_fiveElementArray)[-1];
        }

        [TestMethod, ExpectedException(typeof(IndexOutOfRangeException))]
        public void TestWhenIndexIsEqualToLength()
        {
            string value = new Sublist<string>(_fiveElementArray)[5];
        }

        [TestMethod, ExpectedException(typeof(IndexOutOfRangeException))]
        public void TestWhenIndexIsGreaterThanLength()
        {
            string value = new Sublist<string>(_fiveElementArray)[6];
        }
    }
}
