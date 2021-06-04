namespace ClimbingMap.Mobile.Forms.Services.Data {
   public class AreaInfo : Info {
      public AreaInfo[] Areas { get; set; }
      public RouteInfo[] Routes { get; set; }


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
   }
}