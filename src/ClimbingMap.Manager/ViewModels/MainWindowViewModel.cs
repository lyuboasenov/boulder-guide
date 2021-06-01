using ClimbingMap.Domain.Entities;
using ClimbingMap.Manager.Models;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ClimbingMap.Manager.ViewModels {
   public class MainWindowViewModel : BindableBase, INavigationAware {
      private string _title = "ClimbingMap Manager";

      private const string RepositoryPathSettingsKey = "REPOSITORY_PATH";
      private readonly IDialogService dialogService;
      private readonly IRegionNavigationService regionNavigationService;

      public string RepositoryPath { get; set; }
      public ObservableCollection<RepoItem> RepoStructure { get; private set; } = new ObservableCollection<RepoItem>();
      public ICommand BrowseClickedCommand { get; }

      public string Title {
         get { return _title; }
         set { SetProperty(ref _title, value); }
      }

      public MainWindowViewModel(IDialogService dialogService, IRegionNavigationService regionNavigationService) {

         BrowseClickedCommand = new DelegateCommand(async () => await BrowseClicked());
         this.dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

         this.regionNavigationService = regionNavigationService;

         var prismApp = Prism.PrismApplicationBase.Current as Prism.PrismApplicationBase;

         IRegionManager regionManager = prismApp.Container.Resolve(typeof(IRegionManager)) as IRegionManager;
      }


      public void OnNavigatedTo(NavigationContext navigationContext) {
         regionNavigationService.RequestNavigate(new Uri("HomeView", UriKind.Relative));
      }

      public bool IsNavigationTarget(NavigationContext navigationContext) {
         return true;
      }

      public void OnNavigatedFrom(NavigationContext navigationContext) {

      }




      private async Task BrowseClicked() {
         // Create OpenFileDialog
         Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();
         openFileDlg.CheckFileExists = true;
         openFileDlg.Filter = "index.json";

         // Launch OpenFileDialog by calling ShowDialog method
         Nullable<bool> result = openFileDlg.ShowDialog();
         if (result == true) {
            if (string.IsNullOrEmpty(openFileDlg.FileName)) {
               var parameters = new DialogParameters();
               parameters.Add(DialogViewModel.ParameterKeys.Message, "Please select the root index.json file.");
               parameters.Add(DialogViewModel.ParameterKeys.Severity, DialogViewModel.Severity.Error);
               dialogService.ShowDialog("Select the root index.json", parameters, _ => { });
            } else if (!openFileDlg.SafeFileName.Equals("index.json", StringComparison.OrdinalIgnoreCase)) {
               var parameters = new DialogParameters();
               parameters.Add(DialogViewModel.ParameterKeys.Message, "Please select the root index.json file. Not any other file.");
               parameters.Add(DialogViewModel.ParameterKeys.Severity, DialogViewModel.Severity.Error);
               dialogService.ShowDialog("Select the root index.json", parameters, _ => { });
            } else if (!System.IO.File.Exists(openFileDlg.FileName)) {
               var parameters = new DialogParameters();
               parameters.Add(DialogViewModel.ParameterKeys.Message, "The file you selected does not exists. Please select the root index.json file.");
               parameters.Add(DialogViewModel.ParameterKeys.Severity, DialogViewModel.Severity.Error);
               dialogService.ShowDialog("File does not exists", parameters, _ => { });
            } else {
               RepositoryPath = openFileDlg.FileName;
               FileInfo fileInfo = new FileInfo(RepositoryPath);

               RepoStructure.Clear();
               RepoStructure.Add(await InitializeDirectory(fileInfo.Directory));
            }
         }

      }

      private Task<RepoItem> InitializeDirectory(DirectoryInfo directory) {

         var routeFilePath = System.IO.Path.Combine(directory.FullName, "route.json");
         var areaFilePath = System.IO.Path.Combine(directory.FullName, "area.json");

         if (File.Exists(routeFilePath)) {
            Route route = JsonConvert.DeserializeObject<Route>(File.ReadAllText(routeFilePath));
         } else {

         }
         // recursion
         var subAreas =
            directory.
               GetDirectories().
               Select(dir => InitializeDirectory(dir));

         // actual work
         Area area = JsonConvert.DeserializeObject<Area>(File.ReadAllText(areaFilePath));

         // add routes
         var routes =
            directory.
               GetFiles("*.json").
               Where(f => !f.Name.Equals("area.json", StringComparison.OrdinalIgnoreCase)).
               Select(f => InitializeFile(f));

         Task.WaitAll(subAreas.ToArray());
         Task.WaitAll(routes.ToArray());

         //foreach (var node in subAreas) {
         //   area.Areas.Add(node.Result);
         //}
         //foreach (var node in routes) {
         //   area.Routes.Add(node.Result);
         //}

         //return Task.FromResult(area);
         throw new NotImplementedException();
      }

      private Task<Route> InitializeFile(FileInfo f) {
         return Task.FromResult(
            JsonConvert.DeserializeObject<Route>(File.ReadAllText(f.FullName)));
      }
   }
}
