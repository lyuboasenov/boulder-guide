using BoulderGuide.DTOs;
using BoulderGuide.ImageUtils;
using SkiaSharp;
using System;
using System.Linq;

namespace BoulderGuide.ImageUtils {
   public static class ShapeExtensions {
      public static void Draw(this Shape shape, SKCanvas canvas, Size imageSize, Size offset, SKColor color, int strokeWidth = 5) {
         if (shape is Rectangle rect) {
            rect.Draw(canvas, imageSize, offset, color);
         } else if (shape is Ellipse ellipse) {
            ellipse.Draw(canvas, imageSize, offset, color, strokeWidth: strokeWidth);
         } else if (shape is Path path) {
            path.Draw(canvas, imageSize, offset, color, strokeWidth: strokeWidth);
         } else {
            throw new ArgumentException("unknown shape type.", nameof(shape));
         }
      }

      public static void Draw(this Rectangle rectangle, SKCanvas canvas, Size imageSize, Size offset, SKColor color) {
         if (null != rectangle && null != rectangle.Center && null != rectangle.Radius) {
            var paint = color.ToSKPaint();
            paint.Style = SKPaintStyle.StrokeAndFill;
            var loc = rectangle.Location.ToImagePoint(imageSize, offset);

            canvas.DrawRect(
               (float) loc.X,
               (float) loc.Y,
               (float) (rectangle.Width * imageSize.Width),
               (float) (rectangle.Height * imageSize.Height),
               paint);
         }
      }

      public static void Draw(this Ellipse ellipse, SKCanvas canvas, Size imageSize, Size offset, SKColor color, int strokeWidth = 5) {
         if (null != ellipse && null != ellipse.Center && null != ellipse.Radius) {
            canvas.DrawEllipse(
               ellipse.Center.ToImagePoint(imageSize, offset),
               ellipse.Radius.ToImagePoint(imageSize, offset), color, strokeWidth: strokeWidth);
         }
      }

      public static void Draw(this Path path, SKCanvas canvas, Size imageSize, Size offset, SKColor color, int strokeWidth = 5) {
         if (path?.Points?.Any() ?? false) {
            canvas.DrawPath(path.Points.Select(p => p.ToImagePoint(imageSize, offset)), color, strokeWidth: strokeWidth);
         }
      }
   }
}
