using BoulderGuide.DTOs;
using System.Linq;
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

      public bool IsOffline {
         get {
            var result = true;

            if (!ExistsLocally) {
               result = false;
            }

            if (result && (Route?.Images?.Any(f => !f.ExistsLocally) ?? false)) {
               result = false;
            }
            if (result && (!Route?.ExistsLocally ?? true)) {
               result = false;
            }

            return result;
         }
      }

      public async Task LoadRouteAsync(bool force = false) {
         await Route.DownloadAsync(force).ConfigureAwait(false);
         OnPropertyChanged(nameof(Route));
      }

      public override Task DownloadAsync(bool force = false) {
         return LoadRouteAsync(force);
      }
   }
}
