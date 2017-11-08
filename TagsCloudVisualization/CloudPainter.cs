using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;


namespace TagsCloudVisualization
{
    public class CloudPainter : IDisposable
    {
        private readonly Bitmap bitmap;
        private readonly Graphics graphics;
        private readonly Pen pen = new Pen(Color.ForestGreen, 2);
        private bool disposed;

        public CloudPainter(Size canvasSize)
        {
            bitmap = new Bitmap(canvasSize.Width, canvasSize.Height);
            graphics = Graphics.FromImage(bitmap);
            graphics.FillRectangle(Brushes.Bisque, 
                new Rectangle(0, 0, canvasSize.Width, canvasSize.Height));
        }

        public CloudPainter PaintRectangles(IEnumerable<Rectangle> rectangles)
        {
            CheckDisposed();
            graphics.DrawRectangles(pen, rectangles.ToArray());
            
            return this;
        }

        public CloudPainter SaveToFile(string filePath)
        {
            CheckDisposed();
            bitmap.Save(filePath);
            
            return this;
        }

        private void CheckDisposed() 
        {
            if(disposed) 
                throw new ObjectDisposedException("Object has been already disposed");
        }
        
        public void Dispose()
        {
            disposed = true;
            bitmap.Dispose();
            graphics.Dispose();
            pen.Dispose();
        }
    }
}
