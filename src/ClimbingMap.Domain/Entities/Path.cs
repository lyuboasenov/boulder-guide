using ClimbingMap.Domain.Schema;
using SkiaSharp;
using System.Linq;

namespace ClimbingMap.Domain.Entities {
   public class Path : Shape {
      public RelativePoint[] Points { get; set; } = new RelativePoint[0];

      public override void Draw(SKCanvas canvas, Size imageSize) {
         if ((Points?.Length ?? 0) > 0) {
            canvas.DrawPath(Points.Select(p => p.ToImagePoint(imageSize)));
         }
      }
   }
}
