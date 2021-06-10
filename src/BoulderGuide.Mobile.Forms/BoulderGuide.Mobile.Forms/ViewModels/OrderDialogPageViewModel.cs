using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BoulderGuide.Mobile.Forms.ViewModels {
   public class OrderDialogPageViewModel : BindableBase, IDialogAware {

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

      public bool CanCloseDialog() => true;

      public void OnDialogClosed() { }

      public void OnDialogOpened(IDialogParameters parameters) {

      }

      public event Action<IDialogParameters> RequestClose;

      public void OnSelectedOrderOptionChanged() {
         int i = 0;
         foreach (var current in OrderOptions) {
            if (current == SelectedOrderOption) {
               preferences.RouteOrderByProperty = (Services.Preferences.RouteOrderBy) i;
               break;
            }
            i++;
         }

         RequestClose?.Invoke(null);
      }
   }
}
