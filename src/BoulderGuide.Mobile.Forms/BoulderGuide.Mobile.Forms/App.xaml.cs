using BoulderGuide.Mobile.Forms.Services.Data;
using BoulderGuide.Mobile.Forms.Services.Maps;
using BoulderGuide.Mobile.Forms.ViewModels;
using BoulderGuide.Mobile.Forms.Views;
using Prism;
using Prism.Ioc;
using Xamarin.Essentials.Implementation;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;

namespace BoulderGuide.Mobile.Forms
{
   public partial class App
   {
      public App(IPlatformInitializer initializer)
          : base(initializer)
      {
      }

      protected override async void OnInitialized()
      {
         InitializeComponent();

         await NavigationService.NavigateAsync("MainPage");
      }

      protected override void RegisterTypes(IContainerRegistry containerRegistry)
      {
         containerRegistry.RegisterSingleton<IAppInfo, AppInfoImplementation>();
         containerRegistry.RegisterSingleton<IFileSystem, FileSystemImplementation>();
         containerRegistry.RegisterSingleton<IConnectivity, ConnectivityImplementation>();
         containerRegistry.RegisterSingleton<IGeolocation, GeolocationImplementation>();
         containerRegistry.RegisterSingleton<IPermissions, PermissionsImplementation>();
         containerRegistry.RegisterSingleton<IPreferences, PreferencesImplementation>();

         containerRegistry.RegisterSingleton<IDataService, DataService>();
         containerRegistry.RegisterSingleton<IMapService, MapService>();
         containerRegistry.RegisterSingleton<Services.Preferences.IPreferences, Services.Preferences.Preferences> ();

         containerRegistry.RegisterForNavigation<NavigationPage>();
         containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();
         containerRegistry.RegisterForNavigation<AreaDetailsPage, AreaDetailsPageViewModel>();

         containerRegistry.RegisterDialog<DialogPage, DialogPageViewModel>();
         containerRegistry.RegisterForNavigation<RoutePage, RoutePageViewModel>();
         containerRegistry.RegisterForNavigation<MapPage, MapPageViewModel>();
         containerRegistry.RegisterForNavigation<SettingsPage, SettingsPageViewModel>();
      }
   }
}