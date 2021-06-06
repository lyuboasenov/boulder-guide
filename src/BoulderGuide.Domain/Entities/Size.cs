namespace BoulderGuide.Domain.Entities {
   public class Size {
      public Size() {
         Width = 0;
         Height = 0;
      }

      public Size(int width, int height) {
         Width = width;
         Height = height;
      }

      public double Width { get; set; }
      public double Height { get; set; }
   }
}
