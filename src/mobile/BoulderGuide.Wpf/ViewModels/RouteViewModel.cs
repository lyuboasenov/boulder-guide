using BoulderGuide.DTOs;
using BoulderGuide.Wpf.Domain;
using BoulderGuide.Wpf.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;

namespace BoulderGuide.Wpf.ViewModels {
   public class RouteViewModel : BaseViewModel<RouteView> {
      public Route Route { get; set; }
      public Video SelectedVideo { get; set; }

      public void OnSelectedVideoChanged() {
         (AddVideoCommand as Command)?.RaiseCanExecuteChanged();
         (EditVideoCommand as Command)?.RaiseCanExecuteChanged();
         (RemoveVideoCommand as Command)?.RaiseCanExecuteChanged();
         (GoToVideoCommand as Command)?.RaiseCanExecuteChanged();
      }

      public Topo SelectedTopo { get; set; }
      public void OnSelectedTopoChanged() {

         UpdateSurface?.Invoke(this, EventArgs.Empty);

         (RemoveImageCommand as Command)?.RaiseCanExecuteChanged();
         (AddPathToggledCommand as Command)?.RaiseCanExecuteChanged();
         (AddEllipseToggledCommand as Command)?.RaiseCanExecuteChanged();
         (AddRectangleToggledCommand as Command)?.RaiseCanExecuteChanged();
         (UndoCommand as Command)?.RaiseCanExecuteChanged();
      }

      public Shape SelectedShape { get; set; }
      public void OnSelectedShapeChanged() {
         (RemoveShapeCommand as Command)?.RaiseCanExecuteChanged();
      }

      public List<RelativePoint> CurrentPath { get; } = new List<RelativePoint>();
      public Shape CurrentShape { get; set; }
      public void OnCurrentShapeChanged() {
         (AddPathToggledCommand as Command)?.RaiseCanExecuteChanged();
         (AddEllipseToggledCommand as Command)?.RaiseCanExecuteChanged();
         (AddRectangleToggledCommand as Command)?.RaiseCanExecuteChanged();
         (UndoCommand as Command)?.RaiseCanExecuteChanged();
         (SaveCommand as Command)?.RaiseCanExecuteChanged();
      }
      public bool IsAddPathChecked { get; set; }
      public bool IsAddEllipseChecked { get; set; }
      public bool IsAddRectangleChecked { get; set; }

      public void OnIsAddRectangleCheckedChanged() {
         if (!IsAddRectangleChecked) {
            CommitCurrentShape();
         }
      }

      public void OnIsAddEllipseCheckedChanged() {
         if (!IsAddEllipseChecked) {
            CommitCurrentShape();
         }
      }

      public event EventHandler UpdateSurface;

      public Dictionary<double, string> Difficulties { get; } = new Dictionary<double, string>() {
         { -1, "Project" },
         { 145, "9A" },
         { 140, "8C+" },
         { 135, "8C" },
         { 130, "8B+" },
         { 125, "8B" },
         { 120, "8A+" },
         { 115, "8A" },
         { 110, "7C+" },
         { 105, "7C" },
         { 100, "7B+" },
         { 95, "7B" },
         { 90, "7A+" },
         { 85, "7A" },
         { 80, "6C+" },
         { 75, "6C" },
         { 70, "6B+" },
         { 65, "6B" },
         { 60, "6A+" },
         { 55, "6A" },
         { 50, "5+" },
         { 45, "5" },
         { 40, "5-" },
         { 35, "4+" },
         { 30, "4" },
         { 25, "4-" },
         { 20, "3" },
      };

      public ICommand SaveCommand { get; }
      public ICommand AddVideoCommand { get; }
      public ICommand EditVideoCommand { get; }
      public ICommand RemoveVideoCommand { get; }
      public ICommand GoToVideoCommand { get; }
      public ICommand AddImageCommand { get; }
      public ICommand RemoveImageCommand { get; }
      public ICommand RemoveShapeCommand { get; }
      public ICommand AddPathToggledCommand { get; }
      public ICommand AddEllipseToggledCommand { get; }
      public ICommand AddRectangleToggledCommand { get; }
      public ICommand UndoCommand { get; }

