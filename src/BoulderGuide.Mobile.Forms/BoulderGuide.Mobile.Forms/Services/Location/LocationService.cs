using Xamarin.Essentials.Interfaces;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using BoulderGuide.Mobile.Forms.Services.Errors;
using System.Threading;
using System.Collections.Generic;

namespace BoulderGuide.Mobile.Forms.Services.Location {
   public class LocationService : ILocationService, IDisposable {

      private const int DEFAULT_GET_PERIOD_MS = 300000; // Defaults to 5 minutes
      private const int BACKGROUND_LOOP_DELAY_MS = 500; // 1/2 second

      private Task task;
      private CancellationTokenSource cts;
      private CancellationToken token;


      private int getPeriodInMs = DEFAULT_GET_PERIOD_MS;
      private int millisecondsFromLastPoll = DEFAULT_GET_PERIOD_MS;

      private readonly object _lock = new object();
      private List<ILocationObserver> observers = new List<ILocationObserver>();
      private double myDirection;

      private bool disposedValue;
      private bool shouldBuildAccurancy = true;

      private readonly IPermissions permissions;
      private readonly Preferences.IPreferences preferences;
      private readonly IGeolocation geolocation;
      private readonly IErrorService errorService;
      private readonly ICompass compass;

      public LocationService(
         IPermissions permissions,
         Preferences.IPreferences preferences,
         IGeolocation geolocation,
         IErrorService errorService,
         ICompass compass) {
         this.permissions = permissions;
         this.preferences = preferences;
         this.geolocation = geolocation;
         this.errorService = errorService;
         this.compass = compass;
         compass.ReadingChanged += Compass_ReadingChanged;
      }

      private void Compass_ReadingChanged(object sender, CompassChangedEventArgs e) {
         myDirection = e.Reading.HeadingMagneticNorth;
      }

      private async Task BackgroundLoopAsync() {
         while(!token.IsCancellationRequested) {
            try {
               if (millisecondsFromLastPoll >= getPeriodInMs) {
                  await GetLocationAndNotifyAsync();
                  millisecondsFromLastPoll = 0;
               }

               await Task.Delay(BACKGROUND_LOOP_DELAY_MS, token);
               millisecondsFromLastPoll += BACKGROUND_LOOP_DELAY_MS;
            } catch (Exception ex) {
               await errorService.HandleErrorAsync(ex, true);
            }
         }
      }

      private async Task GetLocationAndNotifyAsync() {
         if (await HasPermissionsAsync()) {
            if (shouldBuildAccurancy) {
               var location = await geolocation.GetLastKnownLocationAsync().ConfigureAwait(false);
               await NotifySubscibersAsync(location).ConfigureAwait(false);

               for (int i = 1; i < 5; i++) {
                  await GetLocationAndNotifyAsync((GeolocationAccuracy) i).ConfigureAwait(false);
               }

               shouldBuildAccurancy = false;
            }

            await GetLocationAndNotifyAsync(GeolocationAccuracy.Best).ConfigureAwait(false);
         }
      }

      private async Task GetLocationAndNotifyAsync(GeolocationAccuracy geolocationAccuracy) {
         var location = await geolocation.
            GetLocationAsync(new GeolocationRequest(geolocationAccuracy), token).
            ConfigureAwait(false);

         await NotifySubscibersAsync(location);
      }

      private Task NotifySubscibersAsync(Xamarin.Essentials.Location location) {
         if (null != location) {
            lock(_lock) {
               preferences.LastKnownLatitude = location.Latitude;
               preferences.LastKnownLongitude = location.Longitude;

               foreach (var observer in observers) {
                  observer.OnLocationChanged(
                     location.Latitude,
                     location.Longitude,
                     myDirection);
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

      public IDisposable Subscribe(ILocationObserver observer) {
         Run();

         lock(_lock) {
            observers.Add(observer);
            getPeriodInMs = preferences.GPSPollIntervalInSeconds * 1000;
            compass.Start(SensorSpeed.Default);
         }

         return new LocationObserverHandle(this, observer);
      }

      public void Unsubscribe(ILocationObserver observer) {
         lock(_lock) {
            observers.Remove(observer);
            if (observers.Count == 0) {
               // back to default poll period
               getPeriodInMs = DEFAULT_GET_PERIOD_MS;
               compass.Stop();;
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

      public void Run() {
         if (!IsRunning) {
            cts = new CancellationTokenSource();
            token = cts.Token;
            task = Task.Run(BackgroundLoopAsync, token);
         }
      }

      public bool IsRunning =>
         task?.IsCanceled == false &&
         task?.IsCompleted == false &&
         task?.IsFaulted == false;
   }
}
