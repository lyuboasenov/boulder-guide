using BoulderGuide.Domain.Entities;
using BoulderGuide.Mobile.Forms.Services.Data;
using BoulderGuide.Mobile.Forms.Views;
using Prism.Navigation;
using Prism.Services.Dialogs;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;

namespace BoulderGuide.Mobile.Forms.ViewModels {
   public class AreaDetailsPageViewModel : ViewModelBase {

      private readonly IDataService dataService;
      private readonly IConnectivity connectivity;

      public ICommand DownloadCommand { get; }
      public ICommand MapCommand { get; }
      public bool IsLoading { get; set; }
      public Area Area { get; set; }
      public AreaInfo Info { get; set; }
      public AreaInfo SelectedAreaInfo { get; set; }
      public RouteInfo SelectedRouteInfo { get; set; }

      public AreaDetailsPageViewModel(
         INavigationService navigationService,
         IDataService dataService,
         IConnectivity connectivity,
         IDialogService dialogService) :
         base (navigationService, dialogService) {
         this.dataService = dataService;
         this.connectivity = connectivity;

         DownloadCommand = new Command(async () => await Download());
         MapCommand = new Command(async () => await Map(), CanShowMap);
      }

      private bool CanShowMap() {
         return Area != null && Info != null;
      }

      private async Task Map() {
         await NavigateAsync(
            $"{Strings.Map}: {Area.Name}" ,
            $"/MainPage/NavigationPage/{nameof(MapPage)}",
            MapPageViewModel.InitializeParameters(Area, Info));
      }

      public override void OnNavigatedTo(INavigationParameters parameters) {
         base.OnNavigatedTo(parameters);

         if (parameters.TryGetValue(nameof(AreaInfo), out AreaInfo info)) {
            Task.Run(async () => await InitializeAsync(info));
         } else {
            NavigationService.GoBackAsync();
         }
      }

      public void OnSelectedAreaInfoChanged() {
         NavigateAsync(
            SelectedAreaInfo.Name,
            $"/MainPage/NavigationPage/{nameof(AreaDetailsPage)}",
            InitializeParameters(SelectedAreaInfo));
      }

      public void OnSelectedRouteInfoChanged() {
         NavigateAsync(
            $"{SelectedRouteInfo.Name} ({new Grade(SelectedRouteInfo.Difficulty)})",
            $"/MainPage/NavigationPage/{nameof(RoutePage)}",
            RoutePageViewModel.InitializeParameters(SelectedRouteInfo, Info));
      }

      public static NavigationParameters InitializeParameters(AreaInfo info) {
         return InitializeParameters(nameof(AreaInfo), info);
      }

      private async Task InitializeAsync(AreaInfo info) {
         IsLoading = true;

         Info = info;

         try {
            Area = await dataService.GetArea(Info);

            Device.BeginInvokeOnMainThread(() => {
               (MapCommand as Command)?.ChangeCanExecute();
            });
         } catch (Exception ex) {
            await HandleExceptionAsync(ex);
         }
         IsLoading = false;
      }

      private async Task Download() {

         if (connectivity.NetworkAccess == NetworkAccess.Internet) {
            IsLoading = true;
            await dataService.DownloadArea(Info);
            IsLoading = false;
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
