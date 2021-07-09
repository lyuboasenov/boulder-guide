using System;

namespace BoulderGuide.Mobile.Forms.Services.UI {
   public class LoadingHandle : IDisposable {
      private bool disposedValue;
      private readonly IActivityIndicationService activityIndicationService;

      public LoadingHandle(IActivityIndicationService activityIndicationService) {
         this.activityIndicationService = activityIndicationService;
      }

      protected virtual void Dispose(bool disposing) {
         if (!disposedValue) {
            if (disposing) {
               // TODO: dispose managed state (managed objects)
               activityIndicationService.FinishLoading();
            }

            disposedValue = true;
         }
      }

      public void Dispose() {
         // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
         Dispose(disposing: true);
         GC.SuppressFinalize(this);
      }
   }
}
