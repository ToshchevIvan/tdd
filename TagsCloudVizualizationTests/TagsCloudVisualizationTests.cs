using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using NUnit.Framework;
using FluentAssertions;
using NUnit.Framework.Interfaces;
using TagsCloudVisualization;


namespace TagsCloudVizualizationTests
{
    [TestFixture]
    // ReSharper disable once InconsistentNaming
    public class CircularCloudLayouter_should
    {
        private const int TestRepeatCount = 10;
        private const int CanvasSideLength = 2500;

        private ICloudLayouter layouter;

        [SetUp]
        public void SetUp()
        {
            layouter = RandomEntitiesFabric.GetRandomLayouter(CanvasSideLength);
        }

        [TearDown]
        public void TearDown()
        {
            if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Passed ||
                layouter.PlacedRectangles.Count == 0)
                return;
            var name = TestContext.CurrentContext.Test.Name;
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, $"{name}.bmp");
            new CloudPainter(new Size(CanvasSideLength, CanvasSideLength))
                .PaintRectangles(layouter.PlacedRectangles)
                .SaveToFile(path);
            Console.WriteLine($"Tag cloud visualization saved to file {path}");
        }

        [Test]
        public void Throw_WhenSuppliedWithIncorrectCenter()
        {
            Action act = () => new CircularCloudLayouter(new Point(-10, -10));

            act.ShouldThrow<ArgumentException>();
        }

        [Test]
        public void HaveEmptyLayout_AfterCreation()
        {
            layouter.PlacedRectangles
                .Should()
                .BeEmpty();
        }

        [Test]
        public void PlaceFirstRectangle_IntoCenter()
        {
            var center = new Point(CanvasSideLength / 2, CanvasSideLength / 2);
            var layouter = new CircularCloudLayouter(center);
            var size = new Size(100, 50);

            var firstRectangle = layouter.PutNextRectangle(size);

            firstRectangle.GetCenter()
                .Should()
                .Be(center);
        }

        private List<Rectangle> PlaceSeveralRandomRectangles(int rectanglesCount)
        {
            var rectangles = new List<Rectangle>();
            for (var i = 0; i < rectanglesCount; i++)
            {
                rectangles.Add(layouter.PutNextRectangle(RandomEntitiesFabric.GetRandomSize()));
            }

            return rectangles;
        }

        [TestCase(2)]
        [TestCase(10)]
        [TestCase(100)]
        public void BeAble_ToPlaceSeveralRectangles(int count)
        {
            PlaceSeveralRandomRectangles(count);

            layouter.PlacedRectangles
                .Should()
                .HaveCount(count);
        }

        [Test]
        [Repeat(TestRepeatCount)]
        public void NotPlace_IntersectingRectangles()
        {
            var rectangles = PlaceSeveralRandomRectangles(
                RandomEntitiesFabric.GetRandomInt(10, 30));

            var intersections = rectangles.Select((rect, index) => new {rect, index})
                .SelectMany(r => rectangles.Skip(r.index + 1),
                    (r1, r2) => r1.rect.IntersectsWith(r2));

            intersections.Any(b => b)
                .Should()
                .BeFalse();
        }

        private double GetLayoutRadius(List<Rectangle> rectangles)
        {
            var center = rectangles[0].GetCenter();

            return rectangles.Select(r => r.Location.DistanceTo(center))
                .Max();
        }

        [Test]
        [Repeat(TestRepeatCount)]
        public void GenerateDenseLayout()
        {
            const double margin = 0.5;

            var rectangles = PlaceSeveralRandomRectangles(
                RandomEntitiesFabric.GetRandomInt(40, 70));
            var radius = GetLayoutRadius(rectangles);
            var expectedArea = radius * radius * Math.PI;
            var actualArea = rectangles.Select(r => r.Width * r.Height)
                .Sum();

            Math.Min(expectedArea / actualArea, actualArea / expectedArea).Should()
                .BeGreaterOrEqualTo(margin);
        }

        // TODO: не совсем понятно, как проверить, что облако круглое
        [Test]
        [Repeat(TestRepeatCount)]
        [Explicit]
        public void GenerateLayout_ThatIsCloseToCircular()
        {
            var rectangles = PlaceSeveralRandomRectangles(
                RandomEntitiesFabric.GetRandomInt(30, 80));
            var radius = GetLayoutRadius(rectangles);

            Assert.Fail();
        }
    }

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
                GetRandomPoint(DrawingExtensions.GetRectangleWithCenter(center, size)));
        }

        public static Size GetRandomSize()
        {
            var widthToHeightRatio = Randomizer.Next(MinWidthToHeightRatio, MaxWidthToHeightRatio);
            var height = Randomizer.Next(MinHeight, MaxHeight);
            var width = height * widthToHeightRatio;

            return new Size(width, height);
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
