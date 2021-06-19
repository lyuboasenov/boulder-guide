﻿using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;

namespace BoulderGuide.Mobile.Forms.Services.Data {
   public abstract class FileBasedEntity : INotifyPropertyChanged {
      protected string relativePath;

      public event PropertyChangedEventHandler PropertyChanged;
      protected IDownloadService DownloadService { get; }

      protected FileBasedEntity(
         string relativePath,
         string remoteRootPath,
         string localRootPath) {

         DownloadService = (IDownloadService) Prism.PrismApplicationBase.Current.Container.CurrentScope.Resolve(typeof(IDownloadService));
         this.relativePath = relativePath ?? throw new ArgumentNullException(nameof(relativePath));
         RemoteRootPath = remoteRootPath ?? throw new ArgumentNullException(nameof(remoteRootPath));
         LocalRootPath = localRootPath ?? throw new ArgumentNullException(nameof(localRootPath));

         var dir = Directory.GetParent(LocalPath);
         if (!dir.Exists) {
            dir.Create();
         }
      }

      public virtual string RemotePath => GetRemotePath(relativePath);

      public virtual string LocalPath => GetLocalPath(relativePath);

      public bool ExistsLocally => File.Exists(LocalPath);

      public string RemoteRootPath { get; }
      public string LocalRootPath { get; }

      public virtual Task DownloadAsync(bool force = false) {
         if (!ExistsLocally || force) {
            return DownloadService.DownloadFile(RemotePath, LocalPath);
         } else {
            return Task.CompletedTask;
         }
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
