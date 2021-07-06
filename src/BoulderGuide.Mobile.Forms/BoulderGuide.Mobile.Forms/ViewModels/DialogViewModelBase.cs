using BoulderGuide.Mobile.Forms.Services.Errors;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace BoulderGuide.Mobile.Forms.ViewModels {
   public abstract class DialogViewModelBase : BindableBase, IDialogAware {
      private IDialogService DialogService { get; }

      private IErrorService errorService;

      public ICommand CloseCommand { get; }

      public DialogViewModelBase() {
         var container = Prism.PrismApplicationBase.Current.Container.CurrentScope;
         DialogService = (IDialogService) container.Resolve(typeof(IDialogService));
         errorService = (IErrorService) container.Resolve(typeof(IErrorService));
         CloseCommand = new Command(Close);
      }

      protected virtual void Close() {
         RequestClose?.Invoke(null);
      }

      public virtual bool CanCloseDialog() => true;

      public virtual void OnDialogClosed() { }

      public virtual void OnDialogOpened(IDialogParameters parameters) {

      }

      public async Task ShowDialogAsync(string path, IDialogParameters parameters = null) {
         var result = await DialogService.ShowDialogAsync(
               path,
               parameters).
               ConfigureAwait(false);

         if (result.Exception != null) {
            await HandleOperationExceptionAsync(result.Exception, Strings.UnableToOpenDialog);
         }
      }

      public Task HandleExceptionAsync(Exception ex, string message) {
         return errorService.HandleErrorAsync(ex, message);
      }

      public Task HandleOperationExceptionAsync(Exception ex, string operation) {
         return HandleExceptionAsync(ex, string.Format(Strings.OperationExceptionFormat, operation));
      }

      public event Action<IDialogParameters> RequestClose;
   }
}
