﻿using BoulderGuide.DTOs;
using BoulderGuide.ImageUtils;
using Newtonsoft.Json;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Path = BoulderGuide.DTOs.Path;
using Size = BoulderGuide.ImageUtils.Size;

namespace BoulderGuide.Wpf {
   /// <summary>
   /// Interaction logic for RouteWindow.xaml
   /// </summary>
   public partial class RouteWindow : Window {
      private string path;
      private Topo selectedSchemas;
      private readonly List<Topo> schemas = new List<Topo>();
      private readonly List<RelativePoint> currentPath = new List<RelativePoint>();
      private Shape currentShape;
      private Size currentImageSize;
      private RouteDTO route;

      public RouteWindow() {
         InitializeComponent();
      }

      public RouteWindow(string path) {
         InitializeComponent();
         this.path = path;

         var fi = new FileInfo(path);

         route = JsonConvert.DeserializeObject<RouteDTO>(File.ReadAllText(path), Shape.StandardJsonConverter);

         txtId.Text = route.Id;
         txtName.Text = route.Name;
         txtInfo.Text = route.Info;
         txtTags.Text = string.Join(',', route.Tags);
         txtVideos.Text = string.Join(Environment.NewLine, route.Videos);
         foreach (var item in lstGrade.Items) {
            if (item is ComboBoxItem ci &&
               double.Parse(ci.DataContext.ToString()) == route.Difficulty) {
               lstGrade.SelectedItem = item;
            }
         }

         txtLocation.Text = route.Location?.ToString();

         foreach (var schema in route.Topos) {
            schema.Id = System.IO.Path.Combine(fi.Directory.FullName, schema.Id);
            schemas.Add(schema);
         }
         InitializeImageList();
         InitializeShapeList();
      }

      private void btnSave_Click(object sender, RoutedEventArgs e) {
         if (string.IsNullOrEmpty(path)) {
            // Create OpenFileDialog
            Microsoft.Win32.SaveFileDialog saveFileDlg = new Microsoft.Win32.SaveFileDialog();
            saveFileDlg.Filter = "Climbing map route file | *.json";

            // Launch OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = saveFileDlg.ShowDialog();
            if (result == true) {
               path = saveFileDlg.FileName;
            } else {
               return;
            }
         }

         var fi = new FileInfo(path);
         SaveRoute(fi.Directory);

         Close();
      }

      private void SaveRoute(DirectoryInfo saveDirectory) {
         var result = route ?? new RouteDTO();
         result.Id = txtId.Text;
         result.Name = txtName.Text;
         result.Info = txtInfo.Text;
         result.Difficulty = double.Parse((lstGrade.SelectedItem as ComboBoxItem).DataContext.ToString());
         result.Tags = txtTags.Text.Split(',', StringSplitOptions.RemoveEmptyEntries);
         result.Videos = txtVideos.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

         if (txtLocation.Text.Length > 0) {
            result.Location = new Location(txtLocation.Text);
         } else {
            result.Location = new Location();
         }

         var name = result.Id.Substring(result.Id.LastIndexOf('/') + 1).ToLowerInvariant();
         int counter = 0;

         var schemaList = new List<Topo>();
         foreach (var schema in schemas) {
            var fi = new FileInfo(schema.Id);
            string id = $"{name}_{counter++}{fi.Extension.ToLowerInvariant()}";

            if (fi.Directory.FullName != saveDirectory.FullName) {
               while (true) {
                  try {
                     File.Copy(
                        fi.FullName,
                        System.IO.Path.Combine(saveDirectory.FullName, id));
                     break;
                  } catch (IOException) {
                     // retry if files already exist
                     id = $"{name}_{counter++}{fi.Extension.ToLowerInvariant()}";
                  }
               }
            }

            schemaList.Add(new Topo() {
               Id = id,
               Shapes = schema.Shapes.ToArray()
            });
         }

         result.Topos = schemaList.ToArray();

         File.WriteAllText(System.IO.Path.Combine(saveDirectory.FullName, $"{name}.json"), JsonConvert.SerializeObject(result, Formatting.Indented));
      }

      private void lstImages_SelectionChanged(object sender, SelectionChangedEventArgs e) {
         // Load schema
         selectedSchemas = lstImages.SelectedItem as Topo;
         btnRemoveImage.IsEnabled = true;
         InitializeShapeList();
         skCanvas.InvalidateVisual();
      }

      private void btnAddImage_Click(object sender, RoutedEventArgs e) {
         // Create OpenFileDialog
         Microsoft.Win32.OpenFileDialog saveFileDlg = new Microsoft.Win32.OpenFileDialog();
         saveFileDlg.Filter = "Image |*.jpg;*.jpeg;*.JPG;*.JPEG";

         // Launch OpenFileDialog by calling ShowDialog method
         Nullable<bool> result = saveFileDlg.ShowDialog();
         if (result == true) {
            schemas.Add(new Topo() {
               Id = saveFileDlg.FileName
            });
            InitializeImageList();
         }
      }

