using Mapsui;
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
         if (BindableMap is null) {
            this.Map.Layers.Clear();
            this.Map.ClearCache();
            this.Map = new Map();
         } else {
            this.Map = BindableMap;
         }
         this.BindableMyLocationLayer = this.MyLocationLayer;
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
   }
}
