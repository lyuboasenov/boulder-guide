﻿using BoulderGuide.Domain.Schema;
using BoulderGuide.Mobile.Forms.Services.Errors;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Essentials.Interfaces;

namespace BoulderGuide.Mobile.Forms.Services.Data {
   internal class DataService : IDataService {
#if DEBUG
      private const string masterIndexRemoteLocation = "https://raw.githubusercontent.com/lyuboasenov/boulder-guide/main/data/index-v2.debug.json";
#else
      private const string masterIndexRemoteLocation = "https://raw.githubusercontent.com/lyuboasenov/boulder-guide/main/data/index.json";
#endif
      private RegionInfo[] regions;

      private readonly IFileSystem fileSystem;
      private readonly HttpClient httpClient = new HttpClient();
      private readonly IConnectivity connectivity;
      private readonly IErrorService errorService;
      private readonly Preferences.IPreferences preferences;

      public DataService(
         IFileSystem fileSystem,
         IConnectivity connectivity,
         IErrorService errorService,
         Preferences.IPreferences preferences) {
         this.fileSystem = fileSystem;
         this.connectivity = connectivity;
         this.errorService = errorService;
         this.preferences = preferences;
      }

      public Task ClearLocalStorage() {
         try {
            var reposDir = System.IO.Path.Combine(fileSystem.AppDataDirectory, "repositories");
            Directory.Delete(reposDir, true);

            return Task.CompletedTask;
         } catch (Exception ex) {
            errorService.HandleError(ex);
            return Task.CompletedTask;
         }
      }

      public Task<int> GetLocalStorageSizeInMB() {
         try {
            var repoDir = System.IO.Path.Combine(fileSystem.AppDataDirectory, "repositories");
            DirectoryInfo info = new DirectoryInfo(repoDir);
            long size = 0;
            foreach (var file in info.GetFiles("*", SearchOption.AllDirectories)) {
               size += file.Length;
            }

            return Task.FromResult<int>((int) (size >> 20));
         } catch (Exception ex) {
            errorService.HandleError(ex);
            return Task.FromResult(0);
         }
      }

      public Task<IEnumerable<AreaInfo>> GetIndexAreas(bool force) {
         try {
            if (connectivity.NetworkAccess != NetworkAccess.Internet) {
               return GetAreas(false, force);
            } else {
               return GetAreas(true, force);
            }
         } catch (Exception ex) {
            errorService.HandleError(ex);
            return Task.FromResult<IEnumerable<AreaInfo>>(null);
         }
      }

      public Task<Domain.Entities.Area> GetArea(AreaInfo info) {
         try {
            OrderAreasRoutes(info);

            if (connectivity.NetworkAccess == NetworkAccess.Internet) {
               return GetArea(info, true);
            } else if (info.IsOffline) {
               return GetArea(info, false);
            } else {
               // TODO show message that it can't be downloaded
               return Task.FromResult<Domain.Entities.Area>(null);
            }
         } catch (Exception ex) {
            errorService.HandleError(ex);
            return Task.FromResult<Domain.Entities.Area>(null);
         }
      }

      public Task<Domain.Entities.Route> GetRoute(RouteInfo info) {
         try {
            if (connectivity.NetworkAccess == NetworkAccess.Internet) {
               return GetRoute(info, true);
            } else if (info.IsOffline) {
               return GetRoute(info, false);
            } else {
               // TODO show message that it can't be downloaded
               return Task.FromResult<Domain.Entities.Route>(null);
            }
         } catch (Exception ex) {
            errorService.HandleError(ex);
            return Task.FromResult<Domain.Entities.Route>(null);
         }
      }

      public async Task DownloadArea(AreaInfo info) {
         try {
            await GetArea(info, true);

            var tasks = new List<Task>();
            foreach (var i in info.Areas ?? Enumerable.Empty<AreaInfo>()) {
               tasks.Add(DownloadArea(i));
            }

            foreach (var i in info.Routes ?? Enumerable.Empty<RouteInfo>()) {
               tasks.Add(DownloadRoute(i));
            }

            await Task.WhenAll(tasks);

            SetIsOfflineAndRoots(info, info.Region);
         } catch (Exception ex) {
            errorService.HandleError(ex);
         }
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
            File.ReadAllText(info.LocalPath), Shape.StandardJsonConverter);
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

         var repositoriesDir = System.IO.Path.Combine(fileSystem.AppDataDirectory, "repositories");
         if (!Directory.Exists(repositoriesDir)) {
            Directory.CreateDirectory(repositoriesDir);
         }

