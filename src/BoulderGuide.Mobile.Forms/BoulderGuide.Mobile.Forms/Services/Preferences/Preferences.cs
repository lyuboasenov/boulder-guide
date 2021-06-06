using System;
using System.Collections.Generic;
using System.Text;

namespace BoulderGuide.Mobile.Forms.Services.Preferences {
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
   }
}
