using ClimbingMap.Domain.Entities;
using Newtonsoft.Json;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ClimbingMap.Wpf {
   /// <summary>
   /// Interaction logic for RouteWindow.xaml
   /// </summary>
   public partial class RouteWindow : Window {
      private string path;
      private Schema selectedSchemas;
      private readonly List<Schema> schemas = new List<Schema>();
      private readonly List<SchemaPoint> currentPath = new List<SchemaPoint>();
      private Shape currentShape;

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
         txtApproach.Text = route.Approach;
         txtHistory.Text = route.History;
         txtTags.Text = string.Join(',', route.Tags);
         foreach (var item in lstGrade.Items) {
            if (item is ComboBoxItem ci &&
               double.Parse(ci.DataContext.ToString()) == route.Difficulty) {
               lstGrade.SelectedItem = item;
            }
         }
         txtHeight.Text = route.Height.ToString();

         txtLat1.Text = route.Location?.Latitude.ToString();
         txtLon1.Text = route.Location?.Longitude.ToString();

         foreach (var schema in route.Schemas) {
            schema.Id = System.IO.Path.Combine(fi.Directory.FullName, schema.Id);
            schemas.Add(schema);
         }
         InitializeList();
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
            Approach = txtApproach.Text,
            History = txtHistory.Text,
            Difficulty = double.Parse((lstGrade.SelectedItem as ComboBoxItem).DataContext.ToString()),
            Tags = txtTags.Text.Split(',', StringSplitOptions.RemoveEmptyEntries)
         };

         if (ushort.TryParse(txtHeight.Text, out ushort h)) {
            result.Height = h;
         }

         result.Location = new Location();
         if (double.TryParse(txtLat1.Text, out double lat)) {
            result.Location.Latitude = lat;
         }
         if (double.TryParse(txtLon1.Text, out double lon)) {
            result.Location.Longitude = lon;
         }

         var name = result.Name.Replace(" ", "_").ToLowerInvariant();
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

         File.WriteAllText(System.IO.Path.Combine(saveDirectory.FullName, $"{name}.json"), JsonConvert.SerializeObject(result));
      }

      private void lstImages_SelectionChanged(object sender, SelectionChangedEventArgs e) {
         tabSchema.IsEnabled = lstImages.SelectedItem != null;
         if (tabSchema.IsEnabled) {
            // Load schema
            selectedSchemas = lstImages.SelectedItem as Schema;
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
            InitializeList();
         }
      }

      private void InitializeList() {
         lstImages.Items.Clear();
         foreach (var schema in schemas) {
            lstImages.Items.Add(schema);
         }
      }

      private void skCanvas_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e) {
         Size imageSize = new Size(0, 0);

         if (!string.IsNullOrEmpty(selectedSchemas?.Id)) {
            using (var bitmap = SkiaSharpHelper.LoadBitmap(selectedSchemas?.Id, skCanvas.ActualWidth, skCanvas.ActualHeight))
            using (var paint = new SKPaint {
               FilterQuality = SKFilterQuality.High, // high quality scaling
               IsAntialias = true
            }) {
               imageSize = new Size(bitmap.Width, bitmap.Height);
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
            if (shape is Domain.Entities.Path p && p.Points.Length > 0) {
               SkiaSharpHelper.DrawPath(
                  e.Surface.Canvas,
                  p.Points.Select(p => new Point(p.X * imageSize.Width, p.Y * imageSize.Height)),
                  System.Windows.Media.Colors.Red);
            } else if (shape is Ellipse el && el.Center != null && el.Radius != null) {
               SkiaSharpHelper.DrawEllipse(
                  e.Surface.Canvas,
                  new Point(el.Center.X * imageSize.Width, el.Center.Y * imageSize.Height),
                  new Point(el.Radius.X * imageSize.Width, el.Radius.Y * imageSize.Height),
                  System.Windows.Media.Colors.Red);
            }
         }
      }



      private void skCanvas_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
         Point position = e.GetPosition(skCanvas);
         position.X /= skCanvas.ActualWidth;
         position.Y /= skCanvas.ActualHeight;

         if (currentShape is Ellipse el) {
            el.Center = new SchemaPoint(position.X, position.Y);
         }
      }

      private void skCanvas_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e) {
         Point position = e.GetPosition(skCanvas);
         position.X /= skCanvas.ActualWidth;
         position.Y /= skCanvas.ActualHeight;

         if (currentShape is Ellipse el) {
            el.Radius = new SchemaPoint(position.X, position.Y);
            btnAddEllipse.IsChecked = false;
         } else if (currentShape is Domain.Entities.Path p) {
            currentPath.Add(new SchemaPoint(position.X, position.Y));
            p.Points = currentPath.ToArray();
            btnUndo.IsEnabled = true;
         }

         skCanvas.InvalidateVisual();
      }

      private void skCanvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e) {
         Point position = e.GetPosition(skCanvas);
         position.X /= skCanvas.ActualWidth;
         position.Y /= skCanvas.ActualHeight;

         if (currentShape is Ellipse el) {
            el.Radius = new SchemaPoint(position.X, position.Y);
         }

         skCanvas.InvalidateVisual();
      }

      private void btnAddPath_Checked(object sender, RoutedEventArgs e) {
         btnAddEllipse.IsChecked = false;
         currentShape = new Domain.Entities.Path();
      }

      private void btnAddPath_Unchecked(object sender, RoutedEventArgs e) {
         selectedSchemas.Shapes = selectedSchemas.Shapes.Append(currentShape).ToArray();
         currentShape = null;
         btnUndo.IsEnabled = false;
         skCanvas.InvalidateVisual();
      }

      private void btnAddEllipse_Checked(object sender, RoutedEventArgs e) {
         btnAddPath.IsChecked = false;
         currentShape = new Ellipse() { Center = null };
      }

      private void btnAddEllipse_Unchecked(object sender, RoutedEventArgs e) {
         selectedSchemas.Shapes = selectedSchemas.Shapes.Append(currentShape).ToArray();
         currentShape = null;
         skCanvas.InvalidateVisual();
      }

      private void btnUndo_Click(object sender, RoutedEventArgs e) {
         if (currentShape is Domain.Entities.Path p && currentPath.Count > 0) {
            currentPath.RemoveAt(currentPath.Count - 1);
            p.Points = currentPath.ToArray();
            skCanvas.InvalidateVisual();
         }
      }
   }
}
