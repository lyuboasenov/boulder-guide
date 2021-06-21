using BoulderGuide.DTOs;
using BoulderGuide.Mobile.Forms.Domain.DTOs;
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
      }

      public string Name => dto?.Name;
      public double Difficulty => dto?.Difficulty ?? 0;
      public Location Location => dto?.Location;
      public string Grade => dto?.Grade;

      public AreaInfo Parent { get; }
      public Route Route { get; private set; }

      public async Task LoadRouteAsync() {
         if (Route is null) {
            var route = new Route(region, dto.Index);
            await route.DownloadAsync();
            Route = route;
         }
      }
   }
}
