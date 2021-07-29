namespace BoulderGuide.ImageUtils {
   public class Size {
      public Size() {
         Width = 0;
         Height = 0;
      }

      public Size(double width, double height) {
         Width = width;
         Height = height;
      }

      public double Width { get; set; }
      public double Height { get; set; }
   }
}
