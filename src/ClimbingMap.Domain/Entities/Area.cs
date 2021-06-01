using System.Collections.Generic;

namespace ClimbingMap.Domain.Entities {
   public class Area {
      public string Id { get; set; }
      public string Name { get; set; }
      public string Info { get; set; }
      public string Approach { get; set; }
      public string Descent { get; set; }
      public string History { get; set; }
      public string Ethics { get; set; }
      public string Accomodations { get; set; }
      public string Restrictions { get; set; }
      public string[] Tags { get; set; } = new string[0];
      public Location[] Location { get; set; } = new Location[0];
      public string[] Links { get; set; } = new string[0];
   }
}
