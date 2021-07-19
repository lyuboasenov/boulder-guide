using BoulderGuide.DTOs;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoulderGuide.Mobile.Forms.Domain {
   public class Route : FileBasedEntity {
      private Region region;
      private RouteDTO dto;

      public Route(Region region, string index) :
         base(
            index,
            region.RemoteRootPath,
            region.LocalRootPath,
            region.Access == RegionAccess.@private) {

         this.region = region;
      }

      public IEnumerable<Image> Images { get; private set; }
      public string Info => dto?.Info;
      public string EightALink => dto?.EightALink;
      public IEnumerable<Topo> Topos => dto?.Topos;
      public IEnumerable<Video> Videos => dto?.Videos;
      public bool IsInitialized => dto != null;


      public override async Task DownloadAsync(bool force = false) {
         await base.DownloadAsync(force);

         dto = JsonConvert.DeserializeObject<RouteDTO>(GetAllText(), Shape.StandardJsonConverter);
         var relativePathNoFile = relativePath.Substring(0, relativePath.LastIndexOf('/'));

         Images = dto.Topos?.Select(s => new Image(region, $"{relativePathNoFile}/{s.Id}"));

         var list = new List<Task>();
         foreach (var image in Images ?? Enumerable.Empty<Image>()) {
            list.Add(image.DownloadAsync(force));
         }

         await Task.WhenAll(list);
      }
   }
}