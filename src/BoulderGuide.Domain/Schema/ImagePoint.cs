using System;

namespace BoulderGuide.Domain.Schema {
   public class ImagePoint {
      private double y;
      private double x;

      public double X {
         get { return x; }
         set {
            if (0 > value) {
               throw new ArgumentException($"{value} is not valid. X should be greater than 0.");
            }
            x = value;
         }
      }
      public double Y {
         get { return y; }
         set {
            if (0 > value) {
               throw new ArgumentException($"{value} is not valid. Y should be greater than 0.");
            }
            y = value;
         }
      }
   }
}
