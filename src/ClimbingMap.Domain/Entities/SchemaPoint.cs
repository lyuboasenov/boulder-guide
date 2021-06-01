using System;
using System.Collections.Generic;
using System.Text;

namespace ClimbingMap.Domain.Entities {
   public class SchemaPoint {
      public double X { get; set; }
      public double Y { get; set; }

      public SchemaPoint() {

      }

      public SchemaPoint(double x, double y) {
         X = x;
         Y = y;
      }
   }
}
