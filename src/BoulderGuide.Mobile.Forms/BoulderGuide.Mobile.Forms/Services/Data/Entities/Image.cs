﻿namespace BoulderGuide.Mobile.Forms.Services.Data.Entities {
   public class Image : FileBasedEntity {
      private Region region;

      public Image(Region region, string relativePath) :
         base (
            relativePath,
            region.RemoteRootPath,
            region.LocalRootPath) {
         this.region = region;
      }
   }
}