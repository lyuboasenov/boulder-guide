namespace BoulderGuide.DTOs {
   public class Track {
      public string Name { get; set; }
      public Location[] Locations { get; set; } = new Location[0];
   }
}
