using Rg.Plugins.Popup.Pages;
using Xamarin.Forms.Xaml;

namespace BoulderGuide.Mobile.Forms.Views {
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class LoadingPopupPage : PopupPage {
      public LoadingPopupPage() {
         InitializeComponent();
      }
      protected override void OnDisappearing() {

      }
      protected override bool OnBackButtonPressed() {
         return true;
      }
      protected override bool OnBackgroundClicked() {
         return false;
      }
   }
}