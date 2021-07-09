using BoulderGuide.DTOs;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BoulderGuide.Mobile.Forms.Domain {
   public class Area : FileBasedEntity {
      private readonly Region region;
      private AreaDTO dto;

      public Area(Region region, string index) :
         base(
            index,
            region.RemoteRootPath,
            region.LocalRootPath,
            region.Access == RegionAccess.@private) {
         this.region = region;
      }

      public IEnumerable<Location> Location => dto?.Location;
      public string Info => dto?.Info;
      public string Access => dto?.Access;
      public string Accommodations => dto?.Accommodations;
      public string Ethics => dto?.Ethics;
      public string History => dto?.History;
      public string Restrictions => dto?.Restrictions;
      public bool IsInitialized => dto != null;

      public override async Task DownloadAsync(bool force = false) {
         await base.DownloadAsync(force);

         dto = JsonConvert.DeserializeObject<AreaDTO>(GetAllText());
      }
   }
}
