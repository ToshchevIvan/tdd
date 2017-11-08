using System.Drawing;
using NUnit.Framework;
using TagsCloudVisualization;


namespace TagsCloudVizualizationTests
{
    // ReSharper disable once InconsistentNaming
    public class DrawingExtensions_should
    {
        private static readonly TestCaseData[] CalculateDistanceBetweenPointsCases = 
        {
            new TestCaseData(new Point(5, 5), new Point(5, 5))
                .Returns(0d)
                .SetName("When point are equal"),
            new TestCaseData(new Point(-1, -1), new Point(-1, -1))
                .Returns(0d)
                .SetName("When points are equal and have negative coords"),
            new TestCaseData(new Point(7, 13), new Point(4, 9))
                .Returns(5d)
                .SetName("When points are different"),
            new TestCaseData(new Point(-2, 9), new Point(1, 13))
                .Returns(5d)
                .SetName("When points are different and have negative coords")
        };
        
        [Test, TestCaseSource(nameof(CalculateDistanceBetweenPointsCases))]
        public double CalculateDistance_BetweenPoints(Point from, Point to)
        {
            return from.DistanceTo(to);
        }


        private static readonly TestCaseData[] CalculateRectangleAreaCases =
        {
            new TestCaseData(Rectangle.Empty)
                .Returns(0)
                .SetName("When rectangle is empty"), 
            new TestCaseData(new Rectangle(0, 0, 5, 4))
                .Returns(20)
                .SetName("When rectangle is not empty"),
            new TestCaseData(new Rectangle(100, 100, 1, 15))
                .Returns(15)
                .SetName("When rectangle is not in origin")
        };

        [Test, TestCaseSource(nameof(CalculateRectangleAreaCases))]
        public int CalculateRectangleArea(Rectangle rectangle)
        {
            return rectangle.Area();
        }


        private static readonly TestCaseData[] GetCenterOfRectangleCases =
        {
            new TestCaseData(Rectangle.Empty)
                .Returns(new Point(0, 0))
                .SetName("When rectangle is empty"), 
            new TestCaseData(new Rectangle(0, 0, 50, 10))
                .Returns(new Point(25, 5))
                .SetName("When rectangle is not empty"),
            new TestCaseData(new Rectangle(100, 100, 20, 30))
                .Returns(new Point(110, 115))
                .SetName("When rectangle is not in origin"),
            new TestCaseData(new Rectangle(50, 30, 15, 7))
                .Returns(new Point(57, 33))
                .SetName("When rectangle sides have odd length")
        };

        [Test, TestCaseSource(nameof(GetCenterOfRectangleCases))]
        public Point GetCenterOfRectangle(Rectangle rectangle)
        {
            return rectangle.GetCenter();
        }
        
        
        private static readonly TestCaseData[] CreateRectangleWithCenterCases =
        {
            new TestCaseData(new Point(0, 0), new Size(10, 10))
                .Returns(new Point(-5, -5))
                .SetName("When center is in origin"), 
            new TestCaseData(new Point(50, 50), new Size(100, 100))
                .Returns(new Point(0, 0))
                .SetName("When center is not in origin"),
            new TestCaseData(new Point(20, 30), new Size(17, 21))
                .Returns(new Point(12, 20))
                .SetName("When rectangle sides have odd length")
        };

        [Test, TestCaseSource(nameof(CreateRectangleWithCenterCases))]
        public Point CreateRectangleWithCenter(Point center, Size size)
        {
            return DrawingExtensions.CreateRectangleWithCenter(center, size)
                .Location;
        }
        
        
        private static readonly TestCaseData[] MultiplyPointCases =
        {
            new TestCaseData(new Point(0, 0), 10)
                .Returns(new Point(0, 0))
                .SetName("When point has zero coords"), 
            new TestCaseData(new Point(50, 50), 0)
                .Returns(new Point(0, 0))
                .SetName("When scalar is zero"),
            new TestCaseData(new Point(5, 7), 2)
                .Returns(new Point(10, 14))
                .SetName("When point and scalar are positive"),
            new TestCaseData(new Point(4, 2), -3)
                .Returns(new Point(-12, -6))
                .SetName("When scalar is negative")
        };

        [Test, TestCaseSource(nameof(MultiplyPointCases))]
        public Point MultiplyPoint(Point point, int scalar)
        {
            return point.Multiply(scalar);
        }
        
        
        private static readonly TestCaseData[] RotatePointCases =
        {
            new TestCaseData(new Point(0, 0), 3)
                .Returns(new Point(0, 0))
                .SetName("When point has zero coords"), 
            new TestCaseData(new Point(50, 50), 0)
                .Returns(new Point(50, 0))
                .SetName("When angle is zero"),
            new TestCaseData(new Point(10, 8), 5)
                .Returns(new Point(2, -7))
                .SetName("When point and angle are positive")
        };

        [Test, TestCaseSource(nameof(RotatePointCases))]
        public Point RotatePoint(Point point, int angleInRadians)
        {
            return point.Rotate(angleInRadians);
        }
    }
}
