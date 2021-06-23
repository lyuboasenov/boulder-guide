using BoulderGuide.DTOs;
using BoulderGuide.ImageUtils;
using BoulderGuide.Mobile.Forms.Domain;
using BoulderGuide.Mobile.Forms.Services.Errors;
using SkiaSharp.Views.Forms;
using System;
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
         set { SetValue(RouteInfoProperty, value); }
      }

      private Domain.Image image;

      protected override void OnPaintSurface(SKPaintSurfaceEventArgs e) {
         base.OnPaintSurface(e);

         try {
            using (var imageStream = image?.GetStream()) {
               e.Surface.Canvas.DrawTopo(imageStream, Topo?.Shapes ?? Enumerable.Empty<Shape>());
            }
         } catch (Exception ex) {
            var errorService =
               Prism.PrismApplicationBase.Current?.Container?.CurrentScope?.Resolve(typeof(IErrorService)) as IErrorService;
            errorService?.HandleError(ex);
         }
      }

      private void SetImageLocalPath() {
         if (Topo != null) {
            image = RouteInfo?.
               Route?.
               Images?.
               FirstOrDefault(i => i?.LocalPath?.EndsWith("/" + Topo.Id) ?? false);
         }
      }
   }
}
