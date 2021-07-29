namespace BoulderGuide.Mobile.Forms.Services.Location {
   public interface ILocationObserver {
      void OnLocationChanged(double latitude, double longitude, double direction);
   }
}
