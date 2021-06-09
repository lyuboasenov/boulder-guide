using System;
using System.Collections.Generic;
using System.Text;

namespace BoulderGuide.Mobile.Forms.Services.Errors {
   public interface IErrorService {
      void HandleError(Exception ex);
   }
}
