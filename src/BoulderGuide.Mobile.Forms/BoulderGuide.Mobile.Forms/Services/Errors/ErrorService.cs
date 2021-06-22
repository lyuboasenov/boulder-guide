using Prism.Services;
using System;

namespace BoulderGuide.Mobile.Forms.Services.Errors {
   internal class ErrorService : IErrorService {
      private readonly IPageDialogService dialogService;

      public ErrorService(IPageDialogService dialogService) {
         this.dialogService = dialogService;
      }

      public void HandleError(Exception ex) {
         dialogService.
            DisplayAlertAsync(
               Strings.GenericExceptionTitle,
               Strings.GenericExceptionMessage,
               Strings.Ok);
#if DEBUG
         PrintException(ex);
#endif
      }

#if DEBUG
      private void PrintException(Exception ex) {
         var indent = new string(' ', 6);
         var json = Newtonsoft.Json.JsonConvert.SerializeObject(ex).Replace(Environment.NewLine, Environment.NewLine + indent);
         System.Diagnostics.Debug.WriteLine("Exception:");
         System.Diagnostics.Debug.WriteLine($"   Type: {ex.GetType().Name}");

         System.Diagnostics.Debug.WriteLine($"   JSON:");
         System.Diagnostics.Debug.WriteLine(indent + json);
      }
#endif

      public void HandleError(Exception ex, string message) {
         dialogService.
            DisplayAlertAsync(
               Strings.GenericExceptionTitle,
               message,
               Strings.Ok);
#if DEBUG
         PrintException(ex);
#endif
      }
   }
}
