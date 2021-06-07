using BoulderGuide.Domain.Entities;
using BoulderGuide.Domain.Schema;
using Newtonsoft.Json;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BoulderGuide.Wpf {
   /// <summary>
   /// Interaction logic for RouteWindow.xaml
   /// </summary>
   public partial class RouteWindow : Window {
      private string path;
      private Schema selectedSchemas;
      private readonly List<Schema> schemas = new List<Schema>();
      private readonly List<RelativePoint> currentPath = new List<RelativePoint>();
      private Shape currentShape;
      private Domain.Entities.Size currentImageSize;

      public RouteWindow() {
         InitializeComponent();
      }

      public RouteWindow(string path) {
         InitializeComponent();
         this.path = path;

         var fi = new FileInfo(path);

         var route = JsonConvert.DeserializeObject<Route>(File.ReadAllText(path), Shape.StandardJsonConverter);

         txtId.Text = route.Id;
         txtName.Text = route.Name;
         txtInfo.Text = route.Info;
         txtTags.Text = string.Join(',', route.Tags);
         foreach (var item in lstGrade.Items) {
            if (item is ComboBoxItem ci &&
               double.Parse(ci.DataContext.ToString()) == route.Difficulty) {
               lstGrade.SelectedItem = item;
            }
         }

         txtLocation.Text = route.Location?.ToString();

         foreach (var schema in route.Schemas) {
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
         var result = new Route() {
            Id = txtId.Text,
            Name = txtName.Text,
            Info = txtInfo.Text,
            Difficulty = double.Parse((lstGrade.SelectedItem as ComboBoxItem).DataContext.ToString()),
            Tags = txtTags.Text.Split(',', StringSplitOptions.RemoveEmptyEntries)
         };


         if (txtLocation.Text.Length > 0) {
            result.Location = new Location(txtLocation.Text);
         } else {
            result.Location = new Location();
         }

         var name = result.Id.Substring(result.Id.LastIndexOf('/') + 1).ToLowerInvariant();
         int counter = 0;

         var schemaList = new List<Schema>();
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

            schemaList.Add(new Schema() {
               Id = id,
               Shapes = schema.Shapes.ToArray()
            });
         }

         result.Schemas = schemaList.ToArray();

         File.WriteAllText(System.IO.Path.Combine(saveDirectory.FullName, $"{name}.json"), JsonConvert.SerializeObject(result, Formatting.Indented));
      }

      private void lstImages_SelectionChanged(object sender, SelectionChangedEventArgs e) {
         tabSchema.IsEnabled = lstImages.SelectedItem != null;
         if (tabSchema.IsEnabled) {
            // Load schema
            selectedSchemas = lstImages.SelectedItem as Schema;
            btnRemoveImage.IsEnabled = true;
            InitializeShapeList();
         }
      }

      private void btnAddImage_Click(object sender, RoutedEventArgs e) {
         // Create OpenFileDialog
         Microsoft.Win32.OpenFileDialog saveFileDlg = new Microsoft.Win32.OpenFileDialog();
         saveFileDlg.Filter = "Image |*.jpg;*.jpeg;*.JPG;*.JPEG";

         // Launch OpenFileDialog by calling ShowDialog method
         Nullable<bool> result = saveFileDlg.ShowDialog();
         if (result == true) {
            schemas.Add(new Schema() {
               Id = saveFileDlg.FileName
            });
            InitializeImageList();
         }
      }

      private void btnRemoveImage_Click(object sender, RoutedEventArgs e) {
         if (lstImages.SelectedItem is Schema schema) {
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
         currentImageSize = new Domain.Entities.Size();

         if (!string.IsNullOrEmpty(selectedSchemas?.Id)) {
            using (var bitmap = SkiaSharpExtensions.LoadBitmap(selectedSchemas?.Id, skCanvas.ActualWidth, skCanvas.ActualHeight))
            using (var paint = new SKPaint {
               FilterQuality = SKFilterQuality.High, // high quality scaling
               IsAntialias = true
            }) {
               currentImageSize = new Domain.Entities.Size(bitmap.Width, bitmap.Height);
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
            shape.Draw(e.Surface.Canvas, currentImageSize);
         }
      }

      private void skCanvas_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
         if (currentShape is Ellipse el) {
            el.Center = GetImageRelativePosition(e);
         }
      }

      private void skCanvas_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e) {
         if (currentShape is Ellipse el) {
            el.Radius = GetImageRelativePosition(e);
            btnAddEllipse.IsChecked = false;
         } else if (currentShape is Domain.Entities.Path p) {
            currentPath.Add(GetImageRelativePosition(e));
            p.Points = currentPath.ToArray();
            btnUndo.IsEnabled = true;
         }

         skCanvas.InvalidateVisual();
      }

      private void skCanvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e) {
         if (currentShape is Ellipse el) {
            el.Radius = GetImageRelativePosition(e);
         }

         skCanvas.InvalidateVisual();
      }

      private RelativePoint GetImageRelativePosition(System.Windows.Input.MouseEventArgs e) {
         Point position = e.GetPosition(skCanvas);
         position.X /= currentImageSize.Width;
         position.Y /= currentImageSize.Height;

         return position.ToRelativePoint();
      }

      private void btnAddPath_Checked(object sender, RoutedEventArgs e) {
         btnAddEllipse.IsChecked = false;
         currentShape = new Domain.Entities.Path();
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
         if (currentShape is Domain.Entities.Path p && currentPath.Count > 0) {
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
