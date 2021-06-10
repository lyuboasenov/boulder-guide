﻿namespace BoulderGuide.Mobile.Forms.Services.Preferences {
   internal class Preferences : IPreferences {
      private readonly Xamarin.Essentials.Interfaces.IPreferences preferences;

      public Preferences(Xamarin.Essentials.Interfaces.IPreferences preferences) {
         this.preferences = preferences;
      }

      public int GPSPollIntervalInSeconds {
         get {
            return preferences.Get(nameof(GPSPollIntervalInSeconds), 5);
         }
         set {
            preferences.Set(nameof(GPSPollIntervalInSeconds), value);
         }
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
   }
}
