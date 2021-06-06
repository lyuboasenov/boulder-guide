using System;
using System.Collections.Generic;
using System.Text;

namespace BoulderGuide.Domain.Entities {
   public class Schema {
      public string Id { get; set; }
      public Shape[] Shapes { get; set; } = new Shape[0];
   }
}
