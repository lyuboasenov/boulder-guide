using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;

namespace BoulderGuide.Mobile.Forms.ViewModels {
   public class MainPageViewModel : ViewModelBase {
      public Breadcrumbs.Item SelectectedBreadcrumbsItem { get; set; }
      public void OnSelectectedBreadcrumbsItemChanged() {
         try {
            for (int i = Breadcrumbs.Items.Count - 1; i >= 0; i--) {
               var current = Breadcrumbs.Items[i];
               Breadcrumbs.Items.RemoveAt(i);
               if (current == SelectectedBreadcrumbsItem) {
                  NavigateAsync(current.Title, current.Path, current.Parameters, current.Glyph);
                  break;
               }
            }
         } catch (Exception ex) {
            HandleOperationException(ex, string.Format(Strings.UnableToNavigateFormat, SelectectedBreadcrumbsItem?.Title));
         }
      }
      public ICommand SettingsCommand { get; }

      public MainPageViewModel() {
         SettingsCommand = new AsyncCommand(Settings);
      }

      private async Task Settings() {
         await NavigateAsync(
            Strings.Settings,
            "/MainPage/NavigationPage/SettingsPage",
            glyph:Icons.MaterialIconFont.Settings);
      }
   }
}
