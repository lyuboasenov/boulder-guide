namespace BoulderGuide.Mobile.Forms.Services.Location {
   public interface ILocationObserver {
      void OnLocationChanged(DTOs.Location value);
   }
}