      private void btnRemoveImage_Click(object sender, RoutedEventArgs e) {
         if (lstImages.SelectedItem is Topo schema) {
            schemas.Remove(schema);
            selectedSchemas = null;
            btnRemoveImage.IsEnabled = false;
            lstImages.SelectedItem = null;
            InitializeImageList();
         }
      }

      private void InitializeImageList() {
         lstImages.Items.Clear();
         foreach (var schema in schemas) {
            lstImages.Items.Add(schema);
         }
      }

      private void skCanvas_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e) {
         currentImageSize = new Size();

         if (!string.IsNullOrEmpty(selectedSchemas?.Id)) {
            using (var bitmap = SkiaSharpExtensions.LoadBitmap(selectedSchemas?.Id, skCanvas.CanvasSize.Width, skCanvas.CanvasSize.Height))
            using (var paint = new SKPaint {
               FilterQuality = SKFilterQuality.High, // high quality scaling
               IsAntialias = true
            }) {
               currentImageSize = new Size(bitmap.Width, bitmap.Height);
               e.Surface.Canvas.DrawBitmap(bitmap, 0, 0, paint);
            }
         }

         var shapes = new List<Shape>();
         if (null != selectedSchemas?.Shapes) {
            shapes.AddRange(selectedSchemas.Shapes);
         }
         if (null != currentShape) {
            shapes.Add(currentShape);
         }

         foreach (Shape shape in shapes) {
            shape.Draw(e.Surface.Canvas, currentImageSize, new Size(0, 0));
         }
      }

      private void skCanvas_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
         if (currentShape is Ellipse el) {
            try {
               el.Center = GetImageRelativePosition(e);
            } catch (ArgumentException) {
               // if point is not valid we do nothing
            }
         }
      }

      private void skCanvas_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e) {
         try {
            if (currentShape is Ellipse el) {
               el.Radius = GetImageRelativePosition(e);
               btnAddEllipse.IsChecked = false;
            } else if (currentShape is Path p) {
               currentPath.Add(GetImageRelativePosition(e));
               p.Points = currentPath.ToArray();
               btnUndo.IsEnabled = true;
            }
         } catch (ArgumentException) {
            // if point is not valid we do nothing
         }


         skCanvas.InvalidateVisual();
      }

      private void skCanvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e) {
         if (currentShape is Ellipse el) {
            try {
               el.Radius = GetImageRelativePosition(e);
            } catch (ArgumentException) {
               // if point is not valid we do nothing
            }
         }

         skCanvas.InvalidateVisual();
      }

      private RelativePoint GetImageRelativePosition(System.Windows.Input.MouseEventArgs e) {
         Point position = e.GetPosition(skCanvas);

         // added ratio calculation because canvas control is smalle than
         // the actual canvas
         double widthRatio = currentImageSize.Width / skCanvas.ActualWidth;
         double heightRatio = currentImageSize.Height / skCanvas.ActualHeight;
         var ratio = Math.Max(widthRatio, heightRatio);

         position.X /= currentImageSize.Width / ratio;
         position.Y /= currentImageSize.Height / ratio;

         return position.ToRelativePoint();
      }

      private void btnAddPath_Checked(object sender, RoutedEventArgs e) {
         btnAddEllipse.IsChecked = false;
         currentShape = new Path();
         currentPath.Clear();
         btnSave.IsEnabled = false;
      }

      private void btnAddPath_Unchecked(object sender, RoutedEventArgs e) {
         CommitCurrentShape();
         btnUndo.IsEnabled = false;
      }

      private void btnAddEllipse_Checked(object sender, RoutedEventArgs e) {
         btnAddPath.IsChecked = false;
         currentShape = new Ellipse() { Center = null };
         btnSave.IsEnabled = false;
      }

      private void btnAddEllipse_Unchecked(object sender, RoutedEventArgs e) {
         CommitCurrentShape();
      }

      private void CommitCurrentShape() {
         selectedSchemas.Shapes = selectedSchemas.Shapes.Append(currentShape).ToArray();
         InitializeShapeList();
         currentShape = null;
         skCanvas.InvalidateVisual();
         btnSave.IsEnabled = true;
      }

      private void btnUndo_Click(object sender, RoutedEventArgs e) {
         if (currentShape is Path p && currentPath.Count > 0) {
            currentPath.RemoveAt(currentPath.Count - 1);
            p.Points = currentPath.ToArray();
            skCanvas.InvalidateVisual();
         }
      }

      private void btnRemoveShape_Click(object sender, RoutedEventArgs e) {
         if (lstShapes.SelectedItem is Shape shape) {
            var list = new List<Shape>(selectedSchemas.Shapes);
            list.Remove(shape);
            selectedSchemas.Shapes = list.ToArray();
            skCanvas.InvalidateVisual();
            InitializeShapeList();
         }
      }

      private void InitializeShapeList() {
         lstShapes.Items.Clear();
         foreach (var shape in selectedSchemas?.Shapes ?? Enumerable.Empty<Shape>()) {
            lstShapes.Items.Add(shape);
         }
      }
   }
}
