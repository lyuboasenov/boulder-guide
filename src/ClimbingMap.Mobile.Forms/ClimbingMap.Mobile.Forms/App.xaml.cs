using ClimbingMap.Mobile.Forms.ViewModels;
using ClimbingMap.Mobile.Forms.Views;
using Prism;
using Prism.Ioc;
using Xamarin.Essentials.Implementation;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;

namespace ClimbingMap.Mobile.Forms
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

         await NavigationService.NavigateAsync("NavigationPage/MainPage");
      }

      protected override void RegisterTypes(IContainerRegistry containerRegistry)
      {
         containerRegistry.RegisterSingleton<IAppInfo, AppInfoImplementation>();
         containerRegistry.RegisterSingleton<IFileSystem, FileSystemImplementation>();

         containerRegistry.RegisterForNavigation<NavigationPage>();
         containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();
         containerRegistry.RegisterForNavigation<AreaDetailsPage, AreaDetailsPageViewModel>();
      }
   }
}