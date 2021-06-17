using System;
using System.IO;

namespace BoulderGuide.Mobile.Forms.Services.Data {
   public abstract class Info {
      public RegionInfo Region { get; set; }
      public string Name { get; set; }
      public string Index { get; set; }
      public bool IsOffline { get; set; }

      public string LocalPath {
         get {
            return Region.GetLocalPath(Index);
         }
      }

      public string RemotePath {
         get {
            return Region.GetRemotePath(Index);
         }
      }

      public string GetImageRemotePath(string relativePath) {
         return Region.GetRemotePath(relativePath);
      }
      public string GetImageLocalPath(string relativePath) {
         return Region.GetLocalPath(relativePath);
      }
   }
}
