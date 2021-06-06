using BoulderGuide.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BoulderGuide.Mobile.Forms.Services.Data {
   public interface IDataService {
      Task RemoveLocalAreas();
      Task<IEnumerable<AreaInfo>> GetAreas(bool force);
      Task<IEnumerable<AreaInfo>> GetOfflineAreas();
      Task<Area> GetOfflineArea(AreaInfo info);
      Task<Area> GetArea(AreaInfo info);
      Task<Route> GetOfflineRoute(RouteInfo info);
      Task<Route> GetRoute(RouteInfo info);
      Task DownloadArea(AreaInfo info);
   }
}
