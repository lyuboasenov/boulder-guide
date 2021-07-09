namespace BoulderGuide.DTOs {
   public class Ellipse : Shape {
      public RelativePoint Center { get; set; } = new RelativePoint();
      public RelativePoint Radius { get; set; } = new RelativePoint();
   }
}
