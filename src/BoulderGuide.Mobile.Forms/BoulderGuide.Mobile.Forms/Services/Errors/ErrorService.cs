using BoulderGuide.Mobile.Forms.Services.Data;
using Plugin.Toasts;
using System;
using System.Collections.Generic;
using System.Text;

namespace BoulderGuide.Mobile.Forms.Services.Errors {
   internal class ErrorService : IErrorService {
      private readonly IToastNotificator toastNotificator;

      public ErrorService(IToastNotificator toastNotificator) {
         this.toastNotificator = toastNotificator;
      }

      public void HandleError(Exception ex) {
         toastNotificator.Notify(new NotificationOptions() {
            Title = "Error",
            Description = FormatException(ex)
         });
      }

      private string FormatException(Exception ex) {
         if (ex is DownloadFileException dEx) {
            return $"Error downloading {dEx.Address}";
         } else {
            return ex.Message + Environment.NewLine + ex.StackTrace;
         }
      }
   }
}
