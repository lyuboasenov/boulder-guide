using ClimbingMap.Manager.ViewModels;
using ClimbingMap.Manager.Views;
using Prism.Ioc;
using System.Windows;

namespace ClimbingMap.Manager {
   /// <summary>
   /// Interaction logic for App.xaml
   /// </summary>
   public partial class App {
      protected override Window CreateShell() {
         return Container.Resolve<MainWindow>();
      }

      protected override void RegisterTypes(IContainerRegistry containerRegistry) {
         containerRegistry.RegisterDialog<DialogView, DialogViewModel>();
      }
   }
}
