using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;

namespace BoulderGuide.Mobile.Forms.ViewModels {
   public class MainPageViewModel : ViewModelBase {
      public Breadcrumbs.Item SelectectedBreadcrumbsItem { get; set; }
      public void OnSelectectedBreadcrumbsItemChanged() {
         RunAsync(async () => await NavigateAsync(SelectectedBreadcrumbsItem));
      }

      public ICommand SettingsCommand { get; }
      public ICommand InfoCommand { get; }

      public MainPageViewModel() {
         SettingsCommand = new AsyncCommand(Settings);
         InfoCommand = new AsyncCommand(Info);
      }

      private Task Info() {
         return NavigateAsync(
            Strings.About,
            "/MainPage/NavigationPage/AboutPage",
            glyph: Icons.MaterialIconFont.Info);
      }

      private Task Settings() {
         return NavigateAsync(
            Strings.Settings,
            "/MainPage/NavigationPage/SettingsPage",
            glyph:Icons.MaterialIconFont.Settings);
      }
   }
}
