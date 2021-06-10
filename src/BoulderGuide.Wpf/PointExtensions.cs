using BoulderGuide.Domain.Schema;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace BoulderGuide.Wpf {
   internal static class PointExtensions {
      public static RelativePoint ToRelativePoint(this Point point) {
         return new RelativePoint() {
            X = point.X,
            Y = point.Y
         };
      }
   }
}
