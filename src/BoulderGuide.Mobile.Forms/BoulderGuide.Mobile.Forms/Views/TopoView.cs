using BoulderGuide.DTOs;
using BoulderGuide.ImageUtils;
using BoulderGuide.Mobile.Forms.Domain;
using SkiaSharp.Views.Forms;
using System.Linq;
using Xamarin.Forms;

namespace BoulderGuide.Mobile.Forms.Views {
   public class TopoView : SKCanvasView {
      public static readonly BindableProperty TopoProperty =
         BindableProperty.Create(
            nameof(Topo),
            typeof(Topo),
            typeof(Topo),
            null,
            propertyChanged: (bindable, _, __) => {
               (bindable as TopoView)?.SetImageLocalPath();
               (bindable as TopoView)?.InvalidateSurface();
            });

      public Topo Topo {
         get { return (Topo) GetValue(TopoProperty); }
         set { SetValue(TopoProperty, value); }
      }

      public static readonly BindableProperty RouteInfoProperty =
         BindableProperty.Create(
            nameof(RouteInfo),
            typeof(RouteInfo),
            typeof(RouteInfo),
            null,
            propertyChanged: (bindable, _, __) => {
               (bindable as TopoView)?.SetImageLocalPath();
               (bindable as TopoView)?.InvalidateSurface();
            });

      public RouteInfo RouteInfo {
         get { return (RouteInfo) GetValue(RouteInfoProperty); }
         set { SetValue(TopoProperty, value); }
      }

      private Domain.Image image;

      protected override void OnPaintSurface(SKPaintSurfaceEventArgs e) {
         base.OnPaintSurface(e);

         using (var imageStream = image?.GetStream()) {
            e.Surface.Canvas.DrawTopo(imageStream, Topo?.Shapes ?? Enumerable.Empty<Shape>());
         }
      }

      private void SetImageLocalPath() {
         if (Topo != null) {
            image = RouteInfo?.
               Route?.
               Images?.
               FirstOrDefault(i => i.LocalPath.EndsWith("/" + Topo.Id));
         }
      }
   }
}
