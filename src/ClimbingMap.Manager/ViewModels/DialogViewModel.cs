using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace ClimbingMap.Manager.ViewModels {
   public class DialogViewModel : BindableBase, IDialogAware {


      public string Message { get; private set; }
      public Color ButtonColor { get; private set; }

      private DelegateCommand<string> _closeDialogCommand;
      public DelegateCommand<string> CloseDialogCommand =>
          _closeDialogCommand ?? (_closeDialogCommand = new DelegateCommand<string>(CloseDialog));


      public DialogViewModel() {

      }

      public bool CanCloseDialog() {
         return true;
      }

      public void OnDialogClosed() {

      }

      public void OnDialogOpened(IDialogParameters parameters) {
         if (parameters.TryGetValue(ParameterKeys.Message, out string message)) {
            Message = message;
         } else {
            Message = "Dialog shown without message.";
         }


         Severity severity = Severity.Error;
         parameters.TryGetValue(ParameterKeys.Severity, out severity);

         if (severity == Severity.Error) {
            ButtonColor = Colors.Red;
         } else if (severity == Severity.Message) {
            ButtonColor = Colors.Orange;
         } else if (severity == Severity.Warning) {
            ButtonColor = Colors.Green;
         }
      }

      public string Title { get; }

      public event Action<IDialogResult> RequestClose;

      protected virtual void CloseDialog(string parameter) {
         ButtonResult result = ButtonResult.None;

         if (parameter?.ToLower() == "true")
            result = ButtonResult.OK;
         else if (parameter?.ToLower() == "false")
            result = ButtonResult.Cancel;

         RaiseRequestClose(new DialogResult(result));
      }

      public virtual void RaiseRequestClose(IDialogResult dialogResult) {
         RequestClose?.Invoke(dialogResult);
      }

      public enum Severity {
         Message,
         Warning,
         Error
      }

      public static class ParameterKeys {
         public const string Message = "Message";
         public const string Severity = "Severity";
      }
   }
}
