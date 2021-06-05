using ClimbingMap.Domain.Entities;
using ClimbingMap.Mobile.Forms.Services.Data;
using Mapsui;

namespace ClimbingMap.Mobile.Forms.Services.Maps {
   public interface IMapService {
      Map GetMap(Area area, AreaInfo info);
      Map GetMap(Route route, AreaInfo info);
   }
}
