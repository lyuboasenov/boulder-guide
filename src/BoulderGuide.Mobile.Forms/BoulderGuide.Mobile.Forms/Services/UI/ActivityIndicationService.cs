using BoulderGuide.Mobile.Forms.Views;
using Rg.Plugins.Popup.Services;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BoulderGuide.Mobile.Forms.Services.UI {
   internal class ActivityIndicationService : IActivityIndicationService {

      private int _loadingCounter;
      private readonly object _lock = new object();

      public Task StartLoadingAsync() {
         lock(_lock) {
            _loadingCounter++;

            return UpdateActivityIndicationPopupAsync();
         }
      }

      public Task FinishLoadingAsync() {
         lock(_lock) {
            _loadingCounter--;
            return UpdateActivityIndicationPopupAsync();
         }
      }

      private Task UpdateActivityIndicationPopupAsync() {
         if (_loadingCounter == 1) {
            // show
            Device.BeginInvokeOnMainThread(async () =>
               await PopupNavigation.Instance.PushAsync(new LoadingPopupPage()));
         } else if (_loadingCounter == 0) {
            // hide
            Device.BeginInvokeOnMainThread(async () =>
               await PopupNavigation.Instance.PopAllAsync());
         }

         return Task.CompletedTask;
      }
   }
}
