using BoulderGuide.Domain.Schema;
using SkiaSharp;

namespace BoulderGuide.Domain.Entities {
   public class Ellipse : Shape {
      public RelativePoint Center { get; set; } = new RelativePoint();
      public RelativePoint Radius { get; set; } = new RelativePoint();

      public override void Draw(SKCanvas canvas, Size imageSize) {
         if (null != Center && null != Radius) {
            canvas.DrawEllipse(Center.ToImagePoint(imageSize), Radius.ToImagePoint(imageSize));
         }
      }
   }
}
