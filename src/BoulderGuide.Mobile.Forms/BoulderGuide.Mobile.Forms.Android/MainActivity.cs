using Android.App;
using Android.Content.PM;
using Android.OS;
using Prism;
using Prism.Ioc;

namespace BoulderGuide.Mobile.Forms.Droid {
   [Activity(Theme = "@style/MainTheme",
             ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
   public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity {
      protected override void OnCreate(Bundle savedInstanceState) {
         TabLayoutResource = Resource.Layout.Tabbar;
         ToolbarResource = Resource.Layout.Toolbar;
         Rg.Plugins.Popup.Popup.Init(this);

         base.OnCreate(savedInstanceState);
         global::Xamarin.Forms.Forms.SetFlags("CarouselView_Experimental");
         global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

         LoadApplication(new App(new AndroidInitializer()));
      }

      public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults) {
         Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

         base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
      }

      public override void OnBackPressed() {
         Rg.Plugins.Popup.Popup.SendBackPressed(base.OnBackPressed);
      }
   }

   public class AndroidInitializer : IPlatformInitializer {
      public void RegisterTypes(IContainerRegistry containerRegistry) {
         // Register any platform specific implementations
      }
   }
}

