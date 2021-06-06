using Mapsui;
using Mapsui.Layers;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace BoulderGuide.Mobile.Forms.Views {
   public class BindableMapView : Mapsui.UI.Forms.MapView {
      public static readonly BindableProperty BindableMapProperty =
         BindableProperty.Create(
            nameof(BindableMap),
            typeof(Map),
            typeof(BindableMapView),
            null,
            propertyChanged: (b, o, n) => { ((BindableMapView) b).OnBindableMapChanged(); });


      public Map BindableMap {
         get { return (Map) GetValue(BindableMapProperty); }
         set { SetValue(BindableMapProperty, value); }
      }

      private void OnBindableMapChanged() {
         this.Map = BindableMap;
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
