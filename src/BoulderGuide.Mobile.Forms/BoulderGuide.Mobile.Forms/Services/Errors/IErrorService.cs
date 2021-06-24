using System;
using System.Threading.Tasks;

namespace BoulderGuide.Mobile.Forms.Services.Errors {
   public interface IErrorService {
      Task HandleErrorAsync(Exception ex, bool isDevelopersOnly = false);
      Task HandleErrorAsync(Exception ex, string message, bool isDevelopersOnly = false);

      void HandleError(Exception ex);
      void HandleError(Exception ex, string message);

   }
}
