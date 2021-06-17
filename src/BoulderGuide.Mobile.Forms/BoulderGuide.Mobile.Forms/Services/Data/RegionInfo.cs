using System.IO;

namespace BoulderGuide.Mobile.Forms.Services.Data {
   public class RegionInfo {
      [Newtonsoft.Json.JsonProperty(PropertyName = "urn")]
      public string Name { get; set; }
      public string Url { get; set; }
      public RegionAccess Access { get; set; }
      public bool Compressed { get; set; }
      public bool Encrypted { get; set; }


      [Newtonsoft.Json.JsonIgnore]
      public string LocalPath { get; set; }

      [Newtonsoft.Json.JsonIgnore]
      public string LocalIndexPath {
         get {
            return GetLocalPath("index.json");
         }
      }

      [Newtonsoft.Json.JsonIgnore]
      public string IndexUrl {
         get {
            return GetRemotePath("index.json");
         }
      }

      public string GetRemotePath(string relativePath) {
         if (string.IsNullOrEmpty(relativePath)) {
            return string.Empty;
         }
         return Url.Trim('/') + "/" + relativePath.Trim('/');
      }

      public string GetLocalPath(string relativePath) {
         if (string.IsNullOrEmpty(relativePath)) {
            return string.Empty;
         }
         return Path.Combine(LocalPath, relativePath.Trim('/'));
      }

   }
}
