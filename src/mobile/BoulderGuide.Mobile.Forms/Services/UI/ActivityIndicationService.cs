using BoulderGuide.Mobile.Forms.Views;
using Rg.Plugins.Popup.Services;
using System.Threading.Tasks;
using Xamarin.Essentials.Interfaces;

namespace BoulderGuide.Mobile.Forms.Services.UI {
   internal class ActivityIndicationService : IActivityIndicationService {

      private int _loadingCounter;
      private readonly object _lock = new object();
      private readonly IMainThread mainThread;

      public ActivityIndicationService(IMainThread mainThread) {
         this.mainThread = mainThread;
      }

      public Task<LoadingHandle> StartLoadingAsync() {
         return Task.FromResult(StartLoading());
      }

      public Task FinishLoadingAsync() {
         FinishLoading();

         return Task.CompletedTask;
      }

      public LoadingHandle StartLoading() {
         lock (_lock) {
            _loadingCounter++;

            UpdateActivityIndicationPopup();
         }

         return new LoadingHandle(this);
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
