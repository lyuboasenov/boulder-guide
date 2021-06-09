namespace BoulderGuide.Mobile.Forms.Services.Preferences {
   public interface IPreferences {
      int GPSPollIntervalInSeconds { get; set; }
      RouteOrderBy RouteOrderByProperty { get; set; }
   }
}
