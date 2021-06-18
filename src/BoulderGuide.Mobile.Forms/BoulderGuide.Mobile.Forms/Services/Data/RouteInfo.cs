using BoulderGuide.DTOs;

namespace BoulderGuide.Mobile.Forms.Services.Data {
   public class RouteInfo : Info {
      public string[] Images { get; set; }
      public double Difficulty { get; set; }

      public DTOs.Location Location { get; set; }

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