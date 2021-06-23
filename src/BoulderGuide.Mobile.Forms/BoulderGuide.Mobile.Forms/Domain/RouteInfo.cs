using BoulderGuide.DTOs;
using BoulderGuide.Mobile.Forms.Domain.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BoulderGuide.Mobile.Forms.Domain {
   public class RouteInfo : FileBasedEntity {
      private Region region;
      private RouteInfoDTO dto;

      public RouteInfo(Region region, AreaInfo parent, RouteInfoDTO dto) :
         base(
            "index.json",
            region.RemoteRootPath,
            region.LocalRootPath,
            region.Access == RegionAccess.@private) {
         this.region = region;
         this.dto = dto;
         Parent = parent;
         Route = new Route(region, dto.Index);
      }

      public string Name => dto?.Name;
      public double Difficulty => dto?.Difficulty ?? 0;
      public Location Location => dto?.Location;
      public string Grade => dto?.Grade;

      public AreaInfo Parent { get; }
      public Route Route { get; }

      public Task LoadRouteAsync(bool force = false) {
         return Route.DownloadAsync(force);
      }

      public override async Task DownloadAsync(bool force = false) {
         var tasks = new List<Task>();
         await base.DownloadAsync(force).ConfigureAwait(false);
         await LoadRouteAsync(force).ConfigureAwait(false);
      }
   }
}
