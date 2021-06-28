using BoulderGuide.Mobile.Forms.Domain;
using BoulderGuide.Mobile.Forms.Domain.DTOs;
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
      private const string masterIndexRemoteLocation = "https://raw.githubusercontent.com/lyuboasenov/boulder-guide/main/data/index-v2.json";
#endif


      private readonly IConnectivity connectivity;
      private readonly IDownloadService downloadService;
      private readonly Preferences.IPreferences preferences;
      private string repositoryDirectory;

      public DataService(
         IFileSystem fileSystem,
         IConnectivity connectivity,
         IDownloadService downloadService,
         Preferences.IPreferences preferences) {
         this.connectivity = connectivity;
         this.downloadService = downloadService;
         this.preferences = preferences;

         repositoryDirectory = Path.Combine(fileSystem.AppDataDirectory, "repositories");
      }

      public Task ClearLocalStorageAsync() {
         Directory.Delete(repositoryDirectory, true);

         return Task.CompletedTask;
      }

      public Task<int> GetLocalStorageSizeInMBAsync() {
         DirectoryInfo info = new DirectoryInfo(repositoryDirectory);
         long size = 0;
         if (info.Exists) {
            foreach (var file in info.GetFiles("*", SearchOption.AllDirectories)) {
               size += file.Length;
            }
         }

         return Task.FromResult((int) (size >> 20));
      }

      public async Task<OperationResult<IEnumerable<AreaInfo>>> GetIndexAreasAsync(bool force) {
         if (connectivity.NetworkAccess != NetworkAccess.Internet) {
               return await GetAreas(false, force);
         } else {
            return await GetAreas(true, force);
         }
      }

      private async Task<OperationResult<IEnumerable<AreaInfo>>> GetAreas(bool download, bool force) {

         if (!Directory.Exists(repositoryDirectory)) {
            Directory.CreateDirectory(repositoryDirectory);
         }

         var masterIndexLocalPath = Path.Combine(repositoryDirectory, "index-v2.json");
         if (!File.Exists(masterIndexLocalPath) || download && force) {
            await downloadService.DownloadFile(masterIndexRemoteLocation, masterIndexLocalPath);
         }


         var errors = new List<Exception>();
         var regions = new List<Region>();

         var regionDtos =
            JsonConvert.DeserializeObject<RegionDTO[]>(
               File.ReadAllText(masterIndexLocalPath));


         foreach (var dto in regionDtos) {
            try {
               if (dto.Access == RegionAccess.@public ||
                  (dto.Access == RegionAccess.@private && preferences.ShowPrivateRegions)) {
                  regions.Add(new Region(
                     dto,
                     Path.Combine(repositoryDirectory, dto.Name)));
                  }
            } catch (Exception ex) {
               errors.Add(ex);
            }
         }

         var result = new List<AreaInfo>();

         foreach (var region in regions ?? Enumerable.Empty<Region>()) {
            if (region.Access == RegionAccess.@public ||
               (region.Access == RegionAccess.@private && preferences.ShowPrivateRegions)) {

               try {
                  if (!region.ExistsLocally || download && force) {
                     await region.DownloadAsync(force).ConfigureAwait(false);
                  }

                  result.Add(await region.GetIndexAsync().ConfigureAwait(false));
               } catch (Exception ex) {
                  errors.Add(ex);
               }
            }
         }

         return new OperationResult<IEnumerable<AreaInfo>>(result, errors);
      }
   }
}
