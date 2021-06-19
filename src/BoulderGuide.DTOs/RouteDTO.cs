namespace BoulderGuide.DTOs {
   public class RouteDTO {
      public string Id { get; set; }
      public Location Location { get; set; } = new Location();
      public double Difficulty { get; set; }
      public Grade Grade { get { return new Grade(Difficulty); } }
      public string Name { get; set; }
      public string Info { get; set; }
      public string[] Tags { get; set; } = new string[0];
      public string[] Links { get; set; } = new string[0];
      public Schema[] Schemas { get; set; } = new Schema[0];
   }
}
