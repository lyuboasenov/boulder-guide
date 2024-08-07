﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BoulderGuide.DTOs;
using BoulderGuide.ImageUtils;
using BoulderGuide.Wpf.ViewModels;

namespace BoulderGuide.Wpf.Views {
   /// <summary>
   /// Interaction logic for RouteView.xaml
   /// </summary>
   public partial class RouteView : UserControl {
      private ImageUtils.Size currentImageSize;
      private ImageUtils.Size canvasSize;
      private RouteViewModel vm;

      public RouteView() {
         InitializeComponent();
         DataContextChanged += RouteView_DataContextChanged;
      }

      private void RouteView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
         if (DataContext is RouteViewModel vm) {
            this.vm = vm;
            vm.UpdateSurface += Vm_UpdateSurface;
         }
      }

      private void Vm_UpdateSurface(object sender, EventArgs e) {
         skCanvas.InvalidateVisual();
      }

      private RelativePoint GetImageRelativePosition(MouseEventArgs e) {
         Point position = e.GetPosition(skCanvas);

         var canvasRatio = canvasSize.Width / skCanvas.ActualWidth;

         position.X *= canvasRatio;
         position.Y *= canvasRatio;

         var offset = new ImageUtils.Size(
                  Math.Max((canvasSize.Width - currentImageSize.Width) / 2, 0),
                  Math.Max((canvasSize.Height - currentImageSize.Height) / 2, 0));

         position.X -= offset.Width;
         position.Y -= offset.Height;

         position.X /= currentImageSize.Width;
         position.Y /= currentImageSize.Height;

         return position.ToRelativePoint();
      }

      private void skCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
         if (vm?.CurrentShape is Ellipse el) {
            try {
               el.Center = GetImageRelativePosition(e);
            } catch (ArgumentException) {
               // if point is not valid we do nothing
            }
         }
      }

      private void skCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
         try {
            if (vm?.CurrentShape is Rectangle rect) {
               rect.Radius = GetImageRelativePosition(e);
               btnAddRectangle.IsChecked = false;
            } else if (vm?.CurrentShape is Ellipse el) {
               el.Radius = GetImageRelativePosition(e);
               btnAddEllipse.IsChecked = false;
            } else if (vm?.CurrentShape is Path p) {
               vm.CurrentPath.Add(GetImageRelativePosition(e));
               p.Points = vm.CurrentPath.ToArray();
               (vm?.UndoCommand as Command)?.RaiseCanExecuteChanged();
            }
         } catch (ArgumentException) {
            // if point is not valid we do nothing
         }

         skCanvas.InvalidateVisual();
      }

      private void skCanvas_MouseMove(object sender, MouseEventArgs e) {
         if (vm?.CurrentShape is Ellipse el) {
            try {
               el.Radius = GetImageRelativePosition(e);
            } catch (ArgumentException) {
               // if point is not valid we do nothing
            }
         }

         skCanvas.InvalidateVisual();
      }

      private void skCanvas_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e) {
         if (DataContext is ViewModels.RouteViewModel vm &&
            !string.IsNullOrEmpty(vm.SelectedTopo?.Id)) {

            var shapes = new List<DTOs.Shape>();
            if (null != vm.SelectedTopo?.Shapes) {
               shapes.AddRange(vm.SelectedTopo.Shapes);
            }
            if (null != vm?.CurrentShape) {
               shapes.Add(vm?.CurrentShape);
            }

            if (System.IO.File.Exists(vm.SelectedTopo.Id)) {
               using (var imgStream = System.IO.File.OpenRead(vm.SelectedTopo.Id)) {
                  canvasSize = new ImageUtils.Size(e.Surface.Canvas.DeviceClipBounds.Width, e.Surface.Canvas.DeviceClipBounds.Height);
                  currentImageSize = e.Surface.Canvas.DrawTopo(imgStream, shapes, 4);
               }
            } else {
               MessageBox.Show($"Image '{vm.SelectedTopo.Id}' not found.");
            }
         }
      }
   }
}
