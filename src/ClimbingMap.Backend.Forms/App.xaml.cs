using ClimbingMap.Backend.Forms.ViewModels;
using ClimbingMap.Backend.Forms.Views;
using Prism;
using Prism.Ioc;
using System;
using Xamarin.Essentials.Implementation;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;

namespace ClimbingMap.Backend.Forms {
   public partial class App {
      public App(IPlatformInitializer initializer)
          : base(initializer) {
      }

      protected override async void OnInitialized() {
         InitializeComponent();

         TemplateUI.TemplateUI.Init();

         var result = await NavigationService.NavigateAsync("NavigationPage/MainPage");
      }

      protected override void RegisterTypes(IContainerRegistry containerRegistry) {
         containerRegistry.RegisterSingleton<IAppInfo, AppInfoImplementation>();

         containerRegistry.RegisterForNavigation<NavigationPage>();
         containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();
         containerRegistry.RegisterDialog<DialogView, DialogViewModel>();

         containerRegistry.Register<IPreferences, PreferencesImplementation>();
         containerRegistry.Register<IFilePicker, FilePickerImplementation>();
      }
   }
}
