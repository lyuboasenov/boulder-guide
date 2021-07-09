using BoulderGuide.DTOs;
using BoulderGuide.Wpf.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace BoulderGuide.Wpf.ViewModels {
   internal class MainWindowViewModel : BaseViewModel<MainWindow> {

      private string rootPath;

      public MainWindowViewModel() {
         AddAreaCommand = new Command(AddArea, CanAddArea);
         AddRouteCommand = new Command(AddRoute, CanAddRoute);
         CopyRouteCommand = new Command(CopyRoute, CanCopyRoute);
         ReloadCommand = new Command(Reload);
      }

      public IEnumerable<Area> Items { get; set; }
      public ICommand AddAreaCommand { get; }
      public ICommand AddRouteCommand { get; }
      public ICommand CopyRouteCommand { get; }
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
      }

      private void Reload() {
         Items = new[] { GetArea(rootPath) };
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
