using System.Collections.Generic;
using System.Linq;

namespace BoulderGuide.Mobile.Forms.ViewModels {
   public class OrderDialogPageViewModel : DialogViewModelBase {

      private readonly Services.Preferences.IPreferences preferences;
      public IEnumerable<string> OrderOptions { get; set; }
      public string SelectedOrderOption { get; set; }

      public OrderDialogPageViewModel(Services.Preferences.IPreferences preferences) {
         this.preferences = preferences;

         OrderOptions = new[] {
            Strings.RouteOrderByName,
            Strings.RouteOrderByNameDesc,
            Strings.RouteOrderByDifficulty,
            Strings.RouteOrderByDifficultyDesc
         };
         SelectedOrderOption = OrderOptions.ElementAt((int) preferences.RouteOrderByProperty);

      }

      public void OnSelectedOrderOptionChanged() {
         int i = 0;
         foreach (var current in OrderOptions) {
            if (current == SelectedOrderOption) {
               preferences.RouteOrderByProperty = (Services.Preferences.RouteOrderBy) i;
               break;
            }
            i++;
         }

         Close();
      }
   }
}
