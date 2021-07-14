namespace BoulderGuide.DTOs {
   public class AreaDTO {
      public string Id { get; set; }
      public string Name { get; set; }
      public string Info { get; set; }
      public string Access { get; set; }
      public string History { get; set; }
      public string Ethics { get; set; }
      public string Accommodations { get; set; }
      public string Restrictions { get; set; }
      public string[] Tags { get; set; } = new string[0];
      public Location[] Location { get; set; } = new Location[0];
      public string[] Links { get; set; } = new string[0];
      public PointOfInterest[] POIs { get; set; } = new PointOfInterest[0];
      public Track[] Tracks { get; set; } = new Track[0];
   }
}
