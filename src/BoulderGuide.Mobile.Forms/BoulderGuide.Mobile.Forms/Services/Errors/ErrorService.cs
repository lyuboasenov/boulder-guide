using Prism.Services;
using Prism.Services.Dialogs;
using System;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials.Interfaces;

namespace BoulderGuide.Mobile.Forms.Services.Errors {
   internal class ErrorService : IErrorService {
      private readonly IPageDialogService pageDialogService;
      private readonly Preferences.IPreferences preferences;
      private readonly IDialogService dialogService;
      private readonly IMainThread mainThread;

      public ErrorService(
         IPageDialogService pageDialogService,
         Preferences.IPreferences preferences,
         IDialogService dialogService,
         IMainThread mainThread) {
         this.pageDialogService = pageDialogService;
         this.preferences = preferences;
         this.dialogService = dialogService;
         this.mainThread = mainThread;
      }

      public Task HandleErrorAsync(Exception ex) {
         return HandleErrorAsync(ex, Strings.GenericExceptionMessage);
      }

      public async Task HandleErrorAsync(Exception ex, string message) {
         if (preferences.IsDeveloperEnabled) {
            await mainThread.InvokeOnMainThreadAsync(async () => {
               if (await pageDialogService.
                  DisplayAlertAsync(
                     Strings.GenericExceptionTitle,
                     message,
                     Strings.ReadMore,
                     Strings.Ok)) {
                  await dialogService.ShowDialogAsync(
                     nameof(Views.TextViewDialogPage),
                     new DialogParameters() {
                     { "Title", Strings.GenericExceptionTitle },
                     { "Text", FormatException(ex) }
                     });
               }
            });

         } else {
            await mainThread.InvokeOnMainThreadAsync(async () =>
               await pageDialogService.
                  DisplayAlertAsync(
                     Strings.GenericExceptionTitle,
                     message,
                     Strings.Ok));
         }

#if DEBUG
         System.Diagnostics.Debug.WriteLine(FormatException(ex));
#endif
      }

      public void HandleError(Exception ex) {
         HandleError(ex, Strings.GenericExceptionMessage);
      }

      public void HandleError(Exception ex, string message) {

         if (preferences.IsDeveloperEnabled) {
            mainThread.InvokeOnMainThreadAsync(async () => {
               if (await pageDialogService.
                  DisplayAlertAsync(
                     Strings.GenericExceptionTitle,
                     message,
                     Strings.ReadMore,
                     Strings.Ok)) {
                  await dialogService.ShowDialogAsync(
                     nameof(Views.TextViewDialogPage),
                     new DialogParameters() {
                     { "Title", Strings.GenericExceptionTitle },
                     { "Text", FormatException(ex) }
                     });
               }
            });

         } else {
            mainThread.InvokeOnMainThreadAsync(async () =>
               await pageDialogService.
                  DisplayAlertAsync(
                     Strings.GenericExceptionTitle,
                     message,
                     Strings.Ok));
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
