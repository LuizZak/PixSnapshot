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

using System.Drawing;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PixSnapshot
{
    /// <inheritdoc />
    /// <summary>
    /// A pre-implemented snapshot provider for Bitmap-comparison tests.
    /// </summary>
    public class BitmapSnapshot : ISnapshotProvider<Bitmap>
    {
        /// <summary>
        /// Whether tests are currently under record mode- under record mode, results are recorded on disk to be later
        /// compared when not in record mode.
        /// 
        /// Calls to Snapshot always fail with an assertion during record mode.
        /// 
        /// Defaults to false.
        /// </summary>
        public static bool RecordMode = false;
        
        public static void Snapshot([NotNull] Bitmap bitmap, [NotNull] TestContext context)
        {
            BitmapSnapshotTesting.Snapshot<BitmapSnapshot, Bitmap>(bitmap, new MsTestAdapter(), new MsTestContextAdapter(context), RecordMode);
        }

        public static void Snapshot([NotNull] Bitmap bitmap, [NotNull] IBitmapSnapshotTestAdapter testAdapter, [NotNull] ITestContext context)
        {
            BitmapSnapshotTesting.Snapshot<BitmapSnapshot, Bitmap>(bitmap, testAdapter, context, RecordMode);
        }

        public Bitmap GenerateBitmap(Bitmap context)
        {
            return context;
        }
    }
}
