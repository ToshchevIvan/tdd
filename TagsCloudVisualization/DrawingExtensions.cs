using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;


namespace TagsCloudVisualization
{
    public static class DrawingExtensions
    {
        public static Point GetCenter(this Rectangle rectangle)
        {
            return rectangle.Location +
                   new Size(rectangle.Width / 2, rectangle.Height / 2);
        }

        public static Rectangle CreateRectangleWithCenter(Point center, Size size)
        {
            return new Rectangle(
                center - new Size(size.Width / 2, size.Height / 2),
                size
            );
        }

        public static double DistanceTo(this Point from, Point to)
        {
            return Math.Sqrt(Math.Pow(from.X - to.X, 2) + Math.Pow(from.Y - to.Y, 2));
        }

        public static int Area(this Rectangle rectangle)
        {
            return rectangle.Width * rectangle.Height;
        }

        public static bool IntersectsWithAny(this Rectangle rectangle, IEnumerable<Rectangle> otherRectangles)
        {
            return otherRectangles.Any(r => r.IntersectsWith(rectangle));
        }

        public static Point Rotate(this Point point, int angleInRadians)
        {
            return new Point(
                (int) (point.X * Math.Cos(angleInRadians)),
                (int) (point.Y * Math.Sin(angleInRadians))
            );
        }

        public static Point Multiply(this Point point, int scalar)
        {
            return new Point(point.X * scalar, point.Y * scalar);
        }
    }
}
