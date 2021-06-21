using BoulderGuide.DTOs;
using BoulderGuide.Mobile.Forms.Services.Data.Entities;
using BoulderGuide.Mobile.Forms.Views;
using Prism.Navigation;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;

namespace BoulderGuide.Mobile.Forms.ViewModels {
   public class RoutePageViewModel : ViewModelBase {

      public RouteInfo Info { get; set; }
      public ICommand MapCommand { get; }
      public ICommand ViewTopoCommand { get; }

      public ICommand GoBackCommand { get; }

      public RoutePageViewModel() {
         MapCommand = new AsyncCommand(Map, CanShowMap);
         ViewTopoCommand = new AsyncCommand<Topo>(ViewTopo);
         GoBackCommand = new AsyncCommand(GoBackAsync);
      }

      public override bool CanNavigate(INavigationParameters parameters) {
         return base.CanNavigate(parameters) &&
            parameters.TryGetValue(nameof(RouteInfo), out RouteInfo _);
      }

      public override void OnNavigatedTo(INavigationParameters parameters) {
         base.OnNavigatedTo(parameters);

         if (parameters.TryGetValue(nameof(RouteInfo), out RouteInfo info)) {
            Info = info;
            RunAsync(InitializeAsync);
         }
      }

      private bool CanShowMap() {
         return Info?.Route != null;
      }

      private async Task Map() {
         await NavigateAsync(
            $"{Info.Name} ({new Grade(Info.Difficulty)})",
            $"/MainPage/NavigationPage/{nameof(MapPage)}",
            MapPageViewModel.InitializeParameters(Info),
            Icons.MaterialIconFont.Place);
      }

      private async Task ViewTopo(Topo topo) {
         if (null != topo) {
            await ShowDialogAsync(
               nameof(TopoDialogPage),
               TopoDialogPageViewModel.InitializeParameters(Info, topo)).
               ConfigureAwait(false);
         }
      }

      private async Task InitializeAsync() {
         try {
            await Info.LoadRouteAsync().ConfigureAwait(false);
            await RunOnMainThreadAsync(() => (MapCommand as AsyncCommand)?.ChangeCanExecute());
         } catch (Exception ex) {
            HandleException(ex);
         }
      }

      public static NavigationParameters InitializeParameters(RouteInfo routeInfo) {
         return InitializeParameters(nameof(RouteInfo), routeInfo);
      }
   }
}
