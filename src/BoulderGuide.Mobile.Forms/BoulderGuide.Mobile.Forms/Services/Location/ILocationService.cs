using System;

namespace BoulderGuide.Mobile.Forms.Services.Location {
   public interface ILocationService {

      void Initialize();
      IDisposable Subscribe(ILocationObserver observer);
      void Unsubscribe(ILocationObserver observer);
   }
}
