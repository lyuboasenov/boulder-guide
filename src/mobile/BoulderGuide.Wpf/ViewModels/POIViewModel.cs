using BoulderGuide.Wpf.Views;
using System;
using System.Windows.Input;

namespace BoulderGuide.Wpf.ViewModels {
   public class POIViewModel : BaseViewModel<POIView> {
      public string Name { get; set; }
      public string Type { get; set; }
      public string Location { get; set; }
      public ICommand OkCommand { get; }

      public POIViewModel() {
         OkCommand = new Command(Ok);
      }

      private void Ok() {
         Close();
      }
   }
}
