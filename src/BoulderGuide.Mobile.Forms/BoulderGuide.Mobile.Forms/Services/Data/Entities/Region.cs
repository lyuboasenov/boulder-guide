using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace BoulderGuide.Mobile.Forms.Services.Data.Entities {
   public class Region : FileBasedEntity {
      private RegionDTO dto;

      public Region(
         RegionDTO dto,
         string localPath,
         IDownloadService downloadService) :
         base(
            downloadService,
            "index.json",
            dto?.Url?.TrimEnd('/'),
            localPath) {
         this.dto = dto ?? throw new ArgumentNullException(nameof(dto));
      }

      public RegionAccess Access => dto.Access;

      internal Task DownloadAsync(string relativePath) {
         return DownloadService.DownloadFile(GetRemotePath(relativePath), GetLocalPath(relativePath));
      }

      public async Task<AreaInfo> GetIndexAsync() {
         var dto = JsonConvert.DeserializeObject<AreaInfoDTO>(GetAllText());
         var index = new AreaInfo(this, null, dto);

         await index.DownloadImagesAsync();
         await index.DownloadMapAsync();

         return index;
      }
   }
}
