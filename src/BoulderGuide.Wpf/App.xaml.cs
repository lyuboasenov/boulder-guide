using BoulderGuide.Wpf.Services;
using BoulderGuide.Wpf.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using NavigationService = BoulderGuide.Wpf.Services.NavigationService;

namespace BoulderGuide.Wpf
{
   /// <summary>
   /// Interaction logic for App.xaml
   /// </summary>
   public partial class App : Application
   {
      internal INavigationService NavigationService { get; private set; }

      protected override void OnStartup(StartupEventArgs e) {
         base.OnStartup(e);
         NavigationService = new NavigationService();
         NavigationService.Show(new MainWindowViewModel(), true);
      }
      protected override void OnLoadCompleted(NavigationEventArgs e) {
         base.OnLoadCompleted(e);
      }
   }
}