      public RouteViewModel(Route route) {
         SaveCommand = new Command(Save, CanSave);
         AddVideoCommand = new Command(AddVideo);
         EditVideoCommand = new Command(EditVideo, () => SelectedVideo != null);
         RemoveVideoCommand = new Command(RemoveVideo, () => SelectedVideo != null);
         GoToVideoCommand = new Command(GoToVideo, () => SelectedVideo != null);
         AddImageCommand = new Command(AddImage);
         RemoveImageCommand = new Command(RemoveImage, () => SelectedTopo != null);
         RemoveShapeCommand = new Command(RemoveShape, () => SelectedShape != null);

         AddPathToggledCommand = new Command(AddPath, () => SelectedTopo != null);
         AddEllipseToggledCommand = new Command(AddEllipse, () => SelectedTopo != null);
         AddRectangleToggledCommand = new Command(AddRectangle, () => SelectedTopo != null);
         UndoCommand = new Command(Undo, () => SelectedTopo != null && CurrentShape is Path && CurrentPath.Count > 0);
         Route = route;
         Route.PropertyChanged += Route_PropertyChanged;
      }

      private void Route_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
         (SaveCommand as Command)?.RaiseCanExecuteChanged();
      }

      private void Undo() {
         var last = CurrentPath.LastOrDefault();
         if (last != null) {
            CurrentPath.Remove(last);
            (CurrentShape as Path).Points = CurrentPath.ToArray();
            (UndoCommand as Command)?.RaiseCanExecuteChanged();
            UpdateSurface?.Invoke(this, EventArgs.Empty);
         }
      }

      private void AddEllipse() {
         if (IsAddEllipseChecked) {
            IsAddPathChecked = false;
            IsAddRectangleChecked = false;
            CurrentShape = new Ellipse() { Center = null };
         } else {
            CommitCurrentShape();
         }
      }

      private void AddRectangle() {
         if (IsAddRectangleChecked) {
            IsAddPathChecked = false;
            IsAddEllipseChecked = false;
            CurrentShape = new Rectangle() { Center = null };
         } else {
            CommitCurrentShape();
         }
      }

      private void AddPath() {
         if (IsAddPathChecked) {
            IsAddEllipseChecked = false;
            IsAddRectangleChecked = false;
            CurrentShape = new Path();
            CurrentPath.Clear();
         } else {
            CommitCurrentShape();
         }
      }

      private void CommitCurrentShape() {
         SelectedTopo.Shapes = SelectedTopo.Shapes.Append(CurrentShape).ToArray();
         CurrentShape = null;
         UpdateSurface?.Invoke(this, EventArgs.Empty);
         RaisePropertyChanged(nameof(SelectedTopo));
      }

      private void RemoveShape() {
         Route.RemoveShape(SelectedTopo, SelectedShape);
         RaisePropertyChanged(nameof(SelectedTopo));
         UpdateSurface?.Invoke(this, EventArgs.Empty);
         SelectedShape = SelectedTopo?.Shapes?.LastOrDefault();
      }

      private void RemoveImage() {
         Route.RemoveTopo(SelectedTopo);
         RaisePropertyChanged(nameof(Route));
      }

      private void AddImage() {
         var path = SelectFile("Image |*.jpg;*.jpeg;*.JPG;*.JPEG");

         if (!string.IsNullOrEmpty(path)) {
            Route.AddTopo(new Topo() {
               Id = path
            });
         }
         RaisePropertyChanged(nameof(Route));
      }

      private void GoToVideo() {
         Process.Start(new ProcessStartInfo {
            FileName = SelectedVideo.Url,
            UseShellExecute = true
         });
      }

      private void RemoveVideo() {
         Route.RemoveVideo(SelectedVideo);
      }

      private void EditVideo() {
         var vm = new VideoWindowViewModel() {
            Id = SelectedVideo.Id,
            Url = SelectedVideo.Url,
            EmbededCode = SelectedVideo.EmbedCode
         };

         (App.Current as App)?.NavigationService.Show(vm, true);

         if (vm.Result == true) {
            SelectedVideo.Id = vm.Id;
            SelectedVideo.Url = vm.Url;
            SelectedVideo.EmbedCode = vm.EmbededCode;
         }
      }

      private void AddVideo() {
         var vm = new VideoWindowViewModel();

         (App.Current as App)?.NavigationService.Show(vm, true);

         if (vm.Result == true) {
            var v = new Video() {
               Id = vm.Id,
               Url = vm.Url,
               EmbedCode = vm.EmbededCode
            };

            Route.AddVideo(v);
         }
      }

      private bool CanSave() {
         return !string.IsNullOrEmpty(Route.Id) && CurrentShape is null;
      }

      private void Save() {
         Route.Save();
      }
   }
}
