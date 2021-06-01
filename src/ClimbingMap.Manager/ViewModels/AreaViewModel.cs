using ClimbingMap.Domain.Entities;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ClimbingMap.Manager.ViewModels {
   public class AreaViewModel : BindableBase, INavigationAware {

      private string _path;
      private readonly IRegionNavigationService regionNavigationService;

      public ICommand SaveCommand { get; }

      public string Id { get; set; }
      public string Name { get; set; }
      public string Info { get; set; }
      public string Approach { get; set; }
      public string Descent { get; set; }
      public string History { get; set; }
      public string Ethics { get; set; }
      public string Accomodations { get; set; }
      public string Restrictions { get; set; }
      public string Tags { get; set; }

      public double Lat1 { get; set; }
      public double Lon1 { get; set; }

      public double Lat2 { get; set; }
      public double Lon2 { get; set; }

      public double Lat3 { get; set; }
      public double Lon3 { get; set; }

      public double Lat4 { get; set; }
      public double Lon4 { get; set; }

      public double Lat5 { get; set; }
      public double Lon5 { get; set; }

      public double Lat6 { get; set; }
      public double Lon6 { get; set; }

      public AreaViewModel(IRegionNavigationService regionNavigationService) {

         SaveCommand = new DelegateCommand(async () => await Save());
         this.regionNavigationService = regionNavigationService;
      }

      private Task Save() {
         if (string.IsNullOrEmpty(_path)) {
            // Create OpenFileDialog
            Microsoft.Win32.SaveFileDialog saveFileDlg = new Microsoft.Win32.SaveFileDialog();
            saveFileDlg.Filter = "area.json";

            // Launch OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = saveFileDlg.ShowDialog();
            if (result == true) {
               _path = saveFileDlg.FileName;
            } else {
               return Task.CompletedTask;
            }
         }

         File.WriteAllText(_path, JsonConvert.SerializeObject(InitializeArea()));

         regionNavigationService.RequestNavigate(new Uri("HomeView", UriKind.Relative));

         return Task.CompletedTask;
      }

      private Area InitializeArea() {
         var result = new Area() {
            Id = Id,
            Name = Name,
            Info = Info,
            Approach = Approach,
            Descent = Descent,
            Accomodations = Accomodations,
            Ethics = Ethics,
            History = History,
            Restrictions = Restrictions
         };

         foreach (string tag in Tags.Split(',')) {
            result.Tags.Add(tag.Trim());
         }

         if (Lat1 != 0) {
            result.Location.Add(new Location(Lat1, Lon1));
         }

         if (Lat2 != 0) {
            result.Location.Add(new Location(Lat2, Lon2));
         }

         if (Lat3 != 0) {
            result.Location.Add(new Location(Lat3, Lon3));
         }

         if (Lat4 != 0) {
            result.Location.Add(new Location(Lat4, Lon4));
         }

         if (Lat5 != 0) {
            result.Location.Add(new Location(Lat5, Lon5));
         }

         if (Lat6 != 0) {
            result.Location.Add(new Location(Lat6, Lon6));
         }

         return result;
      }

      public void OnNavigatedTo(NavigationContext navigationContext) {
         navigationContext.Parameters.TryGetValue("path", out _path);
      }

      public bool IsNavigationTarget(NavigationContext navigationContext) {
         return true;
      }

      public void OnNavigatedFrom(NavigationContext navigationContext) {

      }
   }
}
