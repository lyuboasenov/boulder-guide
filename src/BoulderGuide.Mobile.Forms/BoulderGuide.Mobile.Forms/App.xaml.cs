using BoulderGuide.Mobile.Forms.Services.Data;
using BoulderGuide.Mobile.Forms.Services.Errors;
using BoulderGuide.Mobile.Forms.Services.Location;
using BoulderGuide.Mobile.Forms.Services.UI;
using BoulderGuide.Mobile.Forms.ViewModels;
using BoulderGuide.Mobile.Forms.Views;
using Prism;
using Prism.Ioc;
using Xamarin.Essentials.Implementation;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;

namespace BoulderGuide.Mobile.Forms {
   public partial class App
   {
      public App(IPlatformInitializer initializer)
          : base(initializer)
      {
      }

      protected override IContainerExtension CreateContainerExtension() {
         var container = base.CreateContainerExtension();

         ((container as Prism.Unity.UnityContainerExtension)?.Instance as Unity.UnityContainer)?.AddExtension(new Unity.ForceActivation());

         return container;
      }

      protected override async void OnInitialized()
      {
         InitializeComponent();

         await NavigationService.NavigateAsync("/MainPage/NavigationPage/HomePage");
      }

      protected override void RegisterTypes(IContainerRegistry containerRegistry)
      {
         containerRegistry.RegisterSingleton<IAppInfo, AppInfoImplementation>();
         containerRegistry.RegisterSingleton<IFileSystem, FileSystemImplementation>();
         containerRegistry.RegisterSingleton<IConnectivity, ConnectivityImplementation>();
         containerRegistry.RegisterSingleton<IGeolocation, GeolocationImplementation>();
         containerRegistry.RegisterSingleton<IPermissions, PermissionsImplementation>();
         containerRegistry.RegisterSingleton<IPreferences, PreferencesImplementation>();
         containerRegistry.RegisterSingleton<IVersionTracking, VersionTrackingImplementation>();
         containerRegistry.RegisterSingleton<IMainThread, MainThreadImplementation>();
         containerRegistry.RegisterSingleton<IBrowser, BrowserImplementation>();
         containerRegistry.RegisterSingleton<IEmail, EmailImplementation>();
         containerRegistry.RegisterSingleton<ICompass, CompassImplementation>();

         containerRegistry.RegisterSingleton<IErrorService, ErrorService>();
         containerRegistry.RegisterSingleton<IDataService, DataService>();
         containerRegistry.RegisterSingleton<IDownloadService, DownloadService>();
         containerRegistry.RegisterSingleton<ILocationService, LocationService>();
         containerRegistry.RegisterSingleton<Services.Preferences.IPreferences, Services.Preferences.Preferences> ();
         containerRegistry.RegisterSingleton<IActivityIndicationService, ActivityIndicationService> ();

         containerRegistry.RegisterForNavigation<NavigationPage>();
         containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();
         containerRegistry.RegisterForNavigation<AreaPage, AreaPageViewModel>();

         containerRegistry.RegisterDialog<FilterDialogPage, FilterDialogPageViewModel>();
         containerRegistry.RegisterDialog<OrderDialogPage, OrderDialogPageViewModel>();
         containerRegistry.RegisterDialog<KeyDialogPage, KeyDialogPageViewModel>();
         containerRegistry.RegisterDialog<TopoDialogPage, TopoDialogPageViewModel>();
         containerRegistry.RegisterDialog<TextViewDialogPage, TextViewDialogPageViewModel>();
         containerRegistry.RegisterDialog<VideosDialogPage, VideosDialogPageViewModel>();

         containerRegistry.RegisterForNavigation<RoutePage, RoutePageViewModel>();
         containerRegistry.RegisterForNavigation<MapPage, MapPageViewModel>();
         containerRegistry.RegisterForNavigation<SettingsPage, SettingsPageViewModel>();
         containerRegistry.RegisterForNavigation<HomePage, HomePageViewModel>();

         containerRegistry.RegisterForNavigation<AboutPage, AboutPageViewModel>();
      }
   }
}
