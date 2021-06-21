using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace BoulderGuide.Mobile.Forms.ViewModels {
   public abstract class DialogViewModelBase : BindableBase, IDialogAware {

      public ICommand CloseCommand { get; }

      public DialogViewModelBase() {
         CloseCommand = new Command(Close);
      }

      protected virtual void Close() {
         RequestClose?.Invoke(null);
      }

      public virtual bool CanCloseDialog() => true;

      public virtual void OnDialogClosed() { }

      public virtual void OnDialogOpened(IDialogParameters parameters) {

      }

      public event Action<IDialogParameters> RequestClose;
   }
}
