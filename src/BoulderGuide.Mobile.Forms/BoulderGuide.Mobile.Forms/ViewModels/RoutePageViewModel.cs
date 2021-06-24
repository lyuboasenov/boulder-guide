using BoulderGuide.DTOs;
using BoulderGuide.Mobile.Forms.Domain;
using BoulderGuide.Mobile.Forms.Views;
using Prism.Navigation;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;

namespace BoulderGuide.Mobile.Forms.ViewModels {
   public class RoutePageViewModel : ViewModelBase {

      public RouteInfo Info { get; set; }
      public int DisplayedTopoIndex { get; set; }
      public ICommand MapCommand { get; }
      public ICommand ViewTopoCommand { get; }
      public ICommand VideosCommand { get; }

      public RoutePageViewModel() {
         MapCommand = new AsyncCommand(Map, CanShowMap);
         ViewTopoCommand = new AsyncCommand(ViewTopo, CanViewTopo);
         VideosCommand = new AsyncCommand(Videos, CanVideos);
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

      public void OnDisplayedTopoIndex() {
         RunOnMainThreadAsync(() => (ViewTopoCommand as AsyncCommand)?.RaiseCanExecuteChanged());
      }

      private bool CanShowMap() {
         return Info?.Route != null;
      }

      private bool CanVideos() {
         return Info?.Route?.Videos?.Any() ?? false;
      }

      private async Task Videos() {
         await ShowDialogAsync(
            nameof(VideosDialogPage),
            VideosDialogPageViewModel.InitializeParameters(Info)).
            ConfigureAwait(false);
      }

      private async Task Map() {
         await NavigateAsync(
            $"{Info.Name} ({new Grade(Info.Difficulty)})",
            $"/MainPage/NavigationPage/{nameof(MapPage)}",
            MapPageViewModel.InitializeParameters(Info),
            Icons.MaterialIconFont.Place);
      }

      private async Task ViewTopo() {
         await ShowDialogAsync(
            nameof(TopoDialogPage),
            TopoDialogPageViewModel.InitializeParameters(Info, Info.Route.Topos.ElementAt(DisplayedTopoIndex))).
            ConfigureAwait(false);
      }

      private bool CanViewTopo() {
         return DisplayedTopoIndex < (Info?.Route?.Topos?.Count() ?? 0);
      }

      private async Task InitializeAsync() {
         try {
            await Info.LoadRouteAsync().ConfigureAwait(false);
            await RunOnMainThreadAsync(() => {
               (MapCommand as AsyncCommand)?.RaiseCanExecuteChanged();
               (ViewTopoCommand as AsyncCommand)?.RaiseCanExecuteChanged();
               (VideosCommand as AsyncCommand)?.RaiseCanExecuteChanged();
            });
         } catch (Exception ex) {
            await HandleExceptionAsync(ex);
         }
      }

      public static NavigationParameters InitializeParameters(RouteInfo routeInfo) {
         return InitializeParameters(nameof(RouteInfo), routeInfo);
      }
   }
}
