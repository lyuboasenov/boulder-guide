using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace BoulderGuide.Mobile.Forms.ViewModels {
   public class ColorPickerDialogPageViewModel : DialogViewModelBase {
      private readonly Services.Preferences.IPreferences preferences;

      public ICommand ColorSelectedCommand { get; }

      public IEnumerable<Color> Colors { get; }


      public ColorPickerDialogPageViewModel(Services.Preferences.IPreferences preferences) {
         this.preferences = preferences;
         ColorSelectedCommand = new AsyncCommand<Color>(SelectColor);

         Colors = new[] {
            Color.FromHex("FFFFFF"),
            Color.FromHex("FCBF49"),
            Color.FromHex("F77F00"),
            Color.FromHex("D62828"),
            Color.FromHex("003049"),
         };
      }

      private Task SelectColor(Color selectedColor) {
         preferences.TopoColorHex = selectedColor.ToHex();
         base.Close();

         return Task.CompletedTask;
      }

      protected override void Close() {
         preferences.TopoColorHex = "FFFFFF";

         base.Close();
      }
   }
}
