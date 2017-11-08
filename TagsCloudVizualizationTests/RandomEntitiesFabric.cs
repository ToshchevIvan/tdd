using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TagsCloudVisualization;


namespace TagsCloudVizualizationTests
{
    internal static class RandomEntitiesFabric
    {
        private const int MinHeight = 60;
        private const int MaxHeight = 120;
        private const int MinWidthToHeightRatio = 2;
        private const int MaxWidthToHeightRatio = 4;

        private static readonly Random Randomizer = new Random();

        public static int GetRandomInt(int min, int max)
        {
            return Randomizer.Next(min, max);
        }

        public static CircularCloudLayouter GetRandomLayouter(int canvasSideLength)
        {
            var halfLength = canvasSideLength / 2;
            var fourthLength = canvasSideLength / 4;
            var center = new Point(halfLength, halfLength);
            var size = new Size(fourthLength, fourthLength);

            return new CircularCloudLayouter(
                GetRandomPoint(DrawingExtensions.CreateRectangleWithCenter(center, size)));
        }

        public static Size GetRandomSize()
        {
            var widthToHeightRatio = Randomizer.Next(MinWidthToHeightRatio, MaxWidthToHeightRatio);
            var height = Randomizer.Next(MinHeight, MaxHeight);
            var width = height * widthToHeightRatio;

            return new Size(width, height);
        }

        private static IEnumerable<Size> GetRandomSizesSequence()
        {
            while (true)
            {
                yield return GetRandomSize();
            }
        }

        public static IEnumerable<Rectangle> GetRandomLayout(ICloudLayouter layouter, int rectCount)
        {
            return GetRandomSizesSequence()
                .Take(rectCount)
                .Select(layouter.PutNextRectangle);
        }

        public static Point GetRandomPoint(Rectangle boundingBox)
        {
            return new Point(
                Randomizer.Next(boundingBox.Left, boundingBox.Right),
                Randomizer.Next(boundingBox.Top, boundingBox.Bottom)
            );
        }
    }
}
