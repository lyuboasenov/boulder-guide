using BoulderGuide.DTOs;
using BoulderGuide.Mobile.Forms.Services.Data;
using BoulderGuide.Mobile.Forms.Services.UI;
using BoulderGuide.Mobile.Forms.Views;
using Prism.Navigation;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;

namespace BoulderGuide.Mobile.Forms.ViewModels {
   public class AreaDetailsPageViewModel : ViewModelBase {

      private readonly IDataService dataService;
      private readonly IConnectivity connectivity;
      private readonly IActivityIndicationService activityIndicationService;
      private readonly Services.Preferences.IPreferences preferences;

      public ICommand DownloadCommand { get; }
      public ICommand MapCommand { get; }
      public ICommand FilterCommand { get; }
      public ICommand OrderCommand { get; }
      public Area Area { get; set; }
      public AreaInfo Info { get; set; }
      public Info SelectedChild { get; set; }
      public ObservableCollection<Info> Children { get; set; } = new ObservableCollection<Info>();

      public AreaDetailsPageViewModel(
         IDataService dataService,
         IConnectivity connectivity,
         IActivityIndicationService activityIndicationService,
         Services.Preferences.IPreferences preferences) {
         this.dataService = dataService;
         this.connectivity = connectivity;
         this.activityIndicationService = activityIndicationService;
         this.preferences = preferences;

         DownloadCommand = new Command(async () => await Download());
         FilterCommand = new Command(async () => await Filter());
         OrderCommand = new Command(async () => await Order());
         MapCommand = new Command(async () => await Map(), CanShowMap);
      }

      private async Task Order() {
         await DialogService.ShowDialogAsync(nameof(OrderDialogPage));
         await InitializeAsync(Info);
      }

      private async Task Filter() {
         await DialogService.ShowDialogAsync(nameof(FilterDialogPage));
         await InitializeAsync(Info);
      }

      private bool CanShowMap() {
         return Area != null && Info != null;
      }

      private async Task Map() {
         await NavigateAsync(
            Area.Name,
            $"/MainPage/NavigationPage/{nameof(MapPage)}",
            MapPageViewModel.InitializeParameters(Area, Info),
            Icons.MaterialIconFont.Map);
      }

      public override void OnNavigatedTo(INavigationParameters parameters) {
         base.OnNavigatedTo(parameters);

         if (parameters.TryGetValue(nameof(AreaInfo), out AreaInfo info)) {
            Task.Run(async () => await InitializeAsync(info));
         } else {
            NavigationService.GoBackAsync();
         }
      }

      public void OnSelectedChildChanged() {
         if (SelectedChild is AreaInfo areaInfo) {
            NavigateAsync(
               areaInfo.Name,
               $"/MainPage/NavigationPage/{nameof(AreaDetailsPage)}",
               InitializeParameters(areaInfo),
               Icons.MaterialIconFont.Terrain);
         } else if (SelectedChild is RouteInfo routeInfo) {
            NavigateAsync(
               $"{routeInfo.Name} ({new Grade(routeInfo.Difficulty)})",
               $"/MainPage/NavigationPage/{nameof(RoutePage)}",
               RoutePageViewModel.InitializeParameters(routeInfo, Info),
               Icons.MaterialIconFont.Moving);
         }
      }

      public static NavigationParameters InitializeParameters(AreaInfo info) {
         return InitializeParameters(nameof(AreaInfo), info);
      }

      private async Task InitializeAsync(AreaInfo info) {
         await activityIndicationService.StartLoadingAsync();

         Info = info;

         try {
            Area = await dataService.GetArea(Info);

            var searchTerm = preferences.FilterSearchTerm.ToLowerInvariant();
            var minDifficulty = preferences.FilterMinDifficulty;
            var maxDifficulty = preferences.FilterMaxDifficulty;

            Children.Clear();
            foreach (var area in info.Areas?.
                  Where(a =>
                     string.IsNullOrEmpty(searchTerm) ||
                     a.Name.ToLowerInvariant().Contains(searchTerm))
                   ?? Enumerable.Empty<AreaInfo>()) {
               Children.Add(area);
            }
            foreach (var route in info.Routes?.Where(r =>
                  (string.IsNullOrEmpty(searchTerm) || r.Name.ToLowerInvariant().Contains(searchTerm)) &&
                  minDifficulty <= r.Difficulty && r.Difficulty <= maxDifficulty) ?? Enumerable.Empty<RouteInfo>()) {
               Children.Add(route);
            }

            Device.BeginInvokeOnMainThread(() => {
               (MapCommand as Command)?.ChangeCanExecute();
            });
         } catch (Exception ex) {
            await HandleExceptionAsync(ex);
         }
         await activityIndicationService.FinishLoadingAsync();
      }

      private async Task Download() {

         if (connectivity.NetworkAccess == NetworkAccess.Internet) {
            await activityIndicationService.StartLoadingAsync();
            await dataService.DownloadArea(Info);
            await activityIndicationService.FinishLoadingAsync();
         } else {
            var dialogParams = new DialogParameters();
            dialogParams.Add(
               DialogPageViewModel.ParameterKeys.Message,
               Strings.UnableToDownloadEntireArea);
            dialogParams.Add(DialogPageViewModel.ParameterKeys.Severity, DialogPageViewModel.Severity.Error);

            await DialogService.ShowDialogAsync(nameof(DialogPage), dialogParams);
         }
      }
   }
}
