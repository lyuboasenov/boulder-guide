using System.IO;

namespace BoulderGuide.Mobile.Forms.Services.Data.Entities {
   public class Image : FileBasedEntity {
      private Region region;

      public Image(Region region, string relativePath) :
         base (
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