using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClimbingMap.Wpf
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
         AreaWindow areaWindow = new AreaWindow();
         areaWindow.ShowDialog();
      }

      private void btnNewRoute_Click(object sender, RoutedEventArgs e) {
         RouteWindow routeWindow = new RouteWindow();
         routeWindow.ShowDialog();
      }

      private void btnLoadArea_Click(object sender, RoutedEventArgs e) {
         string areaFilePath = SelectFile("Climbing map area file | area.json");

         if (!string.IsNullOrEmpty(areaFilePath)) {
            AreaWindow areaWindow = new AreaWindow(areaFilePath);
            areaWindow.ShowDialog();
         }
      }

      private void btnLoadRoute_Click(object sender, RoutedEventArgs e) {
         string routeFilePath = SelectFile("Climbing map route file | *.json");

         if (!string.IsNullOrEmpty(routeFilePath)) {
            RouteWindow routeWindow = new RouteWindow(routeFilePath);
            routeWindow.ShowDialog();
         }
      }

      private string SelectFile(string fileFilter) {
         // Create OpenFileDialog
         Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();
         openFileDlg.CheckFileExists = true;
         openFileDlg.Filter = fileFilter;

         // Launch OpenFileDialog by calling ShowDialog method
         Nullable<bool> result = openFileDlg.ShowDialog();
         if (result == true) {
            return openFileDlg.FileName;
         } else {
            return string.Empty;
         }
      }
   }
}
