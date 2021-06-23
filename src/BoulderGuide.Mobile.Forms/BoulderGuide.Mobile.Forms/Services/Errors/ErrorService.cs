using Prism.Services;
using Prism.Services.Dialogs;
using System;
using System.Text;
using System.Threading.Tasks;

namespace BoulderGuide.Mobile.Forms.Services.Errors {
   internal class ErrorService : IErrorService {
      private readonly IPageDialogService pageDialogService;
      private readonly Preferences.IPreferences preferences;
      private readonly IDialogService dialogService;

      public ErrorService(
         IPageDialogService pageDialogService,
         Preferences.IPreferences preferences,
         IDialogService dialogService) {
         this.pageDialogService = pageDialogService;
         this.preferences = preferences;
         this.dialogService = dialogService;
      }

      public void HandleError(Exception ex) {
         HandleError(ex, Strings.GenericExceptionMessage);
      }

      public void HandleError(Exception ex, string message) {

         if (preferences.IsDeveloperEnabled) {
            Task.Run(async () => {
               await pageDialogService.
                  DisplayAlertAsync(
                     Strings.GenericExceptionTitle,
                     message,
                     Strings.ReadMore,
                     Strings.Ok);
               await dialogService.ShowDialogAsync(
                  nameof(Views.TextViewDialogPage),
                  new DialogParameters() {
                     { "Title", Strings.GenericExceptionTitle },
                     { "Text", FormatException(ex) }
                  });
            });

         } else {
            pageDialogService.
               DisplayAlertAsync(
                  Strings.GenericExceptionTitle,
                  message,
                  Strings.Ok);
         }

#if DEBUG
         System.Diagnostics.Debug.WriteLine(FormatException(ex));
#endif
      }

      private static string FormatException(Exception ex) {
         var indent = new string(' ', 6);
         var json = Newtonsoft.Json.JsonConvert.SerializeObject(ex).Replace(Environment.NewLine, Environment.NewLine + indent);
         var sb = new StringBuilder();
         sb.AppendLine("Exception:");
         sb.AppendLine($"   Type: {ex.GetType().Name}");

         sb.AppendLine($"   JSON:");
         sb.AppendLine(indent + json);

         return sb.ToString();
      }
   }
}
