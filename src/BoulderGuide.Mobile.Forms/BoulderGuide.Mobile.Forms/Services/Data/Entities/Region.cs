﻿using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace BoulderGuide.Mobile.Forms.Services.Data.Entities {
   public class Region : FileBasedEntity {
      private RegionDTO dto;

      public Region(
         RegionDTO dto,
         string localPath) :
         base(
            "index.json",
            dto?.Url?.TrimEnd('/'),
            localPath) {
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
