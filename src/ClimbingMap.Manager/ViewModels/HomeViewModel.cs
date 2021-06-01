using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ClimbingMap.Manager.ViewModels {
   public class HomeViewModel : BindableBase {
      private readonly IRegionNavigationService regionNavigationService;

      public ICommand NewAreaCommand { get; }
      public ICommand NewRouteCommand { get; }
      public ICommand LoadAreaCommand { get; }
      public ICommand LoadRouteCommand { get; }

      public HomeViewModel(IRegionNavigationService regionNavigationService) {

         NewAreaCommand = new DelegateCommand(async () => await NewArea());
         NewRouteCommand = new DelegateCommand(async () => await NewRoute());
         LoadAreaCommand = new DelegateCommand(async () => await LoadArea());
         LoadRouteCommand = new DelegateCommand(async () => await LoadRoute());
         this.regionNavigationService = regionNavigationService;
      }
      private Task NewRoute() {
         regionNavigationService.RequestNavigate(
            new Uri("RouteView", UriKind.Relative));

         return Task.CompletedTask;
      }

      private Task LoadArea() {
         string areaFilePath = SelectFile("area.json");

         if (!string.IsNullOrEmpty(areaFilePath)) {
            NavigationParameters @params = new NavigationParameters();
            @params.Add("path", areaFilePath);

            regionNavigationService.RequestNavigate(
               new Uri("AreaView", UriKind.Relative),
               @params);
         }

         return Task.CompletedTask;
      }

      private string SelectFile(string fileFilter) {
         // Create OpenFileDialog
         Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();
         openFileDlg.CheckFileExists = true;
         openFileDlg.Filter = fileFilter;

         // Launch OpenFileDialog by calling ShowDialog method
         Nullable<bool> result = openFileDlg.ShowDialog();
         if (result == true) {
            return openFileDlg.FileName;
         } else {
            return string.Empty;
         }
      }

      private Task LoadRoute() {
         string routeFilePath = SelectFile("route.json");

         if (!string.IsNullOrEmpty(routeFilePath)) {
            NavigationParameters @params = new NavigationParameters();
            @params.Add("path", routeFilePath);

            regionNavigationService.RequestNavigate(
               new Uri("RouteView", UriKind.Relative),
               @params);
         }

         return Task.CompletedTask;
      }

      private Task NewArea() {
         regionNavigationService.RequestNavigate(
            new Uri("AreaView", UriKind.Relative));

         return Task.CompletedTask;
      }
   }
}
