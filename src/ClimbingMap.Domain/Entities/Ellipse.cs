using System;
using System.Collections.Generic;
using System.Text;

namespace ClimbingMap.Domain.Entities {
   public class Ellipse : Shape {
      public SchemaPoint Center { get; set; } = new SchemaPoint();
      public SchemaPoint Radius { get; set; } = new SchemaPoint();
   }
}
