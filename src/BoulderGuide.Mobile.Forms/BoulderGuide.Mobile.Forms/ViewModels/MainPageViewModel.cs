using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace BoulderGuide.Mobile.Forms.ViewModels {
   public class MainPageViewModel : ViewModelBase {
      public Breadcrumbs.Item SelectectedBreadcrumbsItem { get; set; }
      public ICommand SettingsCommand { get; }

      public MainPageViewModel() {
         SettingsCommand = new Command(async () => await Settings());
      }

      private async Task Settings() {
         await NavigateAsync(
            Strings.Settings,
            "/MainPage/NavigationPage/SettingsPage",
            glyph:Icons.MaterialIconFont.Settings);
      }

      public void OnSelectectedBreadcrumbsItemChanged() {
         for (int i = Breadcrumbs.Items.Count - 1; i >= 0; i--) {
            var current = Breadcrumbs.Items[i];
            Breadcrumbs.Items.RemoveAt(i);
            if (current == SelectectedBreadcrumbsItem) {
               NavigateAsync(current.Title, current.Path, current.Parameters, current.Glyph);
               break;
            }
         }
      }
   }
}
