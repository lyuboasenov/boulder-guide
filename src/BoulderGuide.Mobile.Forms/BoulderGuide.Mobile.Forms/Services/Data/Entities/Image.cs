namespace BoulderGuide.Mobile.Forms.Services.Data.Entities {
   public class Image : FileBasedEntity {
      private Region region;
      private string relativePath;

      public Image(Region region, string relativePath) :
         base (
            region.DownloadService,
            relativePath,
            region.RemoteRootPath,
            region.LocalRootPath) {
         this.region = region;
         this.relativePath = relativePath;
      }
   }
}