using ClimbingMap.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace ClimbingMap.Wpf {
   /// <summary>
   /// Interaction logic for AreaWindow.xaml
   /// </summary>
   public partial class AreaWindow : Window {
      private string path;

      public AreaWindow() {
         InitializeComponent();
      }

      public AreaWindow(string path) {
         InitializeComponent();
         this.path = path;
         LoadArea();
      }

      private void btnSave_Click(object sender, RoutedEventArgs e) {
         if (string.IsNullOrEmpty(path)) {
            // Create OpenFileDialog
            Microsoft.Win32.SaveFileDialog saveFileDlg = new Microsoft.Win32.SaveFileDialog();
            saveFileDlg.Filter = "Climbing map area file | area.json";

            // Launch OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = saveFileDlg.ShowDialog();
            if (result == true) {
               path = saveFileDlg.FileName;
            } else {
               return;
            }
         }

         File.WriteAllText(path, JsonConvert.SerializeObject(InitializeArea()));

         Close();
      }

      private object InitializeArea() {
         var result = new Area() {
            Id = txtId.Text,
            Name = txtName.Text,
            Info = txtInfo.Text,
            Approach = txtApproach.Text,
            Descent = txtDescent.Text,
            Accomodations = txtAccomodations.Text,
            Ethics = txtEthics.Text,
            History = txtHistory.Text,
            Restrictions = txtRestrictions.Text,
            Tags = txtTags.Text.Split(',', StringSplitOptions.RemoveEmptyEntries)
         };

         var locations = new List<Location>();
         if (!string.IsNullOrEmpty(txtLat1.Text)) {
            locations.Add(new Location(double.Parse(txtLat1.Text), double.Parse(txtLon1.Text)));
         }

         if (!string.IsNullOrEmpty(txtLat2.Text)) {
            locations.Add(new Location(double.Parse(txtLat2.Text), double.Parse(txtLon2.Text)));
         }

         if (!string.IsNullOrEmpty(txtLat3.Text)) {
            locations.Add(new Location(double.Parse(txtLat3.Text), double.Parse(txtLon3.Text)));
         }

         if (!string.IsNullOrEmpty(txtLat4.Text)) {
            locations.Add(new Location(double.Parse(txtLat4.Text), double.Parse(txtLon4.Text)));
         }

         if (!string.IsNullOrEmpty(txtLat5.Text)) {
            locations.Add(new Location(double.Parse(txtLat5.Text), double.Parse(txtLon5.Text)));
         }

         if (!string.IsNullOrEmpty(txtLat6.Text)) {
            locations.Add(new Location(double.Parse(txtLat6.Text), double.Parse(txtLon6.Text)));
         }
         result.Location = locations.ToArray();

         return result;
      }

      private void LoadArea() {
         Area area = JsonConvert.DeserializeObject<Area>(File.ReadAllText(path));
         txtId.Text = area.Id;
         txtName.Text = area.Name;
         txtInfo.Text = area.Info;
         txtApproach.Text = area.Approach;
         txtDescent.Text = area.Descent;
         txtAccomodations.Text = area.Accomodations;
         txtEthics.Text = area.Ethics;
         txtHistory.Text = area.History;
         txtRestrictions.Text = area.Restrictions;

         if (area.Location.Length > 0) {
            txtLat1.Text = area.Location[0].Latitude.ToString();
            txtLon1.Text = area.Location[0].Longitude.ToString();
         }

         if (area.Location.Length > 1) {
            txtLat2.Text = area.Location[1].Latitude.ToString();
            txtLon2.Text = area.Location[1].Longitude.ToString();
         }

         if (area.Location.Length > 2) {
            txtLat3.Text = area.Location[2].Latitude.ToString();
            txtLon3.Text = area.Location[2].Longitude.ToString();
         }

         if (area.Location.Length > 3) {
            txtLat4.Text = area.Location[3].Latitude.ToString();
            txtLon4.Text = area.Location[3].Longitude.ToString();
         }

         if (area.Location.Length > 4) {
            txtLat5.Text = area.Location[4].Latitude.ToString();
            txtLon5.Text = area.Location[4].Longitude.ToString();
         }

         if (area.Location.Length > 5) {
            txtLat6.Text = area.Location[5].Latitude.ToString();
            txtLon6.Text = area.Location[5].Longitude.ToString();
         }
      }
   }
}
