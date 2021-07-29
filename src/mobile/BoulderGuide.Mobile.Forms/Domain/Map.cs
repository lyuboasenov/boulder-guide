using BoulderGuide.DTOs;
using System.IO;

namespace BoulderGuide.Mobile.Forms.Domain {
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

      public string GetMapLocalFilePath() {
         if (IsPrivate) {
            var decryptedFileLocalPath = LocalPath + ".decrypted";
            using (var fileStream = File.Open(decryptedFileLocalPath, FileMode.Create))
            using (var decryptedStream = GetStream()) {
               decryptedStream.CopyTo(fileStream);
            }

            return decryptedFileLocalPath;
         } else {
            return LocalPath;
         }
      }
   }
}