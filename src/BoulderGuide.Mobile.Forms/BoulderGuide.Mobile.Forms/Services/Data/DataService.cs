using BoulderGuide.Mobile.Forms.Domain;
using BoulderGuide.Mobile.Forms.Domain.DTOs;
using BoulderGuide.Mobile.Forms.Services.Errors;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
      private readonly IDownloadService downloadService;
      private readonly Preferences.IPreferences preferences;

      public DataService(
         IFileSystem fileSystem,
         IConnectivity connectivity,
         IErrorService errorService,
         IDownloadService downloadService,
         Preferences.IPreferences preferences) {
         this.fileSystem = fileSystem;
         this.connectivity = connectivity;
         this.errorService = errorService;
         this.downloadService = downloadService;
         this.preferences = preferences;
      }

      public Task ClearLocalStorage() {
         try {
            var reposDir = Path.Combine(fileSystem.AppDataDirectory, "repositories");
            Directory.Delete(reposDir, true);

            return Task.CompletedTask;
         } catch (Exception ex) {
            return errorService.HandleErrorAsync(ex);
         }
      }

      public async Task<int> GetLocalStorageSizeInMB() {
         try {
            var repoDir = Path.Combine(fileSystem.AppDataDirectory, "repositories");
            DirectoryInfo info = new DirectoryInfo(repoDir);
            long size = 0;
            if (info.Exists) {
               foreach (var file in info.GetFiles("*", SearchOption.AllDirectories)) {
                  size += file.Length;
               }
            }

            return (int) (size >> 20);
         } catch (Exception ex) {
            await errorService.HandleErrorAsync(ex);
            return 0;
         }
      }

      public async Task<IEnumerable<AreaInfo>> GetIndexAreas(bool force) {
         try {
            if (connectivity.NetworkAccess != NetworkAccess.Internet) {
                return await GetAreas(false, force);
            } else {
               return await GetAreas(true, force);
            }
         } catch (Exception ex) {
            await errorService.HandleErrorAsync(ex);
            return null;
         }
      }

      private async Task<IEnumerable<AreaInfo>> GetAreas(bool download, bool force) {

         var repositoriesDir = Path.Combine(fileSystem.AppDataDirectory, "repositories");
         if (!Directory.Exists(repositoriesDir)) {
            Directory.CreateDirectory(repositoriesDir);
         }

         var masterIndexLocalPath = Path.Combine(repositoriesDir, "index-v2.json");
         if (!File.Exists(masterIndexLocalPath) || download && force) {
            await downloadService.DownloadFile(masterIndexRemoteLocation, masterIndexLocalPath);
         }

         regions =
            JsonConvert.DeserializeObject<RegionDTO[]>(
               File.ReadAllText(masterIndexLocalPath))?.
               Select(dto =>
                  new Region(
                     dto,
                     Path.Combine(repositoriesDir, dto.Name)))?.ToArray();

         var result = new List<AreaInfo>();
         foreach (var region in regions ?? Enumerable.Empty<Region>()) {
            if (region.Access == RegionAccess.@public ||
               (region.Access == RegionAccess.@private && preferences.ShowPrivateRegions)) {

               if (!region.ExistsLocally || download && force) {
                  await region.DownloadAsync().ConfigureAwait(false);
               }

               result.Add(await region.GetIndexAsync().ConfigureAwait(false));
            }
         }

         return result;
      }
   }
}
