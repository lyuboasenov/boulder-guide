using Prism.Services;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
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

         TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
      }

      public Task HandleErrorAsync(Exception ex, bool isDevelopersOnly = false) {
         return HandleErrorAsync(ex, Strings.GenericExceptionMessage, isDevelopersOnly);
      }

      public async Task HandleErrorAsync(Exception ex, string message, bool isDevelopersOnly = false) {
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

         } else if (!isDevelopersOnly) {
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

      private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e) {
         HandleError(e.Exception);
         e.SetObserved();
      }

      private static string FormatException(Exception ex) {
         var indent = new string(' ', 6);
         var settings = new Newtonsoft.Json.JsonSerializerSettings() {
            Formatting = Newtonsoft.Json.Formatting.Indented,
            MaxDepth = 10,
            ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Serialize,
            PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects,
            Error = delegate (object sender, Newtonsoft.Json.Serialization.ErrorEventArgs args)
            {
               // silence errors
               args.ErrorContext.Handled = true;
            },
            ContractResolver = new GetExceptionPropertiesResolver()
         };

         var serializer = Newtonsoft.Json.JsonSerializer.CreateDefault(settings);
         var jb = new StringBuilder();
         using (var tw = new System.IO.StringWriter(jb))
         using (var jw = new Newtonsoft.Json.JsonTextWriter(tw)) {
            serializer.Serialize(jw, ex);
         }

         var json = jb.ToString().Replace(Environment.NewLine, Environment.NewLine + indent);
         var sb = new StringBuilder();
         sb.AppendLine("Exception:");
         sb.AppendLine($"   Type: {ex.GetType().Name}");

         sb.AppendLine($"   JSON:");
         sb.AppendLine(indent + json);

         return sb.ToString();
      }
   }

   public class GetExceptionPropertiesResolver : Newtonsoft.Json.Serialization.DefaultContractResolver {
      protected override IList<Newtonsoft.Json.Serialization.JsonProperty> CreateProperties(Type type, Newtonsoft.Json.MemberSerialization memberSerialization) {
         var propertyList = new[] { "Message", "InnerException", "InnerExceptions", "StackTrace" };
         var allProps = base.CreateProperties(type, memberSerialization);

         //Choose the properties you want to serialize/deserialize
         return allProps.Where(p => propertyList.Any(a => a == p.PropertyName)).ToList();
      }
   }
}
