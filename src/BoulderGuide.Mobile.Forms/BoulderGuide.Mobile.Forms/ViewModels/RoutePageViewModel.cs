using BoulderGuide.Domain.Entities;
using BoulderGuide.Mobile.Forms.Services.Data;
using BoulderGuide.Mobile.Forms.Views;
using Prism.Navigation;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;

namespace BoulderGuide.Mobile.Forms.ViewModels {
   public class RoutePageViewModel : ViewModelBase {

      private readonly IDataService dataService;
      private readonly IConnectivity connectivity;

      public Route Route { get; set; }
      public RouteInfo Info { get; set; }
      public AreaInfo AreaInfo { get; set; }
      public ICommand MapCommand { get; }

      public RoutePageViewModel(
         INavigationService navigationService,
         IDataService dataService,
         IConnectivity connectivity,
         IDialogService dialogService) :
         base(navigationService, dialogService) {
         this.dataService = dataService;
         this.connectivity = connectivity;
         MapCommand = new Command(async () => await Map(), CanShowMap);
      }

      public override void OnNavigatedTo(INavigationParameters parameters) {
         base.OnNavigatedTo(parameters);

         if (parameters.TryGetValue(nameof(RouteInfo), out RouteInfo info) &&
            parameters.TryGetValue(nameof(AreaInfo), out AreaInfo areaInfo)) {
            Task.Run(async () => await InitializeAsync(info, areaInfo));
         } else {
            Task.Run(async () => await NavigationService.GoBackAsync());
         }
      }

      private bool CanShowMap() {
         return Route != null && Info != null;
      }

      private async Task Map() {
         await NavigateAsync(
            $"{Route.Name} ({Route.Grade})",
            $"/MainPage/NavigationPage/{nameof(MapPage)}",
            MapPageViewModel.InitializeParameters(Route, AreaInfo),
            Icons.MaterialIconFont.Place);
      }

      private async Task InitializeAsync(RouteInfo info, AreaInfo areaInfo) {
         Info = info;
         AreaInfo = areaInfo;

         try {
            Route = await dataService.GetRoute(Info);

            Device.BeginInvokeOnMainThread(() => {
               (MapCommand as Command)?.ChangeCanExecute();
            });
         } catch (Exception ex) {
            await HandleExceptionAsync(ex);
         }
      }

      public static NavigationParameters InitializeParameters(RouteInfo routeInfo, AreaInfo areaInfo) {
         return InitializeParameters(
            new KeyValuePair<string, object>(nameof(RouteInfo), routeInfo),
            new KeyValuePair<string, object>(nameof(AreaInfo), areaInfo));
      }
   }
}
