using System.Collections.Generic;
using System.Drawing;
using System.Linq;


namespace TagsCloudVisualization
{
    public class CloudPainter
    {
        private readonly Bitmap bitmap;
        private readonly Graphics graphics;

        private Pen pen = new Pen(Color.ForestGreen, 2);

        public CloudPainter(Size canvasSize)
        {
            bitmap = new Bitmap(canvasSize.Width, canvasSize.Height);
            graphics = Graphics.FromImage(bitmap);
            graphics.FillRectangle(Brushes.Bisque, 
                new Rectangle(0, 0, canvasSize.Width, canvasSize.Height));
        }

        public CloudPainter PaintRectangles(IEnumerable<Rectangle> rectangles)
        {
            graphics.DrawRectangles(pen, rectangles.ToArray());
            
            return this;
        }

        public CloudPainter SaveToFile(string filePath)
        {
            bitmap.Save(filePath);
            
            return this;
        }
    }
}
