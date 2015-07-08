using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Bovender.Extensions;

namespace Bovender.UnitTests.Extensions
{
    [TestFixture]
    class ArrayExtensionsTest
    {
        [Test]
        public void SliceArray()
        {
            int[] array = new int[] { 0, 1, 2, 3, 4, 5 };
            int[] slice = array.Slice(2, 2);
            Assert.AreEqual(2, slice[0]);
        }

        [Test]
        public void SliceArrayWithInvalidParams()
        {
            int[] array = new int[] { 0, 1, 2, 3, 4, 5 };
            Assert.Throws<ArgumentOutOfRangeException>(
                () =>
                {
                    int[] slice = array.Slice(20, 2);
                },
                "Index is larger than array has elements"
            );
            Assert.Throws<ArgumentOutOfRangeException>(
                () =>
                {
                    int[] slice = array.Slice(-1, 2);
                },
                "Index is lower than 0"
            );
            Assert.Throws<ArgumentOutOfRangeException>(
                () =>
                {
                    int[] slice = array.Slice(2, 5);
                },
                "Length is too large"
            );
            Assert.Throws<ArgumentOutOfRangeException>(
                () =>
                {
                    int[] slice = array.Slice(2, 0);
                },
                "Length is 0"
            );
        }
    }
}
