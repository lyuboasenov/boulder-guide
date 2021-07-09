using BoulderGuide.Wpf.ViewModels;
using System.Windows;

namespace BoulderGuide.Wpf.Services {
   internal interface INavigationService {
      bool? Show<T>(BaseViewModel<T> vm, bool dialog = false) where T : Window, new();
   }
}