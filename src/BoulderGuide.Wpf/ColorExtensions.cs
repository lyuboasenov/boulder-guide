using SkiaSharp;
using System.Windows.Media;

namespace BoulderGuide.Wpf {
   internal static class ColorExtensions {
      public static SKColor ToSkColor(this Color color) {
         return new SKColor(color.R, color.G, color.B, color.A);
      }
   }
}
