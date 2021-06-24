using Xamarin.Essentials.Interfaces;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using BoulderGuide.Mobile.Forms.Services.Errors;
using System.Threading;
using System.Collections.Generic;

namespace BoulderGuide.Mobile.Forms.Services.Location {
   public class LocationService : ILocationService, IDisposable {

      private readonly Task task;
      private CancellationTokenSource cts;
      private CancellationToken token;
      private readonly int backgoundLoopMillisecondDelay = 500;
      private int pollPeriodInMilliseconds = 5 * 60 * 1000; // Defaults to 5 minutes
      private int millisecondsFromLastPoll = 5 * 60 * 1000;

      private readonly object _lock = new object();
      private bool disposedValue;
      private bool shouldBuildAccurancy = true;

      private readonly IPermissions permissions;
      private readonly Preferences.IPreferences preferences;
      private readonly IGeolocation geolocation;
      private readonly IErrorService errorService;

      private Xamarin.Essentials.Location lastKnownLocation;

      public LocationService(
         IPermissions permissions,
         Preferences.IPreferences preferences,
         IGeolocation geolocation,
         IErrorService errorService) {
         this.permissions = permissions;
         this.preferences = preferences;
         this.geolocation = geolocation;
         this.errorService = errorService;

         cts = new CancellationTokenSource();
         token = cts.Token;
         task = Task.Run(BackgroundLoopAsync, token);
      }

      private async Task BackgroundLoopAsync() {
         while(!token.IsCancellationRequested) {
            try {
               if (millisecondsFromLastPoll >= pollPeriodInMilliseconds) {
                  await PollLocationAsync();
                  millisecondsFromLastPoll = 0;
               }

               await Task.Delay(backgoundLoopMillisecondDelay, token);
               millisecondsFromLastPoll += backgoundLoopMillisecondDelay;
            } catch (Exception ex) {
               await errorService.HandleErrorAsync(ex, true);
            }
         }
      }

      private async Task PollLocationAsync() {
         if (await HasPermissionsAsync()) {
            if (shouldBuildAccurancy) {
               lastKnownLocation = await geolocation.GetLastKnownLocationAsync().ConfigureAwait(false);
               await NotifySubscibersAsync().ConfigureAwait(false);

               for (int i = 1; i < 5; i++) {
                  await PollLocationAsync((GeolocationAccuracy) i).ConfigureAwait(false);
               }

               shouldBuildAccurancy = false;
            }

            await PollLocationAsync(GeolocationAccuracy.Best).ConfigureAwait(false);
         }
      }

      private async Task PollLocationAsync(GeolocationAccuracy geolocationAccuracy) {
         lastKnownLocation = await geolocation.
            GetLocationAsync(new GeolocationRequest(geolocationAccuracy), token).
            ConfigureAwait(false);

         await NotifySubscibersAsync();
      }

      private Task NotifySubscibersAsync() {
         if (null != lastKnownLocation) {
            lock(_lock) {
               foreach (var observer in observers) {
                  observer.OnLocationChanged(
                     new DTOs.Location(
                        lastKnownLocation.Latitude,
                        lastKnownLocation.Longitude));
               }
            }
         }
         return Task.CompletedTask;
      }

      private async Task<bool> HasPermissionsAsync() {
         return await permissions.CheckStatusAsync<Permissions.LocationWhenInUse>() == PermissionStatus.Granted;
      }

      protected virtual void Dispose(bool disposing) {
         if (!disposedValue) {
            if (disposing) {
               cts.Cancel();
            }

            disposedValue = true;
         }
      }

      public void Dispose() {
         // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
         Dispose(disposing: true);
         GC.SuppressFinalize(this);
      }

      private List<ILocationObserver> observers = new List<ILocationObserver>();

      public IDisposable Subscribe(ILocationObserver observer) {
         lock(_lock) {
            observers.Add(observer);
            pollPeriodInMilliseconds = preferences.GPSPollIntervalInSeconds * 1000;
         }

         return new LocationObserverHandle(this, observer);
      }

      public void Unsubscribe(ILocationObserver observer) {
         lock(_lock) {
            observers.Remove(observer);
            if (observers.Count == 0) {
               // back to default poll period
               pollPeriodInMilliseconds = 5 * 60 * 1000;
            }
         }
      }

      private class LocationObserverHandle : IDisposable {
         private readonly ILocationService locationService;
         private readonly ILocationObserver observer;

         public LocationObserverHandle(
            ILocationService locationService,
            ILocationObserver observer) {
            this.locationService = locationService;
            this.observer = observer;
         }

         public void Dispose() {
            locationService.Unsubscribe(observer);
         }
      }

      public void Initialize() {
         // do some dummy work here
         if (task.IsCompleted) {
            // this should not happen
            pollPeriodInMilliseconds = 5 * 60 * 1000;
         }
      }
   }
}
