using BoulderGuide.DTOs;
using BoulderGuide.Wpf.Domain;
using BoulderGuide.Wpf.Views;
using System;
using System.Windows.Input;

namespace BoulderGuide.Wpf.ViewModels {
   public class AreaViewModel : BaseViewModel<AreaView> {
      public Area Area { get; set; }
      public Location SelectedLocation { get; set; }

      public void OnSelectedLocationChanged() {
         (RemovePointCommand as Command)?.RaiseCanExecuteChanged();
      }

      public ICommand SaveCommand { get; }
      public ICommand AddPointCommand { get; }
      public ICommand RemovePointCommand { get; }

      public AreaViewModel(Area area) {
         SaveCommand = new Command(Save, CanSave);
         AddPointCommand = new Command(AddPoint);
         RemovePointCommand = new Command(RemovePoint, CanRemovePoint);
         Area = area;
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
