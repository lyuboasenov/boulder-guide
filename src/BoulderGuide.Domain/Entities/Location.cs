using System;
using System.Collections.Generic;
using System.Text;

namespace BoulderGuide.Domain.Entities {
   public class Location {
      public Location(double latitude, double longitude) {
         Latitude = latitude;
         Longitude = longitude;
      }

      public Location() {
      }

      /// <summary>
      /// from string
      /// </summary>
      /// <param name="latLonDD">N42.15519 E23.40990</param>
      public Location(string latLonDD) {
         if (string.IsNullOrEmpty(latLonDD)) {
            throw new ArgumentNullException(nameof(latLonDD));
         }

         var parts = latLonDD.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
         if (parts.Length != 2) {
            throw new ArgumentException(nameof(latLonDD));
         }

         if (!double.TryParse(parts[0].Substring(1), out double lat)) {
            throw new ArgumentException("invalid latitude");
         }
         Latitude = lat;

         if (!double.TryParse(parts[1].Substring(1), out double lon)) {
            throw new ArgumentException("invalid longitude");
         }
         Longitude = lon;
      }

      public double Latitude { get; set; }
      public double Longitude { get; set; }

      public override string ToString() {
         return $"N{Latitude} E{Longitude}";
      }
   }
}
