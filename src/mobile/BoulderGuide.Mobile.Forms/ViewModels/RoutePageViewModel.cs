using BoulderGuide.DTOs;
using BoulderGuide.Mobile.Forms.Domain;
using BoulderGuide.Mobile.Forms.Services.Preferences;
using BoulderGuide.Mobile.Forms.Views;
using Prism.Navigation;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;

namespace BoulderGuide.Mobile.Forms.ViewModels {
   public class RoutePageViewModel : ViewModelBase {

      private readonly Services.Preferences.IPreferences preferences;
      private readonly IBrowser browser;

      public RouteInfo Info { get; set; }
      public int DisplayedTopoIndex { get; set; }
      public ICommand MapCommand { get; }
      public ICommand ViewTopoCommand { get; }
      public ICommand VideosCommand { get; }
      public ICommand ChangeColorCommand { get; }
      public ICommand EightACommand { get; }
      public Color TopoColor { get; set; }

      public RoutePageViewModel(Services.Preferences.IPreferences preferences, IBrowser browser) {
         this.preferences = preferences;
         this.browser = browser;

         MapCommand = new AsyncCommand(Map, CanShowMap);
         ViewTopoCommand = new AsyncCommand(ViewTopo, CanViewTopo);
         VideosCommand = new AsyncCommand(Videos, CanVideos);
         ChangeColorCommand = new AsyncCommand(ChangeColor);
         EightACommand = new AsyncCommand(EightA, () => !string.IsNullOrEmpty(Info?.Route?.EightALink));
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

      private Task EightA() {
         try {
            return browser.OpenAsync(Info.Route.EightALink);
         } catch (Exception ex) {
            return HandleOperationExceptionAsync(ex, string.Format(Strings.UnableToOpenBrowserFormat, Info.Route.EightALink));
         }

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

         TopoColor = Color.FromHex(preferences.TopoColorHex);
      }

      private bool CanViewTopo() {
         return DisplayedTopoIndex < (Info?.Route?.Topos?.Count() ?? 0);
      }

      private async Task ChangeColor() {
         await ShowDialogAsync(
            nameof(ColorPickerDialogPage));

         TopoColor = Color.FromHex(preferences.TopoColorHex);
      }

      private async Task InitializeAsync() {
         try {
            TopoColor = Color.FromHex(preferences.TopoColorHex);
            await Info.LoadRouteAsync().ConfigureAwait(false);
            await RunOnMainThreadAsync(() => {
               (MapCommand as AsyncCommand)?.RaiseCanExecuteChanged();
               (ViewTopoCommand as AsyncCommand)?.RaiseCanExecuteChanged();
               (VideosCommand as AsyncCommand)?.RaiseCanExecuteChanged();
               (EightACommand as AsyncCommand)?.RaiseCanExecuteChanged();
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
