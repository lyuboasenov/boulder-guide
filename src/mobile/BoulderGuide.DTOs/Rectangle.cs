using System;

namespace BoulderGuide.DTOs {
   public class Rectangle : Ellipse {
      public RelativePoint Location {
         get {
            return new RelativePoint() {
               X = Center.X - Width / 2,
               Y = Center.Y - Height / 2
            };
         }
      }

      public double Width {
         get {
            var width = Math.Abs(Radius.X - Center.X) * 2;
            width = Math.Min(width, 1);
            return Math.Max(width, 0);
         }
      }

      public double Height {
         get {
            var height = Math.Abs(Radius.Y - Center.Y) * 2;
            height = Math.Min(height, 1);
            return Math.Max(height, 0);
         }
      }
   }
}
