using System.Threading.Tasks;
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

         DoneCommand = new Command(async () => await Done());

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

      private Task Done() {

         preferences.FilterSearchTerm = SearchTerm;
         preferences.FilterMinDifficulty = MinDifficulty;
         preferences.FilterMaxDifficulty = MaxDifficulty;

         Close();

         return Task.CompletedTask;
      }
   }
}
