using BoulderGuide.DTOs;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoulderGuide.Mobile.Forms.Services.Data.Entities {
   public class Route : FileBasedEntity {
      private Region region;
      private RouteDTO dto;

      public Route(Region region, string index) :
         base (
            region.DownloadService,
            index,
            region.RemoteRootPath,
            region.LocalRootPath) {

         this.region = region;
      }

      public IEnumerable<Image> Images { get; private set; }


      public override async Task DownloadAsync() {
         await base.DownloadAsync();

         dto = JsonConvert.DeserializeObject<RouteDTO>(GetAllText());
         var relativePathNoFile = relativePath.Substring(0, relativePath.LastIndexOf('/'));

         Images = dto.Schemas?.Select(s => new Image(region, $"{relativePathNoFile}/{s.Id}"));

         var list = new List<Task>();
         foreach (var image in Images ?? Enumerable.Empty<Image>()) {
            list.Add(image.DownloadAsync());
         }

         await Task.WhenAll(list);
      }
   }
}