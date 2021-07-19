namespace BoulderGuide.Mobile.Forms.Services.Preferences {
   internal class Preferences : IPreferences {
      private readonly Xamarin.Essentials.Interfaces.IPreferences preferences;

      public Preferences(Xamarin.Essentials.Interfaces.IPreferences preferences) {
         this.preferences = preferences;
      }

      public RouteOrderBy RouteOrderByProperty {
         get {
            return (RouteOrderBy) preferences.Get(nameof(RouteOrderByProperty), 0);
         }
         set {
            preferences.Set(nameof(RouteOrderByProperty), (int) value);
         }
      }

      public int FilterMinDifficulty {
         get {
            return preferences.Get(nameof(FilterMinDifficulty), 0);
         }
         set {
            preferences.Set(nameof(FilterMinDifficulty), value);
         }
      }
      public int FilterMaxDifficulty {
         get {
            return preferences.Get(nameof(FilterMaxDifficulty), 150);
         }
         set {
            preferences.Set(nameof(FilterMaxDifficulty), value);
         }
      }
      public string FilterSearchTerm {
         get {
            return preferences.Get(nameof(FilterSearchTerm), string.Empty);
         }
         set {
            preferences.Set(nameof(FilterSearchTerm), value);
         }
      }
      public bool ShowPrivateRegions {
         get {
            return preferences.Get(nameof(ShowPrivateRegions), false);
         }
         set {
            preferences.Set(nameof(ShowPrivateRegions), value);
         }
      }
      public string PrivateRegionsKey {
         get {
            return preferences.Get(nameof(PrivateRegionsKey), "");
         }
         set {
            preferences.Set(nameof(PrivateRegionsKey), value);
         }
      }

      public bool IsAdvancedModeEnabled {
         get {
            return preferences.Get(nameof(IsAdvancedModeEnabled), false);
         }
         set {
            preferences.Set(nameof(IsAdvancedModeEnabled), value);
         }
      }
      public bool IsDeveloperEnabled {
         get {
            return preferences.Get(nameof(IsDeveloperEnabled), false);
         }
         set {
            preferences.Set(nameof(IsDeveloperEnabled), value);
         }
      }

      public double LastKnownLatitude {
         get {
            return preferences.Get(nameof(LastKnownLatitude), 42.71725);
         }
         set {
            preferences.Set(nameof(LastKnownLatitude), value);
         }
      }

      public double LastKnownLongitude {
         get {
            return preferences.Get(nameof(LastKnownLongitude), 24.91746);
         }
         set {
            preferences.Set(nameof(LastKnownLongitude), value);
         }
      }

      public string TopoColorHex {
         get {
            return preferences.Get(nameof(TopoColorHex), "FFFFFF");
         }
         set {
            preferences.Set(nameof(TopoColorHex), value);
         }
      }
   }
}
