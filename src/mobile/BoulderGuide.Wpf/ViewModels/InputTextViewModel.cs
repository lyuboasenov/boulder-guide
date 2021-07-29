using BoulderGuide.Wpf.Views;
using System;
using System.Windows.Input;

namespace BoulderGuide.Wpf.ViewModels {
   public class InputTextViewModel : BaseViewModel<InputTextView> {
      public string Input { get; set; }
      public ICommand OkCommand { get; }

      public InputTextViewModel() {
         OkCommand = new Command(Ok);
      }

      private void Ok() {
         Close();
      }
   }
}
