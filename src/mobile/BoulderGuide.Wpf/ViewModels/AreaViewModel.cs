using BoulderGuide.DTOs;
using BoulderGuide.Wpf.Domain;
using BoulderGuide.Wpf.Views;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Xml;

namespace BoulderGuide.Wpf.ViewModels {
   public class AreaViewModel : BaseViewModel<AreaView> {
      public Area Area { get; set; }
      public Location SelectedLocation { get; set; }

      public void OnSelectedLocationChanged() {
         (RemovePointCommand as Command)?.RaiseCanExecuteChanged();
      }
      public PointOfInterest SelectedPOI { get; set; }
      public void OnSelectedPOIChanged() {
         (RemovePOICommand as Command)?.RaiseCanExecuteChanged();
      }
      public Track SelectedTrack { get; set; }
      public void OnSelectedTrackChanged() {
         (RemoveTrackCommand as Command)?.RaiseCanExecuteChanged();
      }

      public ICommand SaveCommand { get; }
      public ICommand AddPointCommand { get; }
      public ICommand RemovePointCommand { get; }
      public ICommand AddPOICommand { get; }
      public ICommand RemovePOICommand { get; }
      public ICommand AddTrackCommand { get; }
      public ICommand RemoveTrackCommand { get; }

      public AreaViewModel(Area area) {
         SaveCommand = new Command(Save, CanSave);
         AddPointCommand = new Command(AddPoint);
         RemovePointCommand = new Command(RemovePoint, CanRemovePoint);
         AddPOICommand = new Command(AddPOI);
         RemovePOICommand = new Command(RemovePOI, CanRemovePOI);
         AddTrackCommand = new Command(AddTrack);
         RemoveTrackCommand = new Command(RemoveTrack, CanRemoveTrack);
         Area = area;
      }

      private bool CanRemoveTrack() {
         return SelectedTrack != null;
      }

      private void RemoveTrack() {
         Area.RemoveTrack(SelectedTrack);
      }

      private void AddTrack() {

         string indexPath = SelectFile("Area gpx | *.gpx");
         if (!string.IsNullOrEmpty(indexPath)) {

            InputTextViewModel vm = new InputTextViewModel();
            (App.Current as App)?.NavigationService.Show(vm, true);

            if (!string.IsNullOrEmpty(vm.Input)) {
               XmlDocument doc = new XmlDocument();
               doc.Load(indexPath);
               XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
               nsmgr.AddNamespace("gpx", "http://www.topografix.com/GPX/1/1");

               var points = new List<Location>();

               foreach (XmlNode node in doc.DocumentElement.SelectNodes($"//gpx:trk[gpx:name='{vm.Input}']/*/gpx:trkpt", nsmgr)) {
                  points.Add(new Location() {
                     Latitude = double.Parse(node.Attributes["lat"].Value),
                     Longitude = double.Parse(node.Attributes["lon"].Value)
                  });
               }

               if (points.Count > 0) {
                  Area.AddTrack(new Track() {
                     Name = vm.Input,
                     Locations = points.ToArray()
                  });
               }
            }
         }
      }

      private void RemovePOI() {
         Area.RemovePOI(SelectedPOI);
      }

      private bool CanRemovePOI() {
         return SelectedPOI != null;
      }

      private void AddPOI() {
         POIViewModel vm = new POIViewModel();
         (App.Current as App)?.NavigationService.Show(vm, true);

         try {
            var poi = new PointOfInterest() {
               Name = vm.Name,
               Type = vm.Type,
               Location = new Location(vm.Location)
            };
            Area.AddPOI(poi);
         } catch (Exception ex) {
            HandleError(ex);
         }
      }

      private bool CanRemovePoint() {
         return SelectedLocation != null;
      }

      private void RemovePoint() {
         try {
            var l = SelectedLocation;
            SelectedLocation = null;
            Area.RemoveLocation(l);
         } catch (Exception ex) {
            HandleError(ex);
         }
      }

      private void AddPoint() {
         InputTextViewModel vm = new InputTextViewModel();
         (App.Current as App)?.NavigationService.Show(vm, true);

         try {
            var l = new Location(vm.Input);
            Area.AddLocation(l);
         } catch (Exception ex) {
            HandleError(ex);
         }
      }

      private bool CanSave() {
         return true;
      }

      private void Save() {
         Area.Save();
      }
   }
}
