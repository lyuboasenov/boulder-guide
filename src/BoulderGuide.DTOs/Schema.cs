namespace BoulderGuide.DTOs {
   public class Schema {
      public string Id { get; set; }
      public Shape[] Shapes { get; set; } = new Shape[0];
   }
}
