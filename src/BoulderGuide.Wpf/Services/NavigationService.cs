using BoulderGuide.Wpf.ViewModels;
using System;
using System.Windows;

namespace BoulderGuide.Wpf.Services {
   internal class NavigationService : INavigationService {

      public bool? Show<T>(BaseViewModel<T> vm, bool dialog = false) where T : Window, new() {
         T view = vm.GetView();

         if (dialog) {
            return view.ShowDialog();
         } else {
            view.Show();
            return null;
         }
      }
   }
}
