using System;
using System.Threading.Tasks;

namespace BoulderGuide.Mobile.Forms.Services.Errors {
   public static class ExceptionExtenssions {
      private static IErrorService errorService =
               Prism.PrismApplicationBase.Current?.Container?.CurrentScope?.Resolve(typeof(IErrorService)) as IErrorService;
      public static Task HandleAsync(this Exception ex, bool isDeveloperOnly = false) {
         if (errorService is null) {
            return Task.CompletedTask;
         } else {
            return errorService.HandleErrorAsync(ex, isDeveloperOnly);
         }
      }

      public static void Handle(this Exception ex) {
         errorService?.HandleError(ex);
      }
   }
}
