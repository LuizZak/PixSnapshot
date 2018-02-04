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

using System;
using System.Drawing;
using System.IO;
using System.Linq;
using FastBitmapLib;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PixSnapshot
{
    /// <summary>
    /// Helper static class to perform bitmap-based rendering comparisons as assertions.
    /// </summary>
    public static class BitmapSnapshotTesting
    {
        /// <summary>
        /// If true, when generating output folders for test results, paths are created for each segment of the namespace
        /// of the target test class, e.g. 'PixUI.Controls.LabelControlViewTests' becomes '...\PixUI\Controls\LabelControlViewtests\',
        /// otherwise a single folder with the fully-qualified class name is used instead.
        /// 
        /// If this property is changed across test recordings, the tests must be re-recorded to account for the new directory paths
        /// expected by the snapshot class.
        /// 
        /// Defaults to false.
        /// </summary>
        public static bool SeparateDirectoriesPerNamespace = false;

        /// <summary>
        /// Performs a snapshot text with a given test context/object pair, using an instantiable snapshot provider.
        /// </summary>
        public static void Snapshot<TProvider, TObject>([NotNull] TObject source, [NotNull] IBitmapSnapshotTestAdapter testAdapter, [NotNull] ITestContext context, bool recordMode) where TProvider : ISnapshotProvider<TObject>, new()
        {
            var provider = new TProvider();

            Snapshot(provider, source, testAdapter, context, recordMode);
        }

        /// <summary>
        /// Performs a snapshot text with a given test context/object pair, using a given instantiated snapshot provider.
        /// </summary>
        public static void Snapshot<T>([NotNull] ISnapshotProvider<T> provider, [NotNull] T target, [NotNull] IBitmapSnapshotTestAdapter testAdapter, [NotNull] ITestContext context, bool recordMode)
        {
            string targetPath = CombinedTestResultPath(testAdapter.TestResultsSavePath(), context);
            
            string testFileName = context.TestName + ".png";
            string testFilePath = Path.Combine(targetPath, testFileName);

            // Verify comparison file's existence (if not in record mode)
            if (!recordMode)
            {
                if (!testAdapter.ReferenceImageExists(testFilePath))
                {
                    testAdapter.AssertFailure(
                        $"Could not find reference image file {testFilePath} to compare. Please re-run the test with {nameof(recordMode)} set to true to record a test result to compare later.");

                    return;
                }
            }
            
            var image = provider.GenerateBitmap(target);

            if (recordMode)
            {
                testAdapter.SaveBitmapFile(image, testFilePath);

                testAdapter.AssertFailure(
                    $"Saved image to path {testFilePath}. Re-run test mode with {nameof(recordMode)} set to false to start comparing with record test result.");
            }
            else
            {
                // Load recorded image and compare
                using (var expected = testAdapter.LoadReferenceImage(testFilePath))
                using (var expLock = expected.FastLock())
                using (var actLock = image.FastLock())
                {
                    bool areEqual = expLock.Width == actLock.Width && expLock.DataArray.SequenceEqual(actLock.DataArray);
                    
                    if (areEqual)
                        return; // Success!

                    // Save to test results directory for further inspection
                    string directoryName = CombinedTestResultPath(context.TestRunDirectory, context);
                    string baseFileName = Path.ChangeExtension(testFileName, null);

                    string savePathExpected = Path.Combine(directoryName, Path.ChangeExtension(baseFileName + "-expected", ".png"));
                    string savePathActual = Path.Combine(directoryName, Path.ChangeExtension(baseFileName + "-actual", ".png"));
                    
                    testAdapter.SaveComparisonBitmapFiles(expected, savePathExpected, image, savePathActual);

                    context.AddResultFile(savePathActual);

                    testAdapter.AssertFailure(
                        $"Resulted image did not match expected image. Inspect results under directory {directoryName} for info about results");
                }
            }
        }
        
        private static string CombinedTestResultPath([NotNull] string basePath, [NotNull] ITestContext context)
        {
            if(!SeparateDirectoriesPerNamespace)
                return Path.Combine(basePath, context.FullyQualifiedTestClassName);

            var segments = context.FullyQualifiedTestClassName.Split('.');

            return Path.Combine(new[] {basePath}.Concat(segments).ToArray());
        }
    }

    /// <summary>
    /// Base interface for objects instantiated to provide bitmaps for snapshot tests
    /// </summary>
    /// <typeparam name="T">The type of object this snapshot provider receives in order to produce snapshots.</typeparam>
    public interface ISnapshotProvider<in T>
    {
        /// <summary>
        /// Asks this snapshot provider to create a <see cref="T:System.Drawing.Bitmap"/> from a given object context.
        /// </summary>
        [NotNull]
        Bitmap GenerateBitmap([NotNull] T context);
    }
}
