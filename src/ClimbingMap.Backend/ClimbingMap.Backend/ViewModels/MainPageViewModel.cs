using ClimbingMap.Domain.Entities;
using Newtonsoft.Json;
using Prism.Navigation;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TemplateUI.Controls;
using Xamarin.Essentials;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;

namespace ClimbingMap.Backend.ViewModels {
   public class MainPageViewModel : ViewModelBase {
      private const string RepositoryPathSettingsKey = "REPOSITORY_PATH";
      public string RepositoryPath { get; set; }
      public TreeViewNodes RepoStructure { get; private set; }
      public TreeViewNode SelectedRepoNode { get; set; }
      public ICommand BrowseClickedCommand { get; }
      public IPreferences Preferences { get; }
      public IFilePicker FilePicker { get; }
      public IDialogService DialogService { get; }

      public MainPageViewModel(
         INavigationService navigationService,
         IPreferences preferences,
         IFilePicker filePicker,
         IDialogService dialogService)
          : base(navigationService) {
         Preferences = preferences ?? throw new ArgumentNullException(nameof(preferences));
         FilePicker = filePicker ?? throw new ArgumentNullException(nameof(filePicker));
         DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

         Title = "Repository";
         BrowseClickedCommand = new Command(async () => await BrowseClicked());

         RepositoryPath = Preferences.Get(RepositoryPathSettingsKey, "Select repository");

      }
      private async Task BrowseClicked() {
         var filePickerOptions = new PickOptions() {
            PickerTitle = "Select root index.json file",
            FileTypes = new FilePickerFileType(
               new Dictionary<DevicePlatform, IEnumerable<string>>() {
                  { DevicePlatform.UWP, new[] { "json" } }
               })
         };

         FileResult selectedFile = await FilePicker.PickAsync(filePickerOptions);

         if (string.IsNullOrEmpty(selectedFile.FullPath)) {
            var parameters = new DialogParameters();
            parameters.Add(DialogViewModel.ParameterKeys.Message, "Please select the root index.json file.");
            parameters.Add(DialogViewModel.ParameterKeys.Severity, DialogViewModel.Severity.Error);
            DialogService.ShowDialog("Select the root index.json", parameters);
         } else if (!selectedFile.FileName.Equals("index.json", StringComparison.OrdinalIgnoreCase)) {
            var parameters = new DialogParameters();
            parameters.Add(DialogViewModel.ParameterKeys.Message, "Please select the root index.json file. Not any other file.");
            parameters.Add(DialogViewModel.ParameterKeys.Severity, DialogViewModel.Severity.Error);
            DialogService.ShowDialog("Select the root index.json", parameters);
         } else if (!System.IO.File.Exists(selectedFile.FileName)) {
            var parameters = new DialogParameters();
            parameters.Add(DialogViewModel.ParameterKeys.Message, "The file you selected does not exists. Please select the root index.json file.");
            parameters.Add(DialogViewModel.ParameterKeys.Severity, DialogViewModel.Severity.Error);
            DialogService.ShowDialog("File does not exists", parameters);
         } else {
            RepositoryPath = selectedFile.FullPath;
            FileInfo fileInfo = new FileInfo(RepositoryPath);

            RepoStructure = new TreeViewNodes();
            RepoStructure.Add(await InitializeDirectory(fileInfo.Directory));
         }
      }

      private Task<TreeViewNode> InitializeDirectory(DirectoryInfo directory) {
         // recursion
         var subNodes =
            directory.
               GetDirectories().
               Select(dir => InitializeDirectory(dir)).ToList();

         // actual work
         Area area = JsonConvert.DeserializeObject<Area>(File.ReadAllText(Path.Combine(directory.FullName, "index.json")));

         // add routes
         subNodes.AddRange(
            directory.
               GetFiles("*.json").
               Where(f => !f.Name.Equals("index.json", StringComparison.OrdinalIgnoreCase)).
               Select(f => InitializeFile(f)));

         Task.WaitAll(subNodes.ToArray());

         var result = new TreeViewNode();
         foreach (var node in subNodes) {
            result.Children.Add(node.Result);
         }

         return Task.FromResult(result);
      }

      private Task<TreeViewNode> InitializeFile(FileInfo f) {
         Route route = JsonConvert.DeserializeObject<Route>(File.ReadAllText(f.FullName));

         return Task.FromResult(
            new TreeViewNode() {
               Text = $"{route.Name} ({route.Grade})"
            });
      }
   }
}
