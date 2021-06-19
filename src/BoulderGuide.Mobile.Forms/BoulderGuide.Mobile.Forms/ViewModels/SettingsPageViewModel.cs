using BoulderGuide.Mobile.Forms.Services.Data;
using BoulderGuide.Mobile.Forms.Views;
using Prism.Navigation;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;

namespace BoulderGuide.Mobile.Forms.ViewModels {
   public class SettingsPageViewModel : ViewModelBase {
      private readonly Services.Preferences.IPreferences preferences;
      private readonly IDataService dataService;
      private readonly IVersionTracking versionTracking;
      private DateTime lastTapped = DateTime.MinValue;
      private int tapCounter;

      public IEnumerable<int> GpsPollingIntervalList { get; set; }
      public int SelectedGpsPollingInterval { get; set; }
      public int LocalStorageSizeInMB { get; set; }
      public ICommand ClearLocalDataCommand { get; }
      public ICommand SettingsTappedCommand { get; }
      public string Version { get; set; }

      public SettingsPageViewModel(
         Services.Preferences.IPreferences preferences,
         IDataService dataService,
         IVersionTracking versionTracking) {
         this.preferences = preferences;
         this.dataService = dataService;
         this.versionTracking = versionTracking;

         ClearLocalDataCommand = new Command(async () => await ClearLocalData());
         SettingsTappedCommand = new Command(async () => await SettingsTapped());
      }

      public override async Task InitializeAsync(INavigationParameters parameters) {
         await base.InitializeAsync(parameters);

         Version = versionTracking.CurrentVersion;
         GpsPollingIntervalList = new[] { 1, 2, 3, 4, 5, 10, 15, 20, 25, 30, 40, 50, 60 };
         SelectedGpsPollingInterval = preferences.GPSPollIntervalInSeconds;

         await dataService.GetLocalStorageSizeInMB();
      }

      private async Task SettingsTapped() {
         if (DateTime.Now.Subtract(lastTapped).TotalSeconds <= 2) {
            tapCounter++;
            if (tapCounter >= 9) {
               await DialogService.ShowDialogAsync(nameof(EnterPasswordDialogPage));
               tapCounter = 0;
            }
         } else {
            tapCounter = 1;
         }
         lastTapped = DateTime.Now;
      }

      private async Task ClearLocalData() {
         await dataService.ClearLocalStorage();
         LocalStorageSizeInMB = await dataService.GetLocalStorageSizeInMB();
      }

      public void OnSelectedGpsPollingIntervalChanged() {
         preferences.GPSPollIntervalInSeconds = SelectedGpsPollingInterval;
      }
   }
}
