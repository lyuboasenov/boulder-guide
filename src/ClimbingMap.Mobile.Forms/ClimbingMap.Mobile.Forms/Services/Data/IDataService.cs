﻿using ClimbingMap.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ClimbingMap.Mobile.Forms.Services.Data {
   public interface IDataService {
      Task<IEnumerable<AreaInfo>> GetAreas();
      Task<IEnumerable<AreaInfo>> GetOfflineAreas();
      Task<Area> GetOfflineArea(AreaInfo info);
      Task<Area> GetArea(AreaInfo info);
      Task<Route> GetOfflineRoute(RouteInfo info);
      Task<Route> GetRoute(RouteInfo info);
      Task DownloadArea(AreaInfo info);
   }
}
