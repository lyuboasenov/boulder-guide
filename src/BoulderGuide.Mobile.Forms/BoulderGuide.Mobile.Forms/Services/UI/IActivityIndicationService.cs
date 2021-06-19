﻿using System.Threading.Tasks;

namespace BoulderGuide.Mobile.Forms.Services.UI {
   public interface IActivityIndicationService {
      Task StartLoadingAsync();
      Task FinishLoadingAsync();

      void StartLoading();
      void FinishLoading();
   }
}
