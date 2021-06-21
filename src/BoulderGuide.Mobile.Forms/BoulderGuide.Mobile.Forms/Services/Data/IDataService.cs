using BoulderGuide.Mobile.Forms.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BoulderGuide.Mobile.Forms.Services.Data {
   public interface IDataService {
      Task ClearLocalStorage();
      Task<int> GetLocalStorageSizeInMB();
      Task<IEnumerable<AreaInfo>> GetIndexAreas(bool force);
   }
}
