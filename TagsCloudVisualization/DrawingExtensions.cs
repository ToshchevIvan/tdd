using System;
using System.Drawing;


namespace TagsCloudVisualization
{
    public static class DrawingExtensions
    {
        public static Point GetCenter(this Rectangle rectangle)
        {
            return rectangle.Location + 
                   new Size(rectangle.Width / 2, rectangle.Height / 2);
        }

        public static Rectangle GetRectangleWithCenter(Point center, Size size)
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
    }
}
