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


      public Map BindableMap {
         get { return (Map) GetValue(BindableMapProperty); }
         set { SetValue(BindableMapProperty, value); }
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
            BindableMyLocationLayer = MyLocationLayer;
         } catch (Exception ex) {
            var errorService =
               Prism.PrismApplicationBase.Current?.Container?.CurrentScope?.Resolve(typeof(IErrorService)) as IErrorService;
            errorService?.HandleError(ex);
         }
      }

      public static readonly BindableProperty BindableMyLocationLayerProperty =
         BindableProperty.Create(
            nameof(BindableMyLocationLayer),
            typeof(Mapsui.UI.Objects.MyLocationLayer),
            typeof(BindableMapView),
            null);

      public Mapsui.UI.Objects.MyLocationLayer BindableMyLocationLayer {
         get { return (Mapsui.UI.Objects.MyLocationLayer) GetValue(BindableMyLocationLayerProperty); }
         private set { SetValue(BindableMyLocationLayerProperty, value); }
      }

      public static readonly BindableProperty ResolutionProperty =
         BindableProperty.Create(
            nameof(Resolution),
            typeof(double),
            typeof(BindableMapView),
            0d,
            propertyChanged: (b, _, __) => {
               ((BindableMapView) b).OnResolutionChanged();
            });

      public double Resolution {
         get { return (double) GetValue(ResolutionProperty); }
         set { SetValue(ResolutionProperty, value); }
      }

      private void OnResolutionChanged() {
         try {
            Navigator.ZoomTo(Resolution);
         } catch (Exception ex) {
            var errorService =
               Prism.PrismApplicationBase.Current?.Container?.CurrentScope?.Resolve(typeof(IErrorService)) as IErrorService;
            errorService?.HandleError(ex);
         }
      }

      public static new readonly BindableProperty RotationProperty =
         BindableProperty.Create(
            nameof(Rotation),
            typeof(double),
            typeof(BindableMapView),
            0d,
            propertyChanged: (b, _, __) => {
               ((BindableMapView) b).OnRotationChanged();
            });

      public new double Rotation {
         get { return (double) GetValue(RotationProperty); }
         set { SetValue(RotationProperty, value); }
      }

      private void OnRotationChanged() {
         try {
            Navigator.RotateTo(Rotation);
         } catch (Exception ex) {
            var errorService =
               Prism.PrismApplicationBase.Current?.Container?.CurrentScope?.Resolve(typeof(IErrorService)) as IErrorService;
            errorService?.HandleError(ex);
         }
      }

      public BindableMapView() {
         Viewport.ViewportChanged += Viewport_ViewportChanged;
      }

      private void Viewport_ViewportChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
         if (e.PropertyName == nameof(Resolution)) {
            Resolution = Viewport.Resolution;
         } else if (e.PropertyName == nameof(Rotation)) {
            Rotation = Viewport.Rotation;
         }
      }
   }
}
