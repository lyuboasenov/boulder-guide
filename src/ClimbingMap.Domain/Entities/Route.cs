using System;
using System.Collections.Generic;
using System.Text;

namespace ClimbingMap.Domain.Entities {
   public class Route {
      public string Id { get; set; }
      public Location Location { get; set; } = new Location();
      public double Difficulty { get; set; }
      public Grade Grade { get { return new Grade(Difficulty); } }
      public float Rating { get; set; }
      public ushort Height { get; set; }
      public string Name { get; set; }
      public string Info { get; set; }
      public string Approach { get; set; }
      public string History { get; set; }
      public string[] Tags { get; set; } = new string[0];
      public string[] Links { get; set; } = new string[0];
      public Schema[] Schemas { get; set; } = new Schema[0];
   }
}
