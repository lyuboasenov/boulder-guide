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
      public string[] Videos { get; set; } = new string[0];

      [Newtonsoft.Json.JsonProperty(PropertyName = "Schemas")]
      public Topo[] Topos { get; set; } = new Topo[0];
   }
}
