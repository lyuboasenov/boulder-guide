using BoulderGuide.DTOs;
using BoulderGuide.ImageUtils;
using BoulderGuide.Mobile.Forms.Services.Data;
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

      private string imageLocalPath;

      protected override void OnPaintSurface(SKPaintSurfaceEventArgs e) {
         base.OnPaintSurface(e);

         e.Surface.Canvas.DrawSchema(imageLocalPath, Schema?.Shapes ?? Enumerable.Empty<Shape>());
      }

      private void SetImageLocalPath() {
         if (Schema != null) {
            imageLocalPath = RouteInfo?.
               Route?.
               Images?.
               Select(i => i.LocalPath)?.
               FirstOrDefault(i => i.EndsWith("/" + Schema.Id));
         }
      }
   }
}
