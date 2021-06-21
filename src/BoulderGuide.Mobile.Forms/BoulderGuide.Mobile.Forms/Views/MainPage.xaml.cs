
using BoulderGuide.Mobile.Forms.ViewModels;
using Xamarin.Forms;

namespace BoulderGuide.Mobile.Forms.Views {
   public partial class MainPage : MasterDetailPage {
      public MainPage() {
         InitializeComponent();
      }

      protected override bool OnBackButtonPressed() {
         if (BindingContext is MainPageViewModel vm) {
            vm.GoBackAsync();
         }
         return true;
      }
   }
}
