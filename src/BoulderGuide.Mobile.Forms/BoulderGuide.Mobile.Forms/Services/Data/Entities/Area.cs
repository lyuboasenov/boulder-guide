using BoulderGuide.DTOs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BoulderGuide.Mobile.Forms.Services.Data.Entities {
   public class Area : FileBasedEntity {
      private readonly Region region;
      private AreaDTO dto;

      public Area(Region region, string index) :
         base (
            region.DownloadService,
            index,
            region.RemoteRootPath,
            region.LocalRootPath) {
         this.region = region;
      }

      public IEnumerable<DTOs.Location> Location => dto?.Location;

      public override async Task DownloadAsync() {
         await base.DownloadAsync();

         dto = JsonConvert.DeserializeObject<AreaDTO>(GetAllText());
      }
   }
}
