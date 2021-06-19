namespace BoulderGuide.Mobile.Forms.Services.Data.Entities {
   public class Map : FileBasedEntity {
      private Region region;
      private string map;

      public Map(Region region, string map) :
         base(
            region.DownloadService,
            map,
            region.RemoteRootPath,
            region.LocalRootPath) {
         this.region = region;
         this.map = map;
      }
   }
}