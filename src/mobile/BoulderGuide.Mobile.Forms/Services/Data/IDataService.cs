using BoulderGuide.Mobile.Forms.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BoulderGuide.Mobile.Forms.Services.Data {
   public interface IDataService {
      Task ClearLocalStorageAsync();
      Task<int> GetLocalStorageSizeInMBAsync();
      Task<OperationResult<IEnumerable<AreaInfo>>> GetIndexAreasAsync(bool force);
   }
}
