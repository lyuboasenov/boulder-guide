using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using Xamarin.Essentials.Interfaces;

namespace ClimbingMap.Mobile.Forms.ViewModels {
   public class MainPageViewModel : ViewModelBase {
      private readonly IFileSystem fileSystem;

      public MainPageViewModel(INavigationService navigationService, IFileSystem fileSystem)
          : base(navigationService) {
         Title = "Main Page";
         this.fileSystem = fileSystem;
      }

      public override void OnNavigatedTo(INavigationParameters parameters) {
         base.OnNavigatedTo(parameters);

         var dict = new Dictionary<string, string>() {
            {
               "rila-monastery", "https://raw.githubusercontent.com/lyuboasenov/climbing-map-rila-monastery/main/index.json"
            }
         };

         foreach (var kv in dict) {
            var repoDir = Path.Combine(fileSystem.AppDataDirectory, kv.Key);

            using (var httpClient = new HttpClient()) {
               using (var response = httpClient.GetAsync(kv.Value).Result) {
                  response.EnsureSuccessStatusCode();

                  var indexPath = Path.Combine(repoDir, "index.json");
                  using (var stream = File.Open(indexPath, FileMode.CreateNew)) {
                     response.Content.CopyToAsync(stream).Wait();
                  }
               }
            }
         }
      }
   }
}
