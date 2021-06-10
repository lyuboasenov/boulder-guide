﻿using System;

namespace BoulderGuide.Domain.Schema {
   public class RelativePoint {
      private double y;
      private double x;

      public double X {
         get { return x; }
         set {
            if (0 > value || value > 1) {
               throw new ArgumentException($"{value} is not valid. X should be between 0 and 1.");
            }
            x = value;
         }
      }
      public double Y {
         get { return y; }
         set {
            if (0 > value || value > 1) {
               throw new ArgumentException($"{value} is not valid. Y should be between 0 and 1.");
            }
            y = value;
         }
      }

      public ImagePoint ToImagePoint(Size imageSize, Size offset) {
         return new ImagePoint() {
            X = X * imageSize.Width + offset.Width,
            Y = Y * imageSize.Height + offset.Height
         };
      }
   }
}