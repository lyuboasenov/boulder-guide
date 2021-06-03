using ClimbingMap.Domain.Entities;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ClimbingMap.Domain.Schema {
   public static class SkiaSharpExtensions {

      private static SKColor defaultColor = SKColors.White;

      public static SKPaint ToSKPaint(this SKColor color) {
         return new SKPaint {
            Style = SKPaintStyle.Stroke,
            Color = color,
            StrokeWidth = 0,
            StrokeCap = SKStrokeCap.Round,
            StrokeJoin = SKStrokeJoin.Round
         };
      }

      public static SKBitmap HandleOrientation(this SKBitmap bitmap, SKEncodedOrigin orientation) {
         switch (orientation) {
            case SKEncodedOrigin.BottomRight:

               using (var surface = new SKCanvas(bitmap)) {
                  surface.RotateDegrees(180, bitmap.Width / 2, bitmap.Height / 2);
                  surface.DrawBitmap(bitmap.Copy(), 0, 0);
               }

               break;

            case SKEncodedOrigin.RightTop:
               var workingCopy = new SKBitmap(bitmap.Height, bitmap.Width);

               using (var surface = new SKCanvas(workingCopy)) {
                  surface.Translate(workingCopy.Width, 0);
                  surface.RotateDegrees(90);
                  surface.DrawBitmap(bitmap, 0, 0);
               }

               bitmap.Dispose();
               bitmap = workingCopy;
               break;

            case SKEncodedOrigin.LeftBottom:
               var workingCopy2 = new SKBitmap(bitmap.Height, bitmap.Width);

               using (var surface = new SKCanvas(workingCopy2)) {
                  surface.Translate(0, workingCopy2.Height);
                  surface.RotateDegrees(270);
                  surface.DrawBitmap(bitmap, 0, 0);
               }

               bitmap.Dispose();
               bitmap = workingCopy2;
               break;
         }

         return bitmap;
      }

      public static SKBitmap LoadBitmap(string bitmapPath, double width, double height) {
         using (var input = File.OpenRead(bitmapPath))                 // load the file
         using (var inputStream = new SKManagedStream(input))          // create a sream SkiaSharp uses
         using (var codec = SKCodec.Create(inputStream)) {             // get the decoder

            var bitmap = SKBitmap.Decode(codec).HandleOrientation(codec.EncodedOrigin);

            // Determine scaling factor
            var bitmapAspectRatio = (double) bitmap.Width / bitmap.Height;
            double factor = bitmapAspectRatio > 1 ? width / bitmap.Width : height / bitmap.Height;

            var info = new SKImageInfo((int) (bitmap.Width * factor), (int) (bitmap.Height * factor));
            bitmap = bitmap.Resize(info, SKFilterQuality.High);

            return bitmap;
         }
      }

      public static void DrawPath(this SKCanvas canvas, IEnumerable<ImagePoint> path) {
         canvas.DrawPath(path, defaultColor);
      }

      public static void DrawPath(this SKCanvas canvas, IEnumerable<ImagePoint> path, SKColor color, ImagePoint originPoint = null, double scaleFactor = 1) {
         canvas.DrawPath(path.ConvertToSKPath(originPoint, scaleFactor), color.ToSKPaint());
      }

      public static void DrawEllipse(this SKCanvas canvas, ImagePoint center, ImagePoint radius) {
         canvas.DrawEllipse(center, radius, defaultColor);
      }

      public static void DrawEllipse(this SKCanvas canvas, ImagePoint center, ImagePoint radius, SKColor color) {
         canvas.DrawOval(
            (float) center.X,
            (float) center.Y,
            (float) Math.Abs(center.X - radius.X),
            (float) Math.Abs(center.Y - radius.Y),
            color.ToSKPaint());
      }

      /// <summary>
      /// Calculates distance between two skpoints.
      /// </summary>
      /// <param name="p1"></param>
      /// <param name="p2"></param>
      /// <returns></returns>
      public static double Distance(this SKPoint p1, SKPoint p2) {
         return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
      }

      /// <summary>
      /// Converts Path to SKPath
      /// </summary>
      /// <param name="path">Path to be converted</param>
      /// <param name="originPoint">Origin point if transposition is to be made</param>
      /// <param name="scaleFactor">Scale factor if scaling is to be made</param>
      /// <returns></returns>
      public static SKPath ConvertToSKPath(this IEnumerable<ImagePoint> path, ImagePoint originPoint = null, double scaleFactor = 1) {
         // var normalizedPath = new SKPath();
         var convertedPath = path.Select(p => p.ToSKPoint(originPoint, scaleFactor)).ToArray();
         return convertedPath.Length == 0 ? new SKPath() : CreateSpline(null, convertedPath);
      }

      /// <summary>
      /// Converts a Point to SKPoint
      /// </summary>
      /// <param name="point">Point to be converted</param>
      /// <param name="originPoint">Origin point if transposition is to be made</param>
      /// <param name="scaleFactor">Scale factor if scaling is to be made</param>
      /// <returns></returns>
      public static SKPoint ToSKPoint(this ImagePoint point, ImagePoint originPoint = null, double scaleFactor = 1) {
         double originX = originPoint?.X ?? 0;
         double originY = originPoint?.Y ?? 0;
         return new SKPoint((float) ((point.X - originX) * scaleFactor), (float) ((point.Y - originY) * scaleFactor));
      }

      public static ImagePoint ToPoint(this SKPoint self) {
         return new ImagePoint() { X = self.X, Y = self.Y };
      }

      /// <summary>
      /// Creates a Spline path through a given set of points.
      /// https://github.com/PeterWaher/IoTGateway/blob/master/Script/Waher.Script.Graphs/Functions/Plots/Plot2DCurve.cs
      /// </summary>
      /// <param name="AppendTo">Spline should be appended to this path. If null, a new path will be created.</param>
      /// <param name="Points">Points between which the spline will be created.</param>
      /// <returns>Spline path.</returns>
      private static SKPath CreateSpline(SKPath AppendTo, params SKPoint[] Points) {
         int i, c = Points.Length;
         if (c == 0)
            throw new ArgumentException("No points provided.", nameof(Points));

         if (AppendTo == null) {
            AppendTo = new SKPath();
            AppendTo.MoveTo(Points[0]);
         } else
            AppendTo.LineTo(Points[0]);

         if (c == 1)
            return AppendTo;

         if (c == 2) {
            AppendTo.LineTo(Points[1]);
            return AppendTo;
         }

         var V = new double[c];

         for (i = 0; i < c; i++)
            V[i] = Points[i].X;

         GetCubicBezierCoefficients(V, out var Ax, out var Bx);

         for (i = 0; i < c; i++)
            V[i] = Points[i].Y;

         GetCubicBezierCoefficients(V, out var Ay, out var By);

         for (i = 0; i < c - 1; i++) {
            AppendTo.CubicTo((float) Ax[i], (float) Ay[i], (float) Bx[i], (float) By[i],
               Points[i + 1].X, Points[i + 1].Y);
         }

         return AppendTo;
      }

      /// <summary>
      /// Gets a set of coefficients for cubic Bezier curves, forming a spline, one coordinate at a time.
      /// https://github.com/PeterWaher/IoTGateway/blob/master/Script/Waher.Script.Graphs/Functions/Plots/Plot2DCurve.cs
      /// </summary>
      /// <param name="V">One set of coordinates.</param>
      /// <param name="A">Corresponding coefficients for first control points.</param>
      /// <param name="B">Corresponding coefficients for second control points.</param>
      private static void GetCubicBezierCoefficients(double[] V, out double[] A, out double[] B) {
         // Calculate Spline between points P[0], ..., P[N].
         // Divide into segments, B[0], ...., B[N-1] of cubic Bezier curves:
         //
         // B[i](t) = (1-t)³P[i] + 3t(1-t)²A[i] + 3t²(1-t)B[i] + t³P[i+1]
         //
         // B'[i](t) = (-3+6t-3t²)P[i]+(3-12t+9t²)A[i]+(6t-9t²)B[i]+3t²P[i+1]
         // B"[i](t) = (6-6t)P[i]+(-12+18t)A[i]+(6-18t)B[i]+6tP[i+1]
         //
         // Choose control points A[i] and B[i] such that:
         //
         // B'[i](1) = B'[i+1](0) => A[i+1]+B[i]=2P[i+1], i<N		(eq 1)
         // B"[1](1) = B"[i+1](0) => A[i]-2B[i]+2A[i+1]-B[i+1]=0		(eq 2)
         //
         // Also add the boundary conditions:
         //
         // B"[0](0)=0 => 2A[0]-B[0]=P[0]			(eq 3)
         // B"[N-1](1)=0 => -A[N-1]+2B[N-1]=P[N]		(eq 4)
         //
         // Method solves this linear equation for one coordinate of A[i] and B[i] at a time.
         //
         // First, the linear equation, is reduced downwards. Only coefficients close to
         // the diagonal, and in the right-most column need to be processed. Furthermore,
         // we don't have to store values we know are zero or one. Since number of operations
         // depend linearly on number of vertices, algorithm is O(N).

         var N = V.Length - 1;
         var N2 = N << 1;
         var i = 0;
         var j = 0;
         double r11, r12, r15;               // r13 & r14 always 0.
         double r22, r23, r25;               // r21 & r24 always 0 for all except last equation, where r21 is -1.
         double r31, r32, r33, r34, r35;
         var Rows = new double[N2, 3];
         double a;

         A = new double[N];
         B = new double[N];

         r11 = 2;        // eq 3
         r12 = -1;
         r15 = V[j++];

         r22 = 1;        // eq 1
         r23 = 1;
         r25 = 2 * V[j++];

         r31 = 1;        // eq 2
         r32 = -2;
         r33 = 2;
         r34 = -1;
         r35 = 0;

         while (true) {
            a = 1 / r11;
            r11 = 1;
            r12 *= a;
            r15 *= a;

            // r21 is always 0. No need to eliminate column.
            // r22 is always 1. No need to scale row.

            // r31 is always 1 at this point.
            r31 -= r11;
            r32 -= r12;
            r35 -= r15;

            if (r32 != 0) {
               r33 -= r32 * r23;
               r35 -= r32 * r25;
               r32 = 0;
            }

            // r33 is always 0.

            // r11 always 1.
            Rows[i, 0] = r12;
            Rows[i, 1] = 0;
            Rows[i, 2] = r15;
            i++;

            // r21, r24 always 0.
            Rows[i, 0] = r22;
            Rows[i, 1] = r23;
            Rows[i, 2] = r25;
            i++;

            if (i >= N2 - 2)
               break;

            r11 = r33;
            r12 = r34;
            r15 = r35;

            r22 = 1;        // eq 1
            r23 = 1;
            r25 = 2 * V[j++];

            r31 = 1;        // eq 2
            r32 = -2;
            r33 = 2;
            r34 = -1;
            r35 = 0;
         }

         r11 = r33;
         r12 = r34;
         r15 = r35;

         //r21 = -1;		// eq 4
         r22 = 2;
         r23 = 0;
         r25 = V[j++];

         a = 1 / r11;
         r11 = 1;
         r12 *= a;
         r15 *= a;

         //r21 += r11;
         r22 += r12;
         r25 += r15;

         r25 /= r22;
         r22 = 1;

         // r11 always 1.
         Rows[i, 0] = r12;
         Rows[i, 1] = 0;
         Rows[i, 2] = r15;
         i++;

         // r21 and r24 always 0.
         Rows[i, 0] = r22;
         Rows[i, 1] = r23;
         Rows[i, 2] = r25;
         i++;

         // Then eliminate back up:

         j--;
         while (i > 0) {
            i--;
            if (i < N2 - 1) {
               a = Rows[i, 1];
               if (a != 0) {
                  Rows[i, 1] = 0;
                  Rows[i, 2] -= a * Rows[i + 1, 2];
               }
            }

            B[--j] = Rows[i, 2];

            i--;
            a = Rows[i, 0];
            if (a != 0) {
               Rows[i, 0] = 0;
               Rows[i, 2] -= a * Rows[i + 1, 2];
            }

            A[j] = Rows[i, 2];
         }
      }
   }
}
