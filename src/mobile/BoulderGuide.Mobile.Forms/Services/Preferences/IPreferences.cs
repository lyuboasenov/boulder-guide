﻿namespace BoulderGuide.Mobile.Forms.Services.Preferences {
   public interface IPreferences {
      int FilterMinDifficulty { get; set; }
      int FilterMaxDifficulty { get; set; }
      string FilterSearchTerm { get; set; }
      bool ShowPrivateRegions { get; set; }
      RouteOrderBy RouteOrderByProperty { get; set; }
      string PrivateRegionsKey { get; set; }
      bool IsAdvancedModeEnabled { get; set; }
      bool IsDeveloperEnabled { get; set; }
      double LastKnownLatitude { get; set; }
      double LastKnownLongitude { get; set; }
      string TopoColorHex { get; set; }
   }
}
