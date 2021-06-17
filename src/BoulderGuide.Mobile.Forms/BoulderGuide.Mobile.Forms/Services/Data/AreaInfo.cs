﻿namespace BoulderGuide.Mobile.Forms.Services.Data {
   public class AreaInfo : Info {
      public AreaInfo[] Areas { get; set; }
      public RouteInfo[] Routes { get; set; }
      public string[] Images { get; set; }

      public string Map { get; set; }

      public string MapLocalPath {
         get {
            return Region.GetLocalPath(Map);
         }
      }

      public string MapRemotePath {
         get {
            return Region.GetRemotePath(Map);
         }
      }


      public bool HasAreas {
         get {
            return (Areas?.Length ?? 0) > 0;
         }
      }

      public bool HasRoutes {
         get {
            return (Routes?.Length ?? 0) > 0;
         }
      }

      public override string ToString() {
         return $"Area: '{Name}'";
      }
   }
}