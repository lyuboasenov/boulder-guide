using BoulderGuide.Domain.Entities;
using BoulderGuide.Mobile.Forms.Services.Data;
using Mapsui;

namespace BoulderGuide.Mobile.Forms.Services.Maps {
   public interface IMapService {
      Map GetMap(Area area, AreaInfo info);
      Map GetMap(Route route, AreaInfo info);
   }
}
