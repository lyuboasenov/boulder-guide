using BoulderGuide.Mobile.Forms.Domain.DTOs;
using System.IO;

namespace BoulderGuide.Mobile.Forms.Domain {
   public class Image : FileBasedEntity {
      private Region region;

      public Image(Region region, string relativePath) :
         base(
            relativePath,
            region.RemoteRootPath,
            region.LocalRootPath,
            region.Access == RegionAccess.@private) {
         this.region = region;
      }

      public new Stream GetStream() {
         return base.GetStream();
      }
   }
}