using System;
using System.Collections.Generic;
using System.Text;

namespace BoulderGuide.Mobile.Forms.Services.Preferences {
   public interface IPreferences {
      int GPSPollIntervalInSeconds { get; set; }
   }
}
