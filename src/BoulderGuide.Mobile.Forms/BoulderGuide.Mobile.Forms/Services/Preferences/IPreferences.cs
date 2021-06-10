namespace BoulderGuide.Mobile.Forms.Services.Preferences {
   public interface IPreferences {
      int FilterMinDifficulty { get; set; }
      int FilterMaxDifficulty { get; set; }
      string FilterSearchTerm { get; set; }
      int GPSPollIntervalInSeconds { get; set; }
      RouteOrderBy RouteOrderByProperty { get; set; }
   }
}
