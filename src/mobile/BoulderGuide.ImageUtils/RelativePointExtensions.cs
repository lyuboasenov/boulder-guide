using BoulderGuide.DTOs;

namespace BoulderGuide.ImageUtils {
   public static class RelativePointExtensions {
      public static ImagePoint ToImagePoint(this RelativePoint relativePoint, Size imageSize, Size offset) {
         return new ImagePoint() {
            X = relativePoint.X * imageSize.Width + offset.Width,
            Y = relativePoint.Y * imageSize.Height + offset.Height
         };
      }
   }
}
