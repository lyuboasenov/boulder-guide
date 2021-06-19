using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoulderGuide.Mobile.Forms.Services.Data.Entities {
   public class RouteInfo : FileBasedEntity {
      private Region region;
      private RouteInfoDTO dto;

      public RouteInfo(Region region, AreaInfo parent, RouteInfoDTO dto) :
         base(
            "index.json",
            region.RemoteRootPath,
            region.LocalRootPath) {
         this.region = region;
         this.dto = dto;
         Parent = parent;
      }

      public string Name => dto.Name;
      public double Difficulty => dto.Difficulty;
      public DTOs.Location Location => dto.Location;

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
