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
        private List<Rectangle> currentLayout;

        [SetUp]
        public void SetUp()
        {
            layouter = RandomEntitiesFabric.GetRandomLayouter(CanvasSideLength);
            currentLayout = new List<Rectangle>();
        }

        [TearDown]
        public void TearDown()
        {
            if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Passed ||
                currentLayout.Count == 0)
                return;

            var name = TestContext.CurrentContext.Test.Name;
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, $"{name}.bmp");
            using (var painter = new CloudPainter(new Size(CanvasSideLength, CanvasSideLength)))
            {
                painter.PaintRectangles(currentLayout)
                    .SaveToFile(path);
            }
            Console.WriteLine($"Tag cloud visualization saved to file {path}");
        }

        [Test]
        public void Throw_WhenSuppliedWithIncorrectCenter()
        {
            Action act = () => new CircularCloudLayouter(new Point(-10, -10));

            act.ShouldThrow<ArgumentException>();
        }

        [Test]
        public void PlaceFirstRectangle_IntoCenter()
        {
            var center = new Point(CanvasSideLength / 2, CanvasSideLength / 2);
            var layouter = new CircularCloudLayouter(center);
            var size = new Size(100, 50);

            var firstRectangle = layouter.PutNextRectangle(size);
            currentLayout.Add(firstRectangle);

            firstRectangle.GetCenter()
                .Should()
                .Be(center);
        }

        [TestCase(2)]
        [TestCase(10)]
        [TestCase(200)]
        public void BeAble_ToPlaceSeveralRectangles(int count)
        {
            Action act = () => currentLayout = RandomEntitiesFabric.GetRandomLayout(layouter, count).ToList();
            
            act.ShouldNotThrow();
        }

        [Test]
        [Repeat(TestRepeatCount)]
        public void NotPlace_IntersectingRectangles()
        {
            currentLayout = RandomEntitiesFabric.GetRandomLayout(layouter,
                    RandomEntitiesFabric.GetRandomInt(10, 30))
                .ToList();

            var intersections = currentLayout.Select((rect, index) => new {rect, index})
                .SelectMany(r => currentLayout.Skip(r.index + 1),
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

            currentLayout = RandomEntitiesFabric.GetRandomLayout(layouter,
                    RandomEntitiesFabric.GetRandomInt(40, 70))
                .ToList();
            var radius = GetLayoutRadius(currentLayout);
            var coveringCircleArea = radius * radius * Math.PI;
            var actualArea = currentLayout.Select(r => r.Area())
                .Sum();

            Math.Min(coveringCircleArea / actualArea, actualArea / coveringCircleArea)
                .Should()
                .BeGreaterOrEqualTo(margin);
        }
    }
}
