using System;
using System.Windows;

namespace BoulderGuide.Wpf
{
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : Window
   {
      public MainWindow()
      {
         InitializeComponent();
      }

      private void btnNewArea_Click(object sender, RoutedEventArgs e) {
         //AreaWindow areaWindow = new AreaWindow();
         //areaWindow.ShowDialog();
      }

      private void btnNewRoute_Click(object sender, RoutedEventArgs e) {
         //RouteWindow routeWindow = new RouteWindow();
         //routeWindow.ShowDialog();
      }

      private void btnLoadArea_Click(object sender, RoutedEventArgs e) {
         //string areaFilePath = SelectFile("Climbing map area file | area.json");

         //if (!string.IsNullOrEmpty(areaFilePath)) {
         //   AreaWindow areaWindow = new AreaWindow(areaFilePath);
         //   areaWindow.ShowDialog();
         //}
      }

      private void btnLoadRoute_Click(object sender, RoutedEventArgs e) {
         //string routeFilePath = SelectFile("Climbing map route file | *.json");

         //if (!string.IsNullOrEmpty(routeFilePath)) {
         //   RouteWindow routeWindow = new RouteWindow(routeFilePath);
         //   routeWindow.ShowDialog();
         //}
      }
   }
}
