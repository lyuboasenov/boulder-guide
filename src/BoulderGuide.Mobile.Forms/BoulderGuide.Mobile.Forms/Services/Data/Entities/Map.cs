namespace BoulderGuide.Mobile.Forms.Services.Data.Entities {
   public class Map : FileBasedEntity {
      private Region region;
      private string map;

      public Map(Region region, string map) :
         base(
            map,
            region.RemoteRootPath,
            region.LocalRootPath,
            region.Access == RegionAccess.@private) {
         this.region = region;
         this.map = map;
      }
   }
}