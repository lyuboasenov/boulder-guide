using BoulderGuide.DTOs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace BoulderGuide.Wpf {
   /// <summary>
   /// Interaction logic for AreaWindow.xaml
   /// </summary>
   public partial class AreaWindow : Window {
      private string path;
      private AreaDTO area;

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

         File.WriteAllText(path, JsonConvert.SerializeObject(InitializeArea(), Formatting.Indented));

         Close();
      }

      private object InitializeArea() {
         var result = area ?? new AreaDTO();
         result.Id = txtId.Text;
         result.Name = txtName.Text;
         result.Info = txtInfo.Text;
         result.Access = txtAccess.Text;
         result.Accommodations = txtAccommodations.Text;
         result.Ethics = txtEthics.Text;
         result.History = txtHistory.Text;
         result.Restrictions = txtRestrictions.Text;
         result.Tags = txtTags.Text.Split(',', StringSplitOptions.RemoveEmptyEntries);

         var locations = new List<Location>();
         if (!string.IsNullOrEmpty(txtLocation1.Text)) {
            locations.Add(new Location(txtLocation1.Text));
         }

         if (!string.IsNullOrEmpty(txtLocation2.Text)) {
            locations.Add(new Location(txtLocation2.Text));
         }

         if (!string.IsNullOrEmpty(txtLocation3.Text)) {
            locations.Add(new Location(txtLocation3.Text));
         }

         if (!string.IsNullOrEmpty(txtLocation4.Text)) {
            locations.Add(new Location(txtLocation4.Text));
         }

         if (!string.IsNullOrEmpty(txtLocation5.Text)) {
            locations.Add(new Location(txtLocation5.Text));
         }

         if (!string.IsNullOrEmpty(txtLocation6.Text)) {
            locations.Add(new Location(txtLocation6.Text));
         }
         result.Location = locations.ToArray();

         return result;
      }

      private void LoadArea() {
         area = JsonConvert.DeserializeObject<AreaDTO>(File.ReadAllText(path));
         txtId.Text = area.Id;
         txtName.Text = area.Name;
         txtInfo.Text = area.Info;
         txtAccess.Text = area.Access;
         txtAccommodations.Text = area.Accommodations;
         txtEthics.Text = area.Ethics;
         txtHistory.Text = area.History;
         txtRestrictions.Text = area.Restrictions;

         if (area.Location.Length > 0) {
            txtLocation1.Text = area.Location[0].ToString();
         }

         if (area.Location.Length > 1) {
            txtLocation2.Text = area.Location[1].ToString();
         }

         if (area.Location.Length > 2) {
            txtLocation3.Text = area.Location[2].ToString();
         }

         if (area.Location.Length > 3) {
            txtLocation4.Text = area.Location[3].ToString();
         }

         if (area.Location.Length > 4) {
            txtLocation5.Text = area.Location[4].ToString();
         }

         if (area.Location.Length > 5) {
            txtLocation6.Text = area.Location[5].ToString();
         }
      }
   }
}
