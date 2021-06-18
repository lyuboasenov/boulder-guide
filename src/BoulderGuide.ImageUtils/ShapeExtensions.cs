using BoulderGuide.DTOs;
using BoulderGuide.ImageUtils;
using SkiaSharp;
using System;
using System.Linq;

namespace BoulderGuide.ImageUtils {
   public static class ShapeExtensions {
      public static void Draw(this Shape shape, SKCanvas canvas, Size imageSize, Size offset) {
         if (shape is Ellipse ellipse) {
            ellipse.Draw(canvas, imageSize, offset);
         } else if (shape is Path path) {
            path.Draw(canvas, imageSize, offset);
         } else {
            throw new ArgumentException("unknown shape type.", nameof(shape));
         }
      }

      public static void Draw(this Ellipse ellipse, SKCanvas canvas, Size imageSize, Size offset) {
         if (null != ellipse && null != ellipse.Center && null != ellipse.Radius) {
            canvas.DrawEllipse(
               ellipse.Center.ToImagePoint(imageSize, offset),
               ellipse.Radius.ToImagePoint(imageSize, offset));
         }
      }

      public static void Draw(this Path path, SKCanvas canvas, Size imageSize, Size offset) {
         if (path?.Points?.Any() ?? false) {
            canvas.DrawPath(path.Points.Select(p => p.ToImagePoint(imageSize, offset)));
         }
      }
   }
}
