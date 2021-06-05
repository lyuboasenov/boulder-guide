using System;
using System.IO;

namespace ClimbingMap.Mobile.Forms.Services.Data {
   public abstract class Info {
      private string localRoot;
      private string remoteRoot;

      public string Name { get; set; }
      public string Index { get; set; }
      public bool IsOffline { get; set; }

      public string LocalRoot {
         get {
            return localRoot;
         }
      }
      public string RemoteRoot {
         get {
            return remoteRoot;
         }
      }

      public string LocalPath {
         get {
            return GetLocalPath(Index);
         }
      }

      public string RemotePath {
         get {
            return GetRemotePath(Index);
         }
      }

      public void SetRoots(string remoteRoot, string localRoot) {
         this.localRoot = localRoot;
         this.remoteRoot = remoteRoot;
      }

      protected string GetRemotePath(string relativePath) {
         if (string.IsNullOrEmpty(relativePath)) {
            return string.Empty;
         }
         return remoteRoot.Trim('/') + "/" + relativePath.Trim('/');
      }

      protected string GetLocalPath(string relativePath) {
         if (string.IsNullOrEmpty(relativePath)) {
            return string.Empty;
         }
         return Path.Combine(localRoot, relativePath.Trim('/'));
      }
   }
}
