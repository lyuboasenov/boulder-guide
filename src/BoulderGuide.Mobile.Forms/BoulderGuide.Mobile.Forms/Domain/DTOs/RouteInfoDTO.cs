using BoulderGuide.DTOs;

namespace BoulderGuide.Mobile.Forms.Domain.DTOs {
   public class RouteInfoDTO {
      public string Name { get; set; }
      public string Index { get; set; }
      public string[] Images { get; set; }
      public double Difficulty { get; set; }

      public Location Location { get; set; }

      public string Grade {
         get {
            return new Grade(Difficulty).ToString();
         }
      }

      public override string ToString() {
         return $"Route: '{Name} ({Grade})'";
      }
   }
}