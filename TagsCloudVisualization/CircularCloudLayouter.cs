using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;


namespace TagsCloudVisualization
{
    public class CircularCloudLayouter : ICloudLayouter
    {
        private const int SpiralStepInRadians = 1;

        private readonly Point center;
        private readonly IEnumerable<Point> spiral;
        private readonly List<Rectangle> placedRectangles = new List<Rectangle>();

        public CircularCloudLayouter(Point center)
        {
            if (center.X < 0 || center.Y < 0)
                throw new ArgumentException($"Center point must have positive coordinates. Actual is {center}");
            this.center = center;
            spiral = GenerateSpiral(center);
        }

        public Rectangle PutNextRectangle(Size rectangleSize)
        {
            var rectangle = spiral
                .Select(location => DrawingExtensions.CreateRectangleWithCenter(location, rectangleSize))
                .First(r => !r.IntersectsWithAny(placedRectangles));
            rectangle = MoveRectangleCloserToCenter(rectangle);
            placedRectangles.Add(rectangle);

            return rectangle;
        }

        private static IEnumerable<Point> GenerateSpiral(Point center)
        {
            var angleInRadians = 0;
            var point = new Point(1, 1);
            var centerOffset = (Size) center;
            while (true)
            {
                yield return point.Multiply(angleInRadians)
                    .Rotate(angleInRadians) + centerOffset;
                angleInRadians += SpiralStepInRadians;
            }
        }

        private Rectangle MoveRectangleCloserToCenter(Rectangle rectangle)
        {
            var canMove = true;
            while (canMove)
            {
                var directionToCenter = center - (Size) rectangle.GetCenter();
                var offsetByX = new Point(Math.Sign(directionToCenter.X), 0);
                var offsetByY = new Point(0, Math.Sign(directionToCenter.Y));
                var movedRectangle = TryMove(rectangle, offsetByX);
                movedRectangle = TryMove(movedRectangle, offsetByY);
                if (movedRectangle == rectangle)
                    canMove = false;
                else
                    rectangle = movedRectangle;
            }

            return rectangle;
        }

        private Rectangle TryMove(Rectangle rectangle, Point offset)
        {
            var oldRectangle = rectangle;
            rectangle.Offset(offset);

            if (rectangle.IntersectsWithAny(placedRectangles))
                return oldRectangle;
            return rectangle;
        }
    }
}
