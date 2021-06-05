using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Essentials.Interfaces;

namespace ClimbingMap.Mobile.Forms.Services.Data {
   internal class DataService : IDataService {
      private static readonly Dictionary<string, string>  areaAddresses = new Dictionary<string, string>() {
            {
               "rila-monastery", "https://raw.githubusercontent.com/lyuboasenov/climbing-map-rila-monastery/main"
            }
         };

      private readonly IFileSystem fileSystem;
      private readonly HttpClient httpClient = new HttpClient();

      public DataService(IFileSystem fileSystem) {
         this.fileSystem = fileSystem;
      }

      public Task<IEnumerable<AreaInfo>> GetAreas(bool force) {
         return GetAreas(true, force);
      }

      public Task<IEnumerable<AreaInfo>> GetOfflineAreas() {
         return GetAreas(false, false);
      }

      public Task<Domain.Entities.Area> GetOfflineArea(AreaInfo info) {
         return GetArea(info, false);
      }

      public Task<Domain.Entities.Area> GetArea(AreaInfo info) {
         return GetArea(info, true);
      }

      public Task<Domain.Entities.Route> GetOfflineRoute(RouteInfo info) {
         return GetRoute(info, false);
      }

      public Task<Domain.Entities.Route> GetRoute(RouteInfo info) {
         return GetRoute(info, true);
      }

      public async Task DownloadArea(AreaInfo info) {
         await GetArea(info, true);

         Parallel.ForEach(info.Areas ?? Enumerable.Empty<AreaInfo>(), async (i) => await DownloadArea(i));
         Parallel.ForEach(info.Routes ?? Enumerable.Empty<RouteInfo>(), async (i) => await DownloadRoute(i));
      }

      private async Task DownloadRoute(RouteInfo info) {
         await GetRoute(info, true);
      }

      private async Task<Domain.Entities.Route> GetRoute(RouteInfo info, bool download) {
         if (download) {
            var fileInfo = new FileInfo(info.LocalPath);

            if (!fileInfo.Directory.Exists) {
               fileInfo.Directory.Create();
            }

            await DownloadFile(info.RemotePath, info.LocalPath);

            foreach (var image in info.Images ?? Enumerable.Empty<string>()) {
               await DownloadFile(info.GetImageRemotePath(image), info.GetImageLocalPath(image));
            }
         }

         if (!File.Exists(info.LocalPath)) {
            throw new ArgumentException(Strings.AreaNotFoundOrCantBeDownloaded);
         }

         return JsonConvert.DeserializeObject<Domain.Entities.Route>(
            File.ReadAllText(info.LocalPath), Domain.Entities.Shape.StandardJsonConverter);
      }

      private async Task<Domain.Entities.Area> GetArea(AreaInfo info, bool download) {
         if (download) {
            var fileInfo = new FileInfo(info.LocalPath);

            if (!fileInfo.Directory.Exists) {
               fileInfo.Directory.Create();
            }

            await DownloadFile(info.RemotePath, info.LocalPath);
         }

         if (!File.Exists(info.LocalPath)) {
            throw new ArgumentException(Strings.AreaNotFoundOrCantBeDownloaded);
         }

         return JsonConvert.DeserializeObject<Domain.Entities.Area>(
            File.ReadAllText(info.LocalPath));
      }

      private async Task<IEnumerable<AreaInfo>> GetAreas(bool download, bool force) {
         var result = new List<AreaInfo>();
         foreach (var kv in areaAddresses) {
            var repoDir = Path.Combine(fileSystem.AppDataDirectory, kv.Key);

            if (!Directory.Exists(repoDir)) {
               Directory.CreateDirectory(repoDir);
            }

            var localIndexPath = Path.Combine(repoDir, "index.json");

            if (download && !File.Exists(localIndexPath) || force) {
               await DownloadFile(kv.Value + "/index.json", localIndexPath);
            }

            if (!File.Exists(localIndexPath)) {
               throw new ArgumentException(Strings.AreaNotFoundOrCantBeDownloaded);
            }

            var index = JsonConvert.DeserializeObject<AreaInfo>(File.ReadAllText(localIndexPath));
            index.SetRoots(kv.Value, repoDir);

            if (!string.IsNullOrEmpty(index.Map)) {
               if (download && !File.Exists(index.MapLocalPath) || force) {
                  await DownloadFile(index.MapRemotePath, index.MapLocalPath);
               }
            }

            SetIsOfflineAndRoots(index, kv.Value, repoDir);
            index.IsOffline = true;

            result.Add(index);
         }

         return result;
      }

      private void SetIsOfflineAndRoots(AreaInfo index, string remoteRoot, string localRoot) {

         index.SetRoots(remoteRoot, localRoot);
         bool isOffline = File.Exists(index.LocalPath) && File.Exists(index.MapLocalPath);

         foreach (var area in index.Areas ?? Enumerable.Empty<AreaInfo>()) {
            SetIsOfflineAndRoots(area, remoteRoot, localRoot);
            isOffline = isOffline && area.IsOffline;
         }

         foreach (var routeInfo in index.Routes ?? Enumerable.Empty<RouteInfo>()) {
            routeInfo.SetRoots(remoteRoot, localRoot);
            routeInfo.IsOffline = File.Exists(routeInfo.LocalPath);

            foreach (var img in routeInfo.Images ?? Enumerable.Empty<string>()) {
               routeInfo.IsOffline = routeInfo.IsOffline && File.Exists(routeInfo.GetImageLocalPath(img));
            }
            isOffline = isOffline && routeInfo.IsOffline;
         }

         index.IsOffline = isOffline;
      }

      private async Task DownloadFile(string remotePath, string localPath) {
         using (var response = await httpClient.GetAsync(remotePath)) {
            response.EnsureSuccessStatusCode();

            using (var stream = File.Open(localPath, FileMode.Create)) {
               await response.Content.CopyToAsync(stream);
            }
         }
      }
   }
}
