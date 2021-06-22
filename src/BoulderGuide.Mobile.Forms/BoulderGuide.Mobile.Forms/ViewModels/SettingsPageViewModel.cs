using BoulderGuide.Mobile.Forms.Services.Data;
using BoulderGuide.Mobile.Forms.Views;
using Prism.Navigation;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials.Interfaces;

namespace BoulderGuide.Mobile.Forms.ViewModels {
   public class SettingsPageViewModel : ViewModelBase {
      private readonly Services.Preferences.IPreferences preferences;
      private readonly IDataService dataService;
      private readonly IVersionTracking versionTracking;
      private DateTime lastTapped = DateTime.MinValue;
      private int tapCounter;

      public IEnumerable<int> GpsPollingIntervalList { get; set; }
      public int SelectedGpsPollingInterval { get; set; }
      public void OnSelectedGpsPollingIntervalChanged() {
         preferences.GPSPollIntervalInSeconds = SelectedGpsPollingInterval;
      }
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

         ClearLocalDataCommand = new AsyncCommand(ClearLocalData);
         SettingsTappedCommand = new AsyncCommand(SettingsTapped);
      }

      public override async Task InitializeAsync(INavigationParameters parameters) {
         try {
            await base.InitializeAsync(parameters).ConfigureAwait(false);

            Version = versionTracking.CurrentVersion;
            GpsPollingIntervalList = new[] { 1, 2, 3, 4, 5, 10, 15, 20, 25, 30, 40, 50, 60 };
            SelectedGpsPollingInterval = preferences.GPSPollIntervalInSeconds;

            LocalStorageSizeInMB =
               await dataService.
                  GetLocalStorageSizeInMB().
                  ConfigureAwait(false);
         } catch (Exception ex) {
            HandleOperationException(ex, Strings.UnableToInitializeSettings);
         }
      }

      private async Task SettingsTapped() {
         try {
            if (DateTime.Now.Subtract(lastTapped).TotalSeconds <= 1) {
               tapCounter++;
               if (tapCounter == 9) {
                  await ShowDialogAsync(nameof(KeyDialogPage)).
                     ConfigureAwait(false);
                  tapCounter = 0;
               }
            } else {
               tapCounter = 1;
            }
            lastTapped = DateTime.Now;
         } catch (Exception ex) {
            HandleOperationException(ex, Strings.UnableToEnterKey);
         }
      }

      private async Task ClearLocalData() {
         try {
            await dataService.
               ClearLocalStorage().
               ConfigureAwait(false);
            LocalStorageSizeInMB =
               await dataService.
               GetLocalStorageSizeInMB().
               ConfigureAwait(false);
         } catch (Exception ex) {
            HandleOperationException(ex, Strings.UnableToClearLocalData);
         }
      }
   }
}
