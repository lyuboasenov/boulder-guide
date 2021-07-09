using System.Threading.Tasks;

namespace BoulderGuide.Mobile.Forms.Services.UI {
   public interface IActivityIndicationService {
      Task<LoadingHandle> StartLoadingAsync();
      Task FinishLoadingAsync();

      LoadingHandle StartLoading();
      void FinishLoading();
   }
}
