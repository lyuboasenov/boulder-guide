using System;

namespace BoulderGuide.Mobile.Forms.Services.Location {
   public interface ILocationService {
      bool IsRunning { get; }
      void Run();
      IDisposable Subscribe(ILocationObserver observer);
      void Unsubscribe(ILocationObserver observer);
   }
}
