using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace ClimbingMap.Mobile.Forms.ViewModels {
   public class DialogPageViewModel : BindableBase, IDialogAware {

      public ICommand CloseCommand { get; }
      public string Message { get; set; }
      public Color SeverityColor { get; set; }

      public DialogPageViewModel() {
         CloseCommand = new DelegateCommand(() => RequestClose(null));
      }

      public bool CanCloseDialog() => true;

      public void OnDialogClosed() { }

      public void OnDialogOpened(IDialogParameters parameters) {
         if (parameters.TryGetValue<string>(ParameterKeys.Message, out string message)) {
            Message = message;
         } else {
            Message = "Dialog without message!";
         }

         Severity severity = Severity.Info;
         parameters.TryGetValue<Severity>(ParameterKeys.Severity, out severity);

         if (severity == Severity.Error) {
            SeverityColor = Color.Red;
         } else if (severity == Severity.Warning) {
            SeverityColor = Color.OrangeRed;
         } else if (severity == Severity.Info) {
            SeverityColor = Color.Green;
         }
      }

      public event Action<IDialogParameters> RequestClose;

      public static class ParameterKeys {
         public const string Message = "Message";
         public const string Severity = "Severity";
      }

      public enum Severity {
         Info,
         Warning,
         Error
      }
   }
}
