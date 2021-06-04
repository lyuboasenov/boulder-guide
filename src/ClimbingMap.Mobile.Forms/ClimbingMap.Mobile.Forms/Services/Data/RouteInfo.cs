using ClimbingMap.Domain.Entities;

namespace ClimbingMap.Mobile.Forms.Services.Data {
   public class RouteInfo : Info {
      public string[] Images { get; set; }
      public double Difficulty { get; set; }

      public Location Location { get; set; }

      public string Grade {
         get {
            return new Grade(Difficulty).ToString();
         }
      }

      public string GetImageRemotePath(string relativePath) {
         return GetRemotePath(relativePath);
      }
      public string GetImageLocalPath(string relativePath) {
         return GetLocalPath(relativePath);
      }
   }
}