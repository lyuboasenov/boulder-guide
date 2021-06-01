using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace ClimbingMap.Backend.Forms.ViewModels {
   public class DialogViewModel : BindableBase, IDialogAware {
      public DelegateCommand CloseCommand { get; }
      public string Message { get; private set; }
      public Color ButtonColor { get; private set; }
      public DialogViewModel() {
         CloseCommand = new DelegateCommand(() => RequestClose(null));
      }
      public event Action<IDialogParameters> RequestClose;
      public bool CanCloseDialog() => true;
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
            ButtonColor = Color.Red;
         } else if (severity == Severity.Message) {
            ButtonColor = Color.Orange;
         } else if (severity == Severity.Warning) {
            ButtonColor = Color.Green;
         }
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
