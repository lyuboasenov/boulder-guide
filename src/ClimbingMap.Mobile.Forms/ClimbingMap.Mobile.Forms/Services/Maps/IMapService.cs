using ClimbingMap.Domain.Entities;
using ClimbingMap.Mobile.Forms.Services.Data;
using Mapsui;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClimbingMap.Mobile.Forms.Services.Maps {
   public interface IMapService {
      Map GetMap(Area area, AreaInfo info);
      Map GetMap(Route route, RouteInfo info);
   }
}
