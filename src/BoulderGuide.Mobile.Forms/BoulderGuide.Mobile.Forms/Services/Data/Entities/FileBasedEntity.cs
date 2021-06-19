using System;
using System.IO;
using System.Threading.Tasks;

namespace BoulderGuide.Mobile.Forms.Services.Data {
   public abstract class FileBasedEntity {
      protected string relativePath;
      public IDownloadService DownloadService { get; }

      protected FileBasedEntity(
         IDownloadService downloadService,
         string relativePath,
         string remoteRootPath,
         string localRootPath) {

         DownloadService = downloadService ?? throw new ArgumentNullException(nameof(downloadService));
         this.relativePath = relativePath ?? throw new ArgumentNullException(nameof(relativePath));
         RemoteRootPath = remoteRootPath ?? throw new ArgumentNullException(nameof(remoteRootPath));
         LocalRootPath = localRootPath ?? throw new ArgumentNullException(nameof(localRootPath));

         if (!Directory.Exists(localRootPath)) {
            Directory.CreateDirectory(localRootPath);
         }
      }

      public virtual string RemotePath => GetRemotePath(relativePath);

      public virtual string LocalPath => GetLocalPath(relativePath);

      public bool ExistsLocally => File.Exists(LocalPath);

      public string RemoteRootPath { get; }
      public string LocalRootPath { get; }

      public virtual Task DownloadAsync() {
         return DownloadService.DownloadFile(RemotePath, LocalPath);
      }

      protected string GetRemotePath(string relativePath) {
         if (string.IsNullOrEmpty(relativePath)) {
            return string.Empty;
         }
         return RemoteRootPath.Trim('/') + "/" + relativePath.Trim('/');
      }

      protected string GetLocalPath(string relativePath) {
         if (string.IsNullOrEmpty(relativePath)) {
            return string.Empty;
         }
         return Path.Combine(LocalRootPath, relativePath.Trim('/'));
      }


      public string GetAllText() {
         return File.ReadAllText(LocalPath);
      }

      protected byte[] GetAllBytes() {
         return File.ReadAllBytes(LocalPath);
      }
   }
}
