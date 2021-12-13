using BoulderGuide.DTOs;
using BoulderGuide.Wpf.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Xml;

namespace BoulderGuide.Wpf.ViewModels {
   internal class MainWindowViewModel : BaseViewModel<MainWindow> {

      private string rootPath;

      public MainWindowViewModel() {
         AddAreaCommand = new Command(AddArea, CanAddArea);
         AddRouteCommand = new Command(AddRoute, CanAddRoute);
         CopyRouteCommand = new Command(CopyRoute, CanCopyRoute);
         ImportRouteCommand = new Command(ImportRoute, CanImportRoute);
         ReloadCommand = new Command(Reload);
      }

      public IEnumerable<Area> Items { get; set; }
      public ICommand AddAreaCommand { get; }
      public ICommand AddRouteCommand { get; }
      public ICommand CopyRouteCommand { get; }
      public ICommand ImportRouteCommand { get; }
      public ICommand ReloadCommand { get; }

      public object SelectedItem { get; set; }
      public string Title { get; set; } = "Initial";

      public override void Initialize() {
         base.Initialize();

         string indexPath = SelectFile("Map index | index.json");
         if (!string.IsNullOrEmpty(indexPath)) {
            var fi = new FileInfo(indexPath);

            rootPath = fi.Directory.FullName;

            Reload();
            Title = $"Climbing region: {Items?.FirstOrDefault()?.Name}";
         } else {
            Close();
         }
      }

      public void OnSelectedItemChanged() {
         (AddAreaCommand as Command)?.RaiseCanExecuteChanged();
         (AddRouteCommand as Command)?.RaiseCanExecuteChanged();
         (CopyRouteCommand as Command)?.RaiseCanExecuteChanged();
         (ImportRouteCommand as Command)?.RaiseCanExecuteChanged();
      }

      private void Reload() {
         Items = new[] { GetArea(rootPath) };
      }

      private bool CanImportRoute() {
         return SelectedItem is Area;
      }

      private bool CanCopyRoute() {
         return SelectedItem is Route;
      }

      private bool CanAddRoute() {
         return SelectedItem is Area;
      }

      private bool CanAddArea() {
         return SelectedItem is Area;
      }

      private void AddRoute() {
         if (SelectedItem is Area a) {
            var route = new Route(a);
            route.Save();
         }

         Reload();
      }

      private void AddArea() {
         Reload();
      }

      private void CopyRoute() {
         if (SelectedItem is Route r) {
            r.Id = r.Id + "-copy";
            r.Save();

            Reload();
         }
      }

      private void ImportRoute() {
         string indexPath = SelectFile("Area gpx | *.gpx");
         if (SelectedItem is Area a) {
            XmlDocument doc = new XmlDocument();
            doc.Load(indexPath);
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("gpx", "http://www.topografix.com/GPX/1/1");

            var points = new List<Location>();

            foreach (XmlNode node in doc.DocumentElement.SelectNodes($"//gpx:wpt", nsmgr)) {
               points.Add(new Location() {
                  Latitude = double.Parse(node.Attributes["lat"].Value),
                  Longitude = double.Parse(node.Attributes["lon"].Value)
               });

               string name = null;
               string desc = null;

               foreach (XmlNode subNode in node.ChildNodes) {
                  if (subNode.Name.Equals("name", StringComparison.InvariantCultureIgnoreCase)) {
                     name = subNode.InnerText;
                  } else if (subNode.Name.Equals("desc", StringComparison.InvariantCultureIgnoreCase)) {
                     desc = subNode.InnerText;
                  }
               }

               var route = new Route(a) {
                  Name = name,
                  Info = desc,
                  Location = $"N{node.Attributes["lat"].Value} E{node.Attributes["lon"].Value}"
               };
               route.Save();
            }

            Reload();
         }
      }

      private Area GetArea(string path) {

         Area area = null;
         var areaPath = System.IO.Path.Combine(path, "area.json");
         if (File.Exists(areaPath)) {
            // load area
            area = new Area(areaPath);


            foreach (var filePath in Directory.GetFiles(path, "*.json")) {
               if (!filePath.EndsWith("index.json") &&
                  !filePath.EndsWith("area.json")) {
                  area.AddRoute(filePath);
               }
            }

            foreach(var subAreaPath in Directory.GetDirectories(path)) {
               if (!subAreaPath.EndsWith(".git") &&
                  !subAreaPath.EndsWith(".github")) {
                  area.AddArea(GetArea(subAreaPath));
               }
            }
         } else {
            MessageBox.Show($"No area file found in {path}");
         }

         return area;
      }
   }
}
