using System.Diagnostics;
using System.Windows;

namespace BoulderGuide.Wpf {
   /// <summary>
   /// Interaction logic for VideoWindow.xaml
   /// </summary>
   public partial class VideoWindow : Window {
      public string Id {
         get {
            return txtId.Text;
         }
         set {
            txtId.Text = value;
         }
      }

      public string Url {
         get {
            return txtUrl.Text;
         }
         set {
            txtUrl.Text = value;
         }
      }

      public string EmbedCode {
         get {
            return txtEmbed.Text;
         }
         set {
            txtEmbed.Text = value;
         }
      }

      public VideoWindow() {
         InitializeComponent();
      }

      private void btnOk_Click(object sender, RoutedEventArgs e) {
         DialogResult = true;
         Close();
      }

      private void btnCancel_Click(object sender, RoutedEventArgs e) {
         DialogResult = false;
         Close();
      }

      private void btnOpenInBrowser_Click(object sender, RoutedEventArgs e) {
         Process.Start(new ProcessStartInfo {
            FileName = Url,
            UseShellExecute = true
         });
      }
   }
}
