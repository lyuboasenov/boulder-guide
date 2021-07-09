namespace BoulderGuide.DTOs {
   public class AreaInfoDTO {

      public string Name { get; set; }
      public string Index { get; set; }
      public AreaInfoDTO[] Areas { get; set; }
      public RouteInfoDTO[] Routes { get; set; }
      public string[] Images { get; set; }

      public string Map { get; set; }

      public override string ToString() {
         return $"Area: '{Name}'";
      }
   }
}