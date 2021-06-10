using SkiaSharp;

namespace BoulderGuide.Domain.Schema {
   public class Ellipse : Shape {
      public RelativePoint Center { get; set; } = new RelativePoint();
      public RelativePoint Radius { get; set; } = new RelativePoint();

      public override void Draw(SKCanvas canvas, Size imageSize, Size offset) {
         if (null != Center && null != Radius) {
            canvas.DrawEllipse(
               Center.ToImagePoint(imageSize, offset),
               Radius.ToImagePoint(imageSize, offset));
         }
      }
   }
}
