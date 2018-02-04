/*
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

using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PixSnapshot;

namespace PixSnapshotTests
{
    [TestClass]
    public class BitmapSnapshotTestingTests
    {
        [TestInitialize]
        public void TestInitialize()
        {
            BitmapSnapshotTesting.SeparateDirectoriesPerNamespace = false;
        }

        [TestMethod]
        public void TestSnapshotSuccess()
        {
            var bitmapRef = new Bitmap(16, 16);
            var bitmapAct = new Bitmap(16, 16);
            var testContext = new MockTestContext();
            var testAdapter = new MockTestAdapter
            {
                LoadReferenceImage_return = bitmapRef,
                ReferenceImageExists_return = true,
                TestResultsSavePath_return = @"C:\Test\TestFilesPath"
            };
            
            BitmapSnapshotTesting.Snapshot<MockSnapshotProvider, Bitmap>(bitmapAct, testAdapter, testContext, false);
            
            Assert.IsNull(testAdapter.AssertFailure_message);
            Assert.IsNull(testAdapter.SaveBitmapFile_bitmap);
            Assert.IsNull(testAdapter.SaveComparisonBitmapFiles_expected);
            Assert.IsNull(testAdapter.SaveComparisonBitmapFiles_expectedPath);
            Assert.IsNull(testAdapter.SaveComparisonBitmapFiles_actual);
            Assert.IsNull(testAdapter.SaveComparisonBitmapFiles_actualPath);
        }
        
        [TestMethod]
        public void TestSnapshotFailureWhenNotEqual()
        {
            var bitmapRef = new Bitmap(16, 16);
            var bitmapAct = new Bitmap(16, 16); bitmapAct.SetPixel(0, 0, Color.Red);
            var testContext = new MockTestContext
            {
                TestRunDirectory = "C:\\Test\\Artifacts",
                FullyQualifiedTestClassName = "Test.TestClass",
                TestName = "TestName"
            };
            var testAdapter = new MockTestAdapter
            {
                LoadReferenceImage_return = bitmapRef,
                ReferenceImageExists_return = true,
                TestResultsSavePath_return = @"C:\Test\TestFilesPath"
            };
            
            BitmapSnapshotTesting.Snapshot<MockSnapshotProvider, Bitmap>(bitmapAct, testAdapter, testContext, false);
            
            Assert.IsNotNull(testAdapter.AssertFailure_message);
            Assert.AreEqual(bitmapRef, testAdapter.SaveComparisonBitmapFiles_expected);
            Assert.AreEqual(@"C:\Test\Artifacts\Test.TestClass\TestName-expected.png", testAdapter.SaveComparisonBitmapFiles_expectedPath);
            Assert.AreEqual(bitmapAct, testAdapter.SaveComparisonBitmapFiles_actual);
            Assert.AreEqual(@"C:\Test\Artifacts\Test.TestClass\TestName-actual.png", testAdapter.SaveComparisonBitmapFiles_actualPath);
        }
        
        [TestMethod]
        public void TestSnapshotFailureWhenReferenceImageNotFound()
        {
            var bitmapRef = new Bitmap(16, 16);
            var bitmapAct = new Bitmap(16, 16); bitmapAct.SetPixel(0, 0, Color.Red);
            var testContext = new MockTestContext
            {
                TestRunDirectory = "C:\\Test\\Artifacts",
                FullyQualifiedTestClassName = "Test.TestClass",
                TestName = "TestName"
            };
            var testAdapter = new MockTestAdapter
            {
                LoadReferenceImage_return = bitmapRef,
                ReferenceImageExists_return = false,
                TestResultsSavePath_return = @"C:\Test\TestFilesPath"
            };
            
            BitmapSnapshotTesting.Snapshot<MockSnapshotProvider, Bitmap>(bitmapAct, testAdapter, testContext, false);
            
            Assert.IsNotNull(testAdapter.AssertFailure_message);
            Assert.IsNull(testAdapter.SaveBitmapFile_bitmap);
            Assert.IsNull(testAdapter.SaveComparisonBitmapFiles_expected);
            Assert.IsNull(testAdapter.SaveComparisonBitmapFiles_expectedPath);
            Assert.IsNull(testAdapter.SaveComparisonBitmapFiles_actual);
            Assert.IsNull(testAdapter.SaveComparisonBitmapFiles_actualPath);
        }

        [TestMethod]
        public void TestSnapshotRecordingAlwaysFails()
        {
            var bitmapAct = new Bitmap(16, 16);
            var testContext = new MockTestContext
            {
                TestRunDirectory = "C:\\Test\\Artifacts",
                FullyQualifiedTestClassName = "Test.TestClass",
                TestName = "TestName"
            };
            var testAdapter = new MockTestAdapter
            {
                ReferenceImageExists_return = false,
                TestResultsSavePath_return = @"C:\Test\TestFilesPath"
            };
            
            BitmapSnapshotTesting.Snapshot<MockSnapshotProvider, Bitmap>(bitmapAct, testAdapter, testContext, true);
            
            Assert.IsNotNull(testAdapter.AssertFailure_message);
        }

        [TestMethod]
        public void TestSnapshotRecordingModeRecordsReferenceImage()
        {
            // Arrange
            var bitmapAct = new Bitmap(16, 16);
            var testContext = new MockTestContext
            {
                TestRunDirectory = "C:\\Test\\Artifacts",
                FullyQualifiedTestClassName = "Test.TestClass",
                TestName = "TestName"
            };
            var testAdapter = new MockTestAdapter
            {
                ReferenceImageExists_return = false,
                TestResultsSavePath_return = @"C:\Test\TestFilesPath"
            };

            // Act
            BitmapSnapshotTesting.Snapshot<MockSnapshotProvider, Bitmap>(bitmapAct, testAdapter, testContext, true);

            // Assert
            Assert.IsNotNull(testAdapter.AssertFailure_message);
            
            Assert.AreEqual(bitmapAct, testAdapter.SaveBitmapFile_bitmap);
            Assert.AreEqual(@"C:\Test\TestFilesPath\Test.TestClass\TestName.png", testAdapter.SaveBitmapFile_path);

            Assert.IsNull(testAdapter.SaveComparisonBitmapFiles_expected);
            Assert.IsNull(testAdapter.SaveComparisonBitmapFiles_expectedPath);
            Assert.IsNull(testAdapter.SaveComparisonBitmapFiles_actual);
            Assert.IsNull(testAdapter.SaveComparisonBitmapFiles_actualPath);
        }
        
        [TestMethod]
        public void TestSnapshotSuccessSeparateDirectoriesPerNamespace()
        {
            var bitmapRef = new Bitmap(16, 16);
            var bitmapAct = new Bitmap(16, 16);
            var testContext = new MockTestContext
            {
                TestRunDirectory = "C:\\Test\\Artifacts",
                FullyQualifiedTestClassName = "Test.TestClass",
                TestName = "TestName"
            };
            var testAdapter = new MockTestAdapter
            {
                LoadReferenceImage_return = bitmapRef,
                ReferenceImageExists_return = true,
                TestResultsSavePath_return = @"C:\Test\TestFilesPath"
            };

            BitmapSnapshotTesting.SeparateDirectoriesPerNamespace = true;
            BitmapSnapshotTesting.Snapshot<MockSnapshotProvider, Bitmap>(bitmapAct, testAdapter, testContext, false);
            
            Assert.AreEqual(@"C:\Test\TestFilesPath\Test\TestClass\TestName.png", testAdapter.LoadReferenceImage_filePath);
        }

        [TestMethod]
        public void TestSnapshotRecordingModeSeparateDirectoriesPerNamespace()
        {
            // Arrange
            var bitmapAct = new Bitmap(16, 16);
            var testContext = new MockTestContext
            {
                TestRunDirectory = "C:\\Test\\Artifacts",
                FullyQualifiedTestClassName = "Test.TestClass",
                TestName = "TestName"
            };
            var testAdapter = new MockTestAdapter
            {
                ReferenceImageExists_return = false,
                TestResultsSavePath_return = @"C:\Test\TestFilesPath"
            };

            // Act
            BitmapSnapshotTesting.SeparateDirectoriesPerNamespace = true;
            BitmapSnapshotTesting.Snapshot<MockSnapshotProvider, Bitmap>(bitmapAct, testAdapter, testContext, true);

            // Assert
            Assert.IsNotNull(testAdapter.AssertFailure_message);
            
            Assert.AreEqual(bitmapAct, testAdapter.SaveBitmapFile_bitmap);
            Assert.AreEqual(@"C:\Test\TestFilesPath\Test\TestClass\TestName.png", testAdapter.SaveBitmapFile_path);

            Assert.IsNull(testAdapter.SaveComparisonBitmapFiles_expected);
            Assert.IsNull(testAdapter.SaveComparisonBitmapFiles_expectedPath);
            Assert.IsNull(testAdapter.SaveComparisonBitmapFiles_actual);
            Assert.IsNull(testAdapter.SaveComparisonBitmapFiles_actualPath);
        }

        internal class MockSnapshotProvider : ISnapshotProvider<Bitmap>
        {
            public Bitmap GenerateBitmap(Bitmap context)
            {
                return context;
            }
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        internal class MockTestAdapter : IBitmapSnapshotTestAdapter
        {
            public bool ReferenceImageExists_return = true;

            public Bitmap LoadReferenceImage_return = new Bitmap(1, 1);
            public string LoadReferenceImage_filePath;

            public string AssertFailure_message;

            public string TestResultsSavePath_return;

            public Bitmap SaveComparisonBitmapFiles_expected;
            public string SaveComparisonBitmapFiles_expectedPath;
            public Bitmap SaveComparisonBitmapFiles_actual;
            public string SaveComparisonBitmapFiles_actualPath;
            
            public Bitmap SaveBitmapFile_bitmap;
            public string SaveBitmapFile_path;

            public void AssertFailure(string message)
            {
                AssertFailure_message = message;
            }
            
            public string TestResultsSavePath()
            {
                return TestResultsSavePath_return;
            }

            public bool ReferenceImageExists(string filePath)
            {
                return ReferenceImageExists_return;
            }

            public Bitmap LoadReferenceImage(string filePath)
            {
                LoadReferenceImage_filePath = filePath;
                return LoadReferenceImage_return;
            }

            public void SaveComparisonBitmapFiles(Bitmap expected, string expectedPath, Bitmap actual, string actualPath)
            {
                SaveComparisonBitmapFiles_expected = expected;
                SaveComparisonBitmapFiles_expectedPath = expectedPath;
                SaveComparisonBitmapFiles_actual = actual;
                SaveComparisonBitmapFiles_actualPath = actualPath;
            }

            public void SaveBitmapFile(Bitmap bitmap, string path)
            {
                SaveBitmapFile_bitmap = bitmap;
                SaveBitmapFile_path = path;
            }
        }
        
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        internal class MockTestContext: ITestContext
        {
            public string AddResultFile_fileName;

            public string TestName { get; set; } = "TestName";
            public string TestRunDirectory { get; set; } = "C:\\Path";
            public string FullyQualifiedTestClassName { get; set; } = "Test.TestClass";

            public void AddResultFile(string fileName)
            {
                AddResultFile_fileName = fileName;
            }
        }
    }
}
