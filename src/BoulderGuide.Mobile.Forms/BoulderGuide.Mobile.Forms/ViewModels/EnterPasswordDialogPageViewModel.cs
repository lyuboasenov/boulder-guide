using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BoulderGuide.Mobile.Forms.ViewModels {
   public class EnterPasswordDialogPageViewModel : BindableBase, IDialogAware {
      public ICommand CloseCommand { get; }
      public ICommand UnlockCommand { get; }
      public ICommand LockCommand { get; }
      public string Key { get; set; }

      private readonly Services.Preferences.IPreferences preferences;

      public EnterPasswordDialogPageViewModel(Services.Preferences.IPreferences preferences) {
         LockCommand = new DelegateCommand(async () => await Lock());
         UnlockCommand = new DelegateCommand(async () => await Unlock(), CanUnlock);

         this.preferences = preferences;
      }

      public bool CanCloseDialog() => true;

      public void OnDialogClosed() { }

      public void OnDialogOpened(IDialogParameters parameters) {
         Key = preferences.PrivateRegionsKey;
      }

      public event Action<IDialogParameters> RequestClose;


      private Task Unlock() {
         preferences.PrivateRegionsKey = Key;
         preferences.ShowPrivateRegions = true;

         RequestClose?.Invoke(null);

         return Task.CompletedTask;
      }

      private bool CanUnlock() {
         return !string.IsNullOrEmpty(Key);
      }

      private Task Lock() {
         preferences.PrivateRegionsKey = string.Empty;
         preferences.ShowPrivateRegions = false;

         RequestClose?.Invoke(null);

         return Task.CompletedTask;
      }

      public void OnKeyChanged() {
         (UnlockCommand as DelegateCommand)?.RaiseCanExecuteChanged();
      }
   }
}
