using BoulderGuide.Mobile.Forms.Services.Data.Entities;
using System;
using System.Threading.Tasks;

namespace BoulderGuide.Mobile.Forms.Services.Location {
   public interface ILocationService {
      event EventHandler<LocationUpdatedEventArgs> LocationUpdated;
      Mapsui.Map GetMap(AreaInfo info);
      Mapsui.Map GetMap(RouteInfo info);

      Task StartLocationPollingAsync();
      Task StopLocationPollingAsync();
   }
}