         var masterIndexLocalPath = System.IO.Path.Combine(repositoriesDir, "index-v2.json");
         if (!File.Exists(masterIndexLocalPath) || download && force) {
            await DownloadFile(masterIndexRemoteLocation, masterIndexLocalPath);
         }

         regions =
            JsonConvert.DeserializeObject<RegionInfo[]>(File.ReadAllText(masterIndexLocalPath));

         var result = new List<AreaInfo>();
         foreach (var region in regions ?? Enumerable.Empty<RegionInfo>()) {
            if (region.Access == RegionAccess.@public ||
               (region.Access == RegionAccess.@private && preferences.ShowPrivateRegions)) {
               region.LocalPath = System.IO.Path.Combine(repositoriesDir, region.Name);

               if (!Directory.Exists(region.LocalPath)) {
                  Directory.CreateDirectory(region.LocalPath);
               }

               if (!File.Exists(region.LocalIndexPath) || download && force) {
                  await DownloadFile(region.IndexUrl, region.LocalIndexPath);
               }

               if (!File.Exists(region.LocalIndexPath)) {
                  throw new ArgumentException(Strings.AreaNotFoundOrCantBeDownloaded);
               }

               var index = JsonConvert.DeserializeObject<AreaInfo>(File.ReadAllText(region.LocalIndexPath));
               index.Region = region;
               foreach (var image in index.Images ?? Enumerable.Empty<string>()) {
                  await DownloadFile(index.GetImageRemotePath(image), index.GetImageLocalPath(image));
               }

               if (!string.IsNullOrEmpty(index.Map)) {
                  if (!File.Exists(index.MapLocalPath) || download && force) {
                     await DownloadFile(index.MapRemotePath, index.MapLocalPath);
                  }
               }

               OrderAreasRoutes(index);
               SetIsOfflineAndRoots(index, region);

               result.Add(index);
            }
         }

         return result;
      }

      private void OrderAreasRoutes(AreaInfo index) {

         if (index.Areas?.Any() ?? false) {
            foreach (var area in index.Areas ?? Enumerable.Empty<AreaInfo>()) {
               OrderAreasRoutes(area);
            }
            index.Areas = index.
               Areas.OrderBy(a => a.Name).ToArray();
         }

         if (index.Routes?.Any() ?? false) {
            var setting = preferences.RouteOrderByProperty;
            if (setting == Preferences.RouteOrderBy.Difficulty) {
               index.Routes = index.Routes.OrderBy(r => r.Difficulty).ToArray();
            } else if (setting == Preferences.RouteOrderBy.DifficultyDesc) {
               index.Routes = index.Routes.OrderByDescending(r => r.Difficulty).ToArray();
            } else if (setting == Preferences.RouteOrderBy.NameDesc) {
               index.Routes = index.Routes.OrderByDescending(r => r.Name).ToArray();
            } else {
               index.Routes = index.Routes.OrderBy(r => r.Name).ToArray();
            }
         }
      }

      private void SetIsOfflineAndRoots(AreaInfo index, RegionInfo region) {

         index.Region = region;
         bool isOffline = File.Exists(index.LocalPath) && File.Exists(index.MapLocalPath);

         foreach (var area in index.Areas ?? Enumerable.Empty<AreaInfo>()) {
            SetIsOfflineAndRoots(area, region);
            isOffline = isOffline && area.IsOffline;
         }

         foreach (var routeInfo in index.Routes ?? Enumerable.Empty<RouteInfo>()) {
            routeInfo.Region = region;
            routeInfo.IsOffline = File.Exists(routeInfo.LocalPath);

            foreach (var img in routeInfo.Images ?? Enumerable.Empty<string>()) {
               routeInfo.IsOffline = routeInfo.IsOffline && File.Exists(routeInfo.GetImageLocalPath(img));
            }
            isOffline = isOffline && routeInfo.IsOffline;
         }

         index.IsOffline = isOffline;
      }

      private async Task DownloadFile(string remotePath, string localPath) {
         try {
            using (var response = await httpClient.GetAsync(remotePath)) {
               response.EnsureSuccessStatusCode();

               using (var stream = File.Open(localPath, FileMode.Create)) {
                  await response.Content.CopyToAsync(stream);
               }
            }
         } catch (Exception ex) {
            errorService.HandleError(new DownloadFileException("", ex) {
               Address = remotePath
            });
         }
      }
   }
}
