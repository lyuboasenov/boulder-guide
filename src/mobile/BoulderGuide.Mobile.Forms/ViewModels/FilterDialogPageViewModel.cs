using System.Windows.Input;
using Xamarin.Forms;

namespace BoulderGuide.Mobile.Forms.ViewModels {
   public class FilterDialogPageViewModel : DialogViewModelBase {
      private readonly Services.Preferences.IPreferences preferences;

      public ICommand DoneCommand { get; }
      public string SearchTerm { get; set; }
      public int MinDifficulty { get; set; }
      public int MaxDifficulty { get; set; }


      public FilterDialogPageViewModel(Services.Preferences.IPreferences preferences) {
         this.preferences = preferences;

         DoneCommand = new Command(Done);

         SearchTerm = preferences.FilterSearchTerm;
         MinDifficulty = preferences.FilterMinDifficulty;
         MaxDifficulty = preferences.FilterMaxDifficulty;
      }

      protected override void Close() {
         preferences.FilterSearchTerm = string.Empty;
         preferences.FilterMinDifficulty = 0;
         preferences.FilterMaxDifficulty = 150;

         base.Close();
      }

      private void Done() {
         preferences.FilterSearchTerm = SearchTerm;
         preferences.FilterMinDifficulty = MinDifficulty;
         preferences.FilterMaxDifficulty = MaxDifficulty;

         base.Close();
      }
   }
}
