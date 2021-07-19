using BoulderGuide.Mobile.Forms.Services.Data;
using BoulderGuide.Mobile.Forms.Views;
using Prism.Navigation;
using Prism.Services;
using System;
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

      public int LocalStorageSizeInMB { get; set; }

      private bool initialized;

      public ICommand ClearLocalDataCommand { get; }
      public ICommand SettingsTappedCommand { get; }
      public string Version { get; set; }
      public bool IsAdvancedModeEnabled { get; set; }
      public void OnIsAdvancedModeEnabledChanged() {
         if (!IsAdvancedModeEnabled) {
            preferences.IsAdvancedModeEnabled = false;
            preferences.ShowPrivateRegions = false;
            preferences.PrivateRegionsKey = string.Empty;
            preferences.IsDeveloperEnabled = false;

            dialogService.
               DisplayAlertAsync(
                  Strings.Settings,
                  Strings.AdvancedModeDisabled,
                  Strings.Ok).ConfigureAwait(false);
         }
      }
      public bool IsDeveloperEnabled { get; set; }

      public void OnIsDeveloperEnabledChanged() {
         preferences.IsDeveloperEnabled = IsDeveloperEnabled;
      }
      public bool ShowPrivateAreas { get; set; }

      public void OnShowPrivateAreasChanged() {
         if (initialized) {
            RunOnMainThreadAsync(LockUnlock);
         }
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
      }

      public override async Task InitializeAsync(INavigationParameters parameters) {
         try {
            await base.InitializeAsync(parameters).ConfigureAwait(false);

            Version = versionTracking.CurrentVersion;
            IsDeveloperEnabled = preferences.IsDeveloperEnabled;
            IsAdvancedModeEnabled = preferences.IsAdvancedModeEnabled;
            ShowPrivateAreas = preferences.ShowPrivateRegions;

            LocalStorageSizeInMB =
               await dataService.
                  GetLocalStorageSizeInMBAsync().
                  ConfigureAwait(false);

            initialized = true;
         } catch (Exception ex) {
            await HandleOperationExceptionAsync(ex, Strings.UnableToInitializeSettings);
         }
      }

      private async Task SettingsTapped() {
         try {
            if (DateTime.Now.Subtract(lastTapped).TotalSeconds <= 1) {
               tapCounter++;
               if (tapCounter == 9) {
                  IsAdvancedModeEnabled = true;
                  preferences.IsAdvancedModeEnabled = true;
                  tapCounter = 0;

                  await dialogService.
                     DisplayAlertAsync(
                        Strings.Settings,
                        Strings.AdvancedModeEnabled,
                        Strings.Ok).ConfigureAwait(false);
               }
            } else {
               tapCounter = 1;
            }
            lastTapped = DateTime.Now;
         } catch (Exception ex) {
            await HandleOperationExceptionAsync(ex, Strings.UnableToEnableAddvancedMode);
         }
      }

      private async Task LockUnlock() {
         try {
            if (preferences.ShowPrivateRegions) {
               // lock
               preferences.ShowPrivateRegions = false;
               preferences.PrivateRegionsKey = string.Empty;
            } else {
               // unlock
               await ShowDialogAsync(nameof(KeyDialogPage)).
                  ConfigureAwait(false);
            }

            ShowPrivateAreas = preferences.ShowPrivateRegions;
         } catch (Exception ex) {
            await HandleOperationExceptionAsync(ex, Strings.UnableToEnterKey);
         }
      }

      private async Task ClearLocalData() {
         try {
            await dataService.
               ClearLocalStorageAsync().
               ConfigureAwait(false);
            LocalStorageSizeInMB =
               await dataService.
               GetLocalStorageSizeInMBAsync().
               ConfigureAwait(false);
         } catch (Exception ex) {
            await HandleOperationExceptionAsync(ex, Strings.UnableToClearLocalData);
         }
      }
   }
}
