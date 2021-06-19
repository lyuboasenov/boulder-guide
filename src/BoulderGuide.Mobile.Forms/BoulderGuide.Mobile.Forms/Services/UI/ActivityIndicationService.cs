using BoulderGuide.Mobile.Forms.Views;
using Rg.Plugins.Popup.Services;
using System.Threading.Tasks;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;

namespace BoulderGuide.Mobile.Forms.Services.UI {
   internal class ActivityIndicationService : IActivityIndicationService {

      private int _loadingCounter;
      private readonly object _lock = new object();
      private readonly IMainThread mainThread;

      public ActivityIndicationService(IMainThread mainThread) {
         this.mainThread = mainThread;
      }

      public Task StartLoadingAsync() {
         StartLoading();

         return Task.CompletedTask;
      }

      public Task FinishLoadingAsync() {
         FinishLoading();

         return Task.CompletedTask;
      }

      public void StartLoading() {
         lock (_lock) {
            _loadingCounter++;

            UpdateActivityIndicationPopup();
         }
      }

      public void FinishLoading() {
         lock (_lock) {
            _loadingCounter--;
            UpdateActivityIndicationPopup();
         }
      }

      private void UpdateActivityIndicationPopup() {
         if (_loadingCounter == 1) {
            // show
            mainThread.InvokeOnMainThreadAsync(async () =>
               await PopupNavigation.Instance.PushAsync(new LoadingPopupPage()));
         } else if (_loadingCounter == 0) {
            // hide
            mainThread.InvokeOnMainThreadAsync(async () =>
               await PopupNavigation.Instance.PopAllAsync());
         }
      }
   }
}
