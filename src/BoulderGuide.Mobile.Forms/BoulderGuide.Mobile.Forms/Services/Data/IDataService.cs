using BoulderGuide.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BoulderGuide.Mobile.Forms.Services.Data {
   public interface IDataService {
      Task ClearLocalStorage();
      Task<int> GetLocalStorageSizeInMB();
      Task<IEnumerable<AreaInfo>> GetIndexAreas(bool force);
      Task<Area> GetArea(AreaInfo info);
      Task<Route> GetRoute(RouteInfo info);
      Task DownloadArea(AreaInfo info);
   }
}
