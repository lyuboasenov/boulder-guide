using System;
using System.Collections.Generic;
using System.Text;

namespace ClimbingMap.Domain.Entities {
   public class Route {
      public string Id { get; set; }
      public Location Location { get; set; }
      public double Difficulty { get; set; }
      public Grade Grade { get { return new Grade(Difficulty); } }
      public float Rating { get; set; }
      public ushort Length { get; set; }
      public string Name { get; set; }
      public string Info { get; set; }
      public string Approach { get; set; }
      public string History { get; set; }
      public ICollection<SchemaPoint> Topo { get; } = new HashSet<SchemaPoint>();
      public ICollection<string> Tags { get; } = new HashSet<string>();
   }
}
