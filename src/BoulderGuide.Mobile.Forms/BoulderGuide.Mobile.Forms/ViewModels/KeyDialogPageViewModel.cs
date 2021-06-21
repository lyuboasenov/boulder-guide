using Prism.Commands;
using Prism.Services.Dialogs;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BoulderGuide.Mobile.Forms.ViewModels {
   public class KeyDialogPageViewModel : DialogViewModelBase {
      public ICommand UnlockCommand { get; }
      public ICommand LockCommand { get; }
      public string Key { get; set; }

      private readonly Services.Preferences.IPreferences preferences;

      public KeyDialogPageViewModel(Services.Preferences.IPreferences preferences) {
         LockCommand = new DelegateCommand(async () => await Lock());
         UnlockCommand = new DelegateCommand(async () => await Unlock(), CanUnlock);

         this.preferences = preferences;
      }

      public override void OnDialogOpened(IDialogParameters parameters) {
         base.OnDialogOpened(parameters);

         Key = preferences.PrivateRegionsKey;
      }

      private Task Unlock() {
         preferences.PrivateRegionsKey = Key;
         preferences.ShowPrivateRegions = true;

         Close();

         return Task.CompletedTask;
      }

      private bool CanUnlock() {
         return !string.IsNullOrEmpty(Key);
      }

      private Task Lock() {
         preferences.PrivateRegionsKey = string.Empty;
         preferences.ShowPrivateRegions = false;

         Close();

         return Task.CompletedTask;
      }

      public void OnKeyChanged() {
         (UnlockCommand as DelegateCommand)?.RaiseCanExecuteChanged();
      }
   }
}
