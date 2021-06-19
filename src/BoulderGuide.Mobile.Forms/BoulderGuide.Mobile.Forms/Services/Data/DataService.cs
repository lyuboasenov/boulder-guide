using BoulderGuide.DTOs;
using BoulderGuide.Mobile.Forms.Services.Data.Entities;
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
      private Region[] regions;

      private readonly IFileSystem fileSystem;
      private readonly IConnectivity connectivity;
      private readonly IErrorService errorService;
      private readonly Preferences.IPreferences preferences;
      private readonly IDownloadService downloadService;

      public DataService(
         IFileSystem fileSystem,
         IConnectivity connectivity,
         IErrorService errorService,
         Preferences.IPreferences preferences,
         IDownloadService downloadService) {
         this.fileSystem = fileSystem;
         this.connectivity = connectivity;
         this.errorService = errorService;
         this.preferences = preferences;
         this.downloadService = downloadService;
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

      private async Task<IEnumerable<AreaInfo>> GetAreas(bool download, bool force) {

         var repositoriesDir = System.IO.Path.Combine(fileSystem.AppDataDirectory, "repositories");
         if (!Directory.Exists(repositoriesDir)) {
            Directory.CreateDirectory(repositoriesDir);
         }

         var masterIndexLocalPath = System.IO.Path.Combine(repositoriesDir, "index-v2.json");
         if (!File.Exists(masterIndexLocalPath) || download && force) {
            await downloadService.DownloadFile(masterIndexRemoteLocation, masterIndexLocalPath);
         }

         regions =
            JsonConvert.DeserializeObject<RegionDTO[]>(
               File.ReadAllText(masterIndexLocalPath))?.
               Select(dto =>
                  new Region(
                     dto,
                     System.IO.Path.Combine(repositoriesDir, dto.Name),
                     downloadService))?.ToArray();

         var result = new List<AreaInfo>();
         foreach (var region in regions ?? Enumerable.Empty<Region>()) {
            if (region.Access == RegionAccess.@public ||
               (region.Access == RegionAccess.@private && preferences.ShowPrivateRegions)) {

               if (!region.ExistsLocally || download && force) {
                  await region.DownloadAsync();
               }

               result.Add(await region.GetIndexAsync());
            }
         }

         return result;
      }

      //private void OrderAreasRoutes(AreaInfoDTO index) {

      //   if (index.Areas?.Any() ?? false) {
      //      foreach (var area in index.Areas ?? Enumerable.Empty<AreaInfoDTO>()) {
      //         OrderAreasRoutes(area);
      //      }
      //      index.Areas = index.
      //         Areas.OrderBy(a => a.Name).ToArray();
      //   }

      //   if (index.Routes?.Any() ?? false) {
      //      var setting = preferences.RouteOrderByProperty;
      //      if (setting == Preferences.RouteOrderBy.Difficulty) {
      //         index.Routes = index.Routes.OrderBy(r => r.Difficulty).ToArray();
      //      } else if (setting == Preferences.RouteOrderBy.DifficultyDesc) {
      //         index.Routes = index.Routes.OrderByDescending(r => r.Difficulty).ToArray();
      //      } else if (setting == Preferences.RouteOrderBy.NameDesc) {
      //         index.Routes = index.Routes.OrderByDescending(r => r.Name).ToArray();
      //      } else {
      //         index.Routes = index.Routes.OrderBy(r => r.Name).ToArray();
      //      }
      //   }
      //}
   }
}
