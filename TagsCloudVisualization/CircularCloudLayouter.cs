using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace TagsCloudVisualization
{
    //CR(epeshk): избавиться от do {...} while(...); циклов в решении
    public class CircularCloudLayouter : ICloudLayouter
    {
        //CR(epeshk): зачем этот список нужно в публичном интерфейсе?
        public List<Rectangle> PlacedRectangles { get; } = new List<Rectangle>();
        private readonly Point center;
        private readonly IEnumerator<Point> spiralEnumerator;

        public CircularCloudLayouter(Point center)
        {
            if (center.X < 0 || center.Y < 0)
                throw new ArgumentException($"Center point must have positive coordinates. Actual is {center}");
            this.center = center;
            spiralEnumerator = GenerateSpiral(center).GetEnumerator();
        }

        public Rectangle PutNextRectangle(Size rectangleSize)
        {
            var rectangle = Rectangle.Empty;
            do
            {
                //CR(epeshk): попробовать избавиться от явного использования енумератора
                spiralEnumerator.MoveNext();
                var nextLocation = spiralEnumerator.Current;
                rectangle = DrawingExtensions.GetRectangleWithCenter(nextLocation, rectangleSize);
            } while (IntersectsWithOthers(rectangle));
            rectangle = MoveRectangleCloserToCenter(rectangle);
            PlacedRectangles.Add(rectangle);
            
            return rectangle;
        }

        private static IEnumerable<Point> GenerateSpiral(Point center)
        {
            var angle = 0;
            while (true)
            {
                //CR(epeshk): на самом деле это поворот, умножение на константу и сложение векторов. Попробовать избавиться от явных покоординатных действий с Point'ами
                yield return new Point(
                    (int) Math.Abs(angle * Math.Cos(angle) + center.X),
                    (int) Math.Abs(angle * Math.Sin(angle) + center.Y)
                );
                angle += 1; //CR(epeshk): выделить константу, добавить единицы измерения
            }
        }

        //CR(epeshk): как протестировать этот метод отдельно от остальных?
        private bool IntersectsWithOthers(Rectangle rectangle)
        {
            return PlacedRectangles.Any(r => r.IntersectsWith(rectangle));
        }

        private Rectangle MoveRectangleCloserToCenter(Rectangle rectangle)
        {
            var canMove = true;
            do
            {
                //CR(epeshk): нечитаемый метод
                var directionToCenter = center - (Size) rectangle.GetCenter();
                var movedByXRectangle = rectangle;
                if (!TryMove(ref movedByXRectangle, new Point(Math.Sign(directionToCenter.X), 0)))
                    movedByXRectangle = rectangle;
                var movedByYRectangle = movedByXRectangle;
                if (!TryMove(ref movedByYRectangle, new Point(0, Math.Sign(directionToCenter.Y))))
                    movedByYRectangle = movedByXRectangle;
                if (movedByYRectangle == rectangle)
                    canMove = false;
                else
                    rectangle = movedByYRectangle;
            } while (canMove);

            return rectangle;
        }

        private bool TryMove(ref Rectangle rectangle, Point offset)
        {
            rectangle.Offset(offset);
            
            return !IntersectsWithOthers(rectangle);
        }
    }
}
