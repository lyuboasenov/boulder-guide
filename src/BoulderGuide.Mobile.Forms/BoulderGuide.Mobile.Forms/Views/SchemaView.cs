using BoulderGuide.DTOs;
using BoulderGuide.ImageUtils;
using BoulderGuide.Mobile.Forms.Services.Data.Entities;
using SkiaSharp.Views.Forms;
using System.Linq;
using Xamarin.Forms;

namespace BoulderGuide.Mobile.Forms.Views {
   public class SchemaView : SKCanvasView {
      public static readonly BindableProperty SchemaProperty =
         BindableProperty.Create(
            nameof(Schema),
            typeof(Schema),
            typeof(Schema),
            null,
            propertyChanged: (bindable, _, __) => {
               (bindable as SchemaView)?.SetImageLocalPath();
               (bindable as SchemaView)?.InvalidateSurface();
            });

      public Schema Schema {
         get { return (Schema) GetValue(SchemaProperty); }
         set { SetValue(SchemaProperty, value); }
      }

      public static readonly BindableProperty RouteInfoProperty =
         BindableProperty.Create(
            nameof(RouteInfo),
            typeof(RouteInfo),
            typeof(RouteInfo),
            null,
            propertyChanged: (bindable, _, __) => {
               (bindable as SchemaView)?.SetImageLocalPath();
               (bindable as SchemaView)?.InvalidateSurface();
            });

      public RouteInfo RouteInfo {
         get { return (RouteInfo) GetValue(RouteInfoProperty); }
         set { SetValue(SchemaProperty, value); }
      }

      private Services.Data.Entities.Image image;

      protected override void OnPaintSurface(SKPaintSurfaceEventArgs e) {
         base.OnPaintSurface(e);

         using (var imageStream = image?.GetStream()) {
            e.Surface.Canvas.DrawTopo(imageStream, Schema?.Shapes ?? Enumerable.Empty<Shape>());
         }
      }

      private void SetImageLocalPath() {
         if (Schema != null) {
            image = RouteInfo?.
               Route?.
               Images?.
               FirstOrDefault(i => i.LocalPath.EndsWith("/" + Schema.Id));
         }
      }
   }
}
