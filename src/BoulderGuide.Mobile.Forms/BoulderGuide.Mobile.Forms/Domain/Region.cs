using BoulderGuide.Mobile.Forms.Domain.DTOs;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace BoulderGuide.Mobile.Forms.Domain {
   public class Region : FileBasedEntity {
      private RegionDTO dto;

      public Region(
         RegionDTO dto,
         string localPath) :
         base(
            "index.json",
            dto?.Url?.TrimEnd('/'),
            localPath,
            dto.Access == RegionAccess.@private) {
         this.dto = dto ?? throw new ArgumentNullException(nameof(dto));
      }

      public RegionAccess Access => dto.Access;

      public async Task<AreaInfo> GetIndexAsync(bool force = false) {
         var dto = JsonConvert.DeserializeObject<AreaInfoDTO>(GetAllText());
         var index = new AreaInfo(this, null, dto);

         await index.DownloadImagesAsync(force);
         await index.DownloadMapAsync(force);

         return index;
      }
   }
}
