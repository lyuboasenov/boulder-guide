using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace BoulderGuide.Mobile.Forms.ViewModels {
   public class TextViewDialogPageViewModel : BindableBase, IDialogAware {

      public string Title { get; set; }
      public string Text { get; set; }

      public ICommand CloseCommand { get; }
      public TextViewDialogPageViewModel() {
         CloseCommand = new Command(() => RequestClose?.Invoke(null));
      }

      public bool CanCloseDialog() => true;

      public void OnDialogClosed() { }


      public void OnDialogOpened(IDialogParameters parameters) {
         if (parameters.TryGetValue(nameof(Title), out string title) &&
            parameters.TryGetValue(nameof(Text), out string text)) {
            Title = title;
            Text = text;
         } else {
            RequestClose?.Invoke(null);
         }
      }

      public event Action<IDialogParameters> RequestClose;

   }
}
