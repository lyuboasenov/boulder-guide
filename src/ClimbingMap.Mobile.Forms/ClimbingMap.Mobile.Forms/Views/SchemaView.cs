using ClimbingMap.Domain.Entities;
using ClimbingMap.Domain.Schema;
using ClimbingMap.Mobile.Forms.Services.Data;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.IO;
using System.Linq;
using Xamarin.Forms;

namespace ClimbingMap.Mobile.Forms.Views {
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

      private Domain.Entities.Size imageSize;
      private string imageLocalPath;

      protected override void OnPaintSurface(SKPaintSurfaceEventArgs e) {
         base.OnPaintSurface(e);

         if (File.Exists(imageLocalPath)) {
            using (var bitmap = SkiaSharpExtensions.LoadBitmap(imageLocalPath, CanvasSize.Width, CanvasSize.Height))
            using (var paint = new SKPaint {
               FilterQuality = SKFilterQuality.High, // high quality scaling
               IsAntialias = true
            }) {
               imageSize = new Domain.Entities.Size(bitmap.Width, bitmap.Height);
               e.Surface.Canvas.DrawBitmap(bitmap, 0, 0, paint);
            }

            foreach (var shape in Schema.Shapes ?? Enumerable.Empty<Shape>()) {
               shape.Draw(e.Surface.Canvas, imageSize);
            }
         }
      }

      private void SetImageLocalPath() {
         if (Schema != null && RouteInfo != null) {
            var relativePath = RouteInfo.Images.First(i => i.EndsWith("/" + Schema.Id));
            imageLocalPath = RouteInfo.GetImageLocalPath(relativePath);
         }
      }
   }
}
