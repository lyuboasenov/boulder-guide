using System;

namespace BoulderGuide.Mobile.Forms.Services.Location {
   public class LocationUpdatedEventArgs : EventArgs {
      public double Latitude { get; }
      public double Longitude { get; }

      public LocationUpdatedEventArgs(
         double latitude,
         double longitude) {
         Latitude = latitude;
         Longitude = longitude;
      }
   }
}