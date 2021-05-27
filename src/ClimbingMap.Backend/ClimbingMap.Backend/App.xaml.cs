using ClimbingMap.Backend.ViewModels;
using ClimbingMap.Backend.Views;
using Prism;
using Prism.Ioc;
using Xamarin.Essentials.Implementation;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;

namespace ClimbingMap.Backend {
   public partial class App {
      public App(IPlatformInitializer initializer)
          : base(initializer) {
      }

      protected override async void OnInitialized() {
         InitializeComponent();

         TemplateUI.TemplateUI.Init();

         await NavigationService.NavigateAsync("NavigationPage/MainPage");
      }

      protected override void RegisterTypes(IContainerRegistry containerRegistry) {
         containerRegistry.RegisterSingleton<IAppInfo, AppInfoImplementation>();

         containerRegistry.RegisterForNavigation<NavigationPage>();
         containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();
         containerRegistry.RegisterDialog<DialogView, DialogViewModel>();

         containerRegistry.Register<IPreferences, PreferencesImplementation>();
      }
   }
}
