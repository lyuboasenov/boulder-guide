using BoulderGuide.Mobile.Forms.Services.Errors;
using Mapsui;
using System;
using Xamarin.Forms;

namespace BoulderGuide.Mobile.Forms.Views {
   public class BindableMapView : Mapsui.UI.Forms.MapView {
      public static readonly BindableProperty BindableMapProperty =
         BindableProperty.Create(
            nameof(BindableMap),
            typeof(Map),
            typeof(BindableMapView),
            null,
            propertyChanged: (b, o, n) => {
               ((BindableMapView) b).OnBindableMapChanged();
            });

      public static readonly BindableProperty MyLocationProperty =
         BindableProperty.Create(
            nameof(MyLocation),
            typeof(Mapsui.UI.Forms.Position),
            typeof(BindableMapView),
            null,
            propertyChanged:(b, _, __) => (b as BindableMapView)?.OnMyLocationChanged());

      public static readonly BindableProperty ResolutionProperty =
         BindableProperty.Create(
            nameof(Resolution),
            typeof(double),
            typeof(BindableMapView),
            0.2d,
            propertyChanged: (b, _, __) => ((BindableMapView) b).OnResolutionChanged(),
            validateValue: (b, v) => (b as BindableMapView)?.IsValidResolution(v) ?? false);

      public static readonly BindableProperty MinResolutionProperty =
         BindableProperty.Create(
            nameof(MinResolution),
            typeof(double),
            typeof(BindableMapView),
            0.2d,
            validateValue: (b,  v) => (b as BindableMapView)?.IsValidMinResolution(v) ?? false);


      public static readonly BindableProperty MaxResolutionProperty =
         BindableProperty.Create(
            nameof(MaxResolution),
            typeof(double),
            typeof(BindableMapView),
            20000d,
            validateValue: (b, v) => (b as BindableMapView)?.IsValidMaxResolution(v) ?? false);

      public static new readonly BindableProperty RotationProperty =
         BindableProperty.Create(
            nameof(Rotation),
            typeof(double),
            typeof(BindableMapView),
            0d,
            propertyChanged: (b, _, __) => {
               ((BindableMapView) b).OnRotationChanged();
            });

      public static readonly BindableProperty TrackMyLocationProperty =
         BindableProperty.Create(
            nameof(TrackMyLocation),
            typeof(bool),
            typeof(BindableMapView),
            false,
            propertyChanged: (b, _, __) => (b as BindableMapView)?.ONTrackMyLocationChanged());

      public Map BindableMap {
         get { return (Map) GetValue(BindableMapProperty); }
         set { SetValue(BindableMapProperty, value); }
      }

      public Mapsui.UI.Forms.Position MyLocation {
         get { return (Mapsui.UI.Forms.Position) GetValue(MyLocationProperty); }
         private set { SetValue(MyLocationProperty, value); }
      }

      public double Resolution {
         get { return (double) GetValue(ResolutionProperty); }
         set { SetValue(ResolutionProperty, value); }
      }

      public double MinResolution {
         get { return (double) GetValue(MinResolutionProperty); }
         set { SetValue(MinResolutionProperty, value); }
      }

      public double MaxResolution {
         get { return (double) GetValue(MaxResolutionProperty); }
         set { SetValue(MaxResolutionProperty, value); }
      }

      public new double Rotation {
         get { return (double) GetValue(RotationProperty); }
         set { SetValue(RotationProperty, value); }
      }

      public bool TrackMyLocation {
         get { return (bool) GetValue(TrackMyLocationProperty); }
         set { SetValue(TrackMyLocationProperty, value); }
      }

      public BindableMapView() {
         Viewport.ViewportChanged += Viewport_ViewportChanged;
      }

      private void OnMyLocationChanged() {
         MyLocationLayer.UpdateMyLocation(MyLocation);
         if (TrackMyLocation) {
            GoToMyLocation();
         }
      }

      private void Viewport_ViewportChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
         if (e.PropertyName == nameof(Resolution)) {
            var resolution = Viewport.Resolution;

            resolution = Math.Max(resolution, MinResolution);
            resolution = Math.Min(resolution, MaxResolution);

            Resolution = resolution;
         } else if (e.PropertyName == nameof(Rotation)) {
            Rotation = Viewport.Rotation;
         }
      }

      private void OnBindableMapChanged() {
         try {
            if (BindableMap is null) {
               Map.Layers.Clear();
               Map.ClearCache();
               Map = new Map();
            } else {
               Map = BindableMap;
            }
         } catch (Exception ex) {
            var errorService =
               Prism.PrismApplicationBase.Current?.Container?.CurrentScope?.Resolve(typeof(IErrorService)) as IErrorService;
            errorService?.HandleErrorAsync(ex);
         }
      }

      private void OnResolutionChanged() {
         try {
            Navigator.ZoomTo(Math.Max(Resolution, 0.2));
         } catch (Exception ex) {
            var errorService =
               Prism.PrismApplicationBase.Current?.Container?.CurrentScope?.Resolve(typeof(IErrorService)) as IErrorService;
            errorService?.HandleErrorAsync(ex);
         }
      }

      private void OnRotationChanged() {
         try {
            Navigator.RotateTo(Rotation);
         } catch (Exception ex) {
            var errorService =
               Prism.PrismApplicationBase.Current?.Container?.CurrentScope?.Resolve(typeof(IErrorService)) as IErrorService;
            errorService?.HandleErrorAsync(ex);
         }
      }

      private bool IsValidResolution(object v) {
         if (v is double d) {
            return MinResolution <= d && d <= MaxResolution;
         }

         return false;
      }
      private bool IsValidMinResolution(object v) {
         if (v is double d) {
            return 0 < d && d < MaxResolution;
         }

         return false;
      }

      private bool IsValidMaxResolution(object v) {
         if (v is double d) {
            return d > MinResolution;
         }

         return false;
      }

      private void ONTrackMyLocationChanged() {
         if (TrackMyLocation) {
            GoToMyLocation();
         }
      }

      private void GoToMyLocation() {
         MyLocationFollow = true;
         MyLocationFollow = false;
      }
   }
}
