﻿using System;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace BoulderGuide.Mobile.Forms.Views {
   public class PinchToZoomContainer : ContentView {

      public static readonly BindableProperty ContentScaleProperty =
         BindableProperty.Create(
            nameof(ContentScale),
            typeof(double),
            typeof(double),
            1d,
            propertyChanged: (bindable, _, __) => {
               var control = bindable as PinchToZoomContainer;

               control.Content.Scale = control.ContentScale;
               control.Content.TranslationX = (control.Width - control.Content.Width * control.ContentScale) / 2;
               control.Content.TranslationY = (control.Height - control.Content.Height * control.ContentScale) / 2;
            });

      public double ContentScale {
         get { return (double) GetValue(ContentScaleProperty); }
         set { SetValue(ContentScaleProperty, value); }
      }

      private double startScale = 1;
      private double xOffset = 0;
      private double yOffset = 0;
      private double lastX;
      private double lastY;

      public PinchToZoomContainer() {
         var pinchGesture = new PinchGestureRecognizer();
         pinchGesture.PinchUpdated += OnPinchUpdated;
         GestureRecognizers.Add(pinchGesture);

         var pan = new PanGestureRecognizer();
         pan.PanUpdated += OnPanUpdated;
         GestureRecognizers.Add(pan);

         IsClippedToBounds = true;
      }

      private void OnPanUpdated(object sender, PanUpdatedEventArgs e) {
         if (ContentScale > 1) {
            switch (e.StatusType) {
               case GestureStatus.Started:
                  lastX = Content.TranslationX;
                  lastY = Content.TranslationY;
                  break;
               case GestureStatus.Running:
                  double targetX = lastX + e.TotalX * ContentScale;
                  double targetY = lastY + e.TotalY * ContentScale;

                  Content.TranslationX = targetX.Clamp(-Content.Width * (ContentScale - 1), 0);
                  Content.TranslationY = targetY.Clamp(-Content.Height * (ContentScale - 1), 0);
                  break;
            }
         }
      }

      void OnPinchUpdated(object sender, PinchGestureUpdatedEventArgs e) {
         if (e.Status == GestureStatus.Started) {
            // Store the current scale factor applied to the wrapped user interface element,
            // and zero the components for the center point of the translate transform.
            startScale = Content.Scale;
            Content.AnchorX = 0;
            Content.AnchorY = 0;
         }
         if (e.Status == GestureStatus.Running) {
            // Calculate the scale factor to be applied.
            ContentScale += (e.Scale - 1) * startScale;
            ContentScale = Math.Max(1, ContentScale);

            // The ScaleOrigin is in relative coordinates to the wrapped user interface element,
            // so get the X pixel coordinate.
            double renderedX = Content.X + xOffset;
            double deltaX = renderedX / Width;
            double deltaWidth = Width / (Content.Width * startScale);
            double originX = (e.ScaleOrigin.X - deltaX) * deltaWidth;

            // The ScaleOrigin is in relative coordinates to the wrapped user interface element,
            // so get the Y pixel coordinate.
            double renderedY = Content.Y + yOffset;
            double deltaY = renderedY / Height;
            double deltaHeight = Height / (Content.Height * startScale);
            double originY = (e.ScaleOrigin.Y - deltaY) * deltaHeight;

            // Calculate the transformed element pixel coordinates.
            double targetX = xOffset - (originX * Content.Width) * (ContentScale - startScale);
            double targetY = yOffset - (originY * Content.Height) * (ContentScale - startScale);

            // Apply translation based on the change in origin.
            Content.TranslationX = targetX.Clamp(-Content.Width * (ContentScale - 1), 0);
            Content.TranslationY = targetY.Clamp(-Content.Height * (ContentScale - 1), 0);

            // Apply scale factor
            Content.Scale = ContentScale;
         }
         if (e.Status == GestureStatus.Completed) {
            // Store the translation delta's of the wrapped user interface element.
            xOffset = Content.TranslationX;
            yOffset = Content.TranslationY;
         }
      }
   }
}
