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
         toastNotificator.Notify(FormatException(ex));
      }

      private NotificationOptions FormatException(Exception ex) {
         if (ex is DownloadFileException dEx) {
            return new NotificationOptions() {
               Title = "Error downloading file",
               Description = dEx.Address
            };
         } else {
            return new NotificationOptions() {
               Title = "Generic error",
               Description = ex.Message + Environment.NewLine + ex.StackTrace
            };
         }
      }
   }
}
