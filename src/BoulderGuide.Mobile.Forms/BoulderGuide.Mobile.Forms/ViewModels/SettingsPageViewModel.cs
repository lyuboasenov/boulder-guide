using BoulderGuide.Mobile.Forms.Services.Data;
using Prism.Navigation;
using Prism.Services.Dialogs;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace BoulderGuide.Mobile.Forms.ViewModels {
   public class SettingsPageViewModel : ViewModelBase {
      private readonly Services.Preferences.IPreferences preferences;
      private readonly IDataService dataService;

      public IEnumerable<int> GpsPollingIntervalList { get; set; }
      public int SelectedGpsPollingInterval { get; set; }
      public int LocalStorageSizeInMB { get; set; }
      public ICommand ClearLocalDataCommand { get; }


      public SettingsPageViewModel(
         INavigationService navigationService,
         IDialogService dialogService,
         Services.Preferences.IPreferences preferences,
         IDataService dataService) : base(navigationService, dialogService) {
         this.preferences = preferences;
         this.dataService = dataService;

         ClearLocalDataCommand = new Command(async () => await ClearLocalData());
      }

      public override void Initialize(INavigationParameters parameters) {
         base.Initialize(parameters);


         GpsPollingIntervalList = new[] { 1, 2, 3, 4, 5, 10, 15, 20, 25, 30, 40, 50, 60 };
         SelectedGpsPollingInterval = preferences.GPSPollIntervalInSeconds;
         Task.Run(async () => {

            LocalStorageSizeInMB = await dataService.GetLocalStorageSizeInMB();
         });
      }

      private Task ClearLocalData() {
         return dataService.ClearLocalStorage();
      }

      public void OnSelectedGpsPollingIntervalChanged() {
         preferences.GPSPollIntervalInSeconds = SelectedGpsPollingInterval;
      }
   }
}
