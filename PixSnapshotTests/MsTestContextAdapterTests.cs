﻿/*
    PixSnapshot
    The MIT License (MIT)

    Copyright (c) 2018 Luiz Fernando

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using PixSnapshot;

namespace PixSnapshotTests
{
    [TestClass]
    public class MsTestContextAdapterTests
    {
        [TestMethod]
        public void TestMsTestContextAdapter()
        {
            var sut = new MsTestContextAdapter(TestContext);

            Assert.AreEqual(TestContext.TestRunDirectory, sut.TestRunDirectory);
            Assert.AreEqual(TestContext.FullyQualifiedTestClassName, sut.FullyQualifiedTestClassName);
            Assert.AreEqual(TestContext.TestName, sut.TestName);
        }

        [TestMethod]
        public void TestMsTestAdapter()
        {
            var sut = new MsTestAdapter();

            Assert.ThrowsException<AssertFailedException>(() => sut.AssertFailure("Test"));
        }

        public TestContext TestContext { get; set; }
    }
}