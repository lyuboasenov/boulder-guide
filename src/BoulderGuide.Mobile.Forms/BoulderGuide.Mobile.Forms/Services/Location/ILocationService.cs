using BoulderGuide.Domain.Entities;
using BoulderGuide.Mobile.Forms.Services.Data;
using Mapsui;
using System;
using System.Threading.Tasks;

namespace BoulderGuide.Mobile.Forms.Services.Location {
   public interface ILocationService {
      event EventHandler<LocationUpdatedEventArgs> LocationUpdated;
      Map GetMap(Area area, AreaInfo info);
      Map GetMap(Route route, AreaInfo info);

      Task StartLocationPollingAsync();
      Task StopLocationPollingAsync();
   }
}
