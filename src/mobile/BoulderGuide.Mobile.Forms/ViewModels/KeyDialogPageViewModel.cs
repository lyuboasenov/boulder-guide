using Prism.Services.Dialogs;
using System.Windows.Input;
using Xamarin.Forms;

namespace BoulderGuide.Mobile.Forms.ViewModels {
   public class KeyDialogPageViewModel : DialogViewModelBase {
      public ICommand UnlockCommand { get; }
      public ICommand LockCommand { get; }
      public string Key { get; set; }

      private readonly Services.Preferences.IPreferences preferences;

      public KeyDialogPageViewModel(Services.Preferences.IPreferences preferences) {
         LockCommand = new Command(Lock);
         UnlockCommand = new Command(Unlock, CanUnlock);

         this.preferences = preferences;
      }

      public override void OnDialogOpened(IDialogParameters parameters) {
         base.OnDialogOpened(parameters);

         Key = preferences.PrivateRegionsKey;
      }

      private void Unlock() {
         preferences.PrivateRegionsKey = Key;
         preferences.ShowPrivateRegions = true;

         Close();
      }

      private bool CanUnlock() {
         return !string.IsNullOrEmpty(Key);
      }

      private void Lock() {
         preferences.PrivateRegionsKey = string.Empty;
         preferences.ShowPrivateRegions = false;

         Close();
      }

      public void OnKeyChanged() {
         (UnlockCommand as Command)?.ChangeCanExecute();
      }
   }
}
