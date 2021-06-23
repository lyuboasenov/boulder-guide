using BoulderGuide.Mobile.Forms.Services.Data;
using BoulderGuide.Mobile.Forms.Views;
using Prism.Navigation;
using Prism.Services;
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
      private readonly IPageDialogService dialogService;

      public IEnumerable<int> GpsPollingIntervalList { get; set; }
      public int SelectedGpsPollingInterval { get; set; }
      public void OnSelectedGpsPollingIntervalChanged() {
         preferences.GPSPollIntervalInSeconds = SelectedGpsPollingInterval;
      }
      public int LocalStorageSizeInMB { get; set; }
      public ICommand ClearLocalDataCommand { get; }
      public ICommand SettingsTappedCommand { get; }
      public ICommand EnterKeyCommand { get; }
      public string Version { get; set; }
      public bool IsAdvancedModeEnabled { get; set; }
      public bool IsDeveloperEnabled { get; set; }

      public void OnIsDeveloperEnabledChanged() {
         preferences.IsDeveloperEnabled = IsDeveloperEnabled;
      }

      public SettingsPageViewModel(
         Services.Preferences.IPreferences preferences,
         IDataService dataService,
         IVersionTracking versionTracking,
         IPageDialogService dialogService) {
         this.preferences = preferences;
         this.dataService = dataService;
         this.versionTracking = versionTracking;
         this.dialogService = dialogService;

         ClearLocalDataCommand = new AsyncCommand(ClearLocalData);
         SettingsTappedCommand = new AsyncCommand(SettingsTapped);
         EnterKeyCommand = new AsyncCommand(EnterKey);
      }

      public override async Task InitializeAsync(INavigationParameters parameters) {
         try {
            await base.InitializeAsync(parameters).ConfigureAwait(false);

            Version = versionTracking.CurrentVersion;
            GpsPollingIntervalList = new[] { 1, 2, 3, 4, 5, 10, 15, 20, 25, 30, 40, 50, 60 };
            SelectedGpsPollingInterval = preferences.GPSPollIntervalInSeconds;
            IsDeveloperEnabled = preferences.IsDeveloperEnabled;
            IsAdvancedModeEnabled = preferences.IsAdvancedModeEnabled;

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
                  IsAdvancedModeEnabled = !IsAdvancedModeEnabled;
                  preferences.IsAdvancedModeEnabled = IsAdvancedModeEnabled;
                  tapCounter = 0;

                  if (IsAdvancedModeEnabled) {
                     await dialogService.
                        DisplayAlertAsync(
                           Strings.Settings,
                           Strings.AdvancedModeEnabled,
                           Strings.Ok).ConfigureAwait(false);
                  } else {
                     preferences.PrivateRegionsKey = string.Empty;
                     preferences.ShowPrivateRegions = false;
                     preferences.IsDeveloperEnabled = false;

                     await dialogService.
                        DisplayAlertAsync(
                           Strings.Settings,
                           Strings.AdvancedModeDisabled,
                           Strings.Ok).ConfigureAwait(false);
                  }
               }
            } else {
               tapCounter = 1;
            }
            lastTapped = DateTime.Now;
         } catch (Exception ex) {
            HandleOperationException(ex, Strings.UnableToEnableAddvancedMode);
         }
      }

      private async Task EnterKey() {
         try {
            await ShowDialogAsync(nameof(KeyDialogPage)).
               ConfigureAwait(false);
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
