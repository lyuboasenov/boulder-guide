﻿using BoulderGuide.Mobile.Forms.Services.Data;
using BoulderGuide.Mobile.Forms.Views;
using Prism.Navigation;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;

namespace BoulderGuide.Mobile.Forms.ViewModels {
   public class MainPageViewModel : ViewModelBase {

      public ObservableCollection<AreaInfo> AreaInfos { get; } = new ObservableCollection<AreaInfo>();
      public AreaInfo SelectedAreaInfo { get; set; }
      public bool IsLoading { get; set; }
      public Breadcrumbs.Item SelectectedBreadcrumbsItem { get; set; }

      public ICommand ReloadCommand { get; }
      public ICommand SettingsCommand { get; }

      private readonly IDataService dataService;
      private readonly IConnectivity connectivity;
      private readonly IPermissions permissions;

      public MainPageViewModel(
         INavigationService navigationService,
         IDialogService dialogService,
         IDataService dataService,
         IConnectivity connectivity,
         IPermissions permissions)
          : base(navigationService, dialogService) {

         ReloadCommand = new Command(async (f) => await Reload((bool)f));
         SettingsCommand = new Command(async () => await Settings());

         Title = Strings.ClimbingAreas;
         this.dataService = dataService;
         this.connectivity = connectivity;
         this.permissions = permissions;
      }

      public override void OnNavigatedTo(INavigationParameters parameters) {
         base.OnNavigatedTo(parameters);

         ReloadCommand.Execute(false);

         Task.Run(async () => {
            await InitializeAsync();
         });
      }

      public void OnSelectedAreaInfoChanged() {
         NavigateAsync(
            SelectedAreaInfo.Name,
            $"/MainPage/NavigationPage/{nameof(AreaDetailsPage)}",
            AreaDetailsPageViewModel.InitializeParameters(SelectedAreaInfo));
      }
      private async Task InitializeAsync() {
         var status = await permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
         if (status != PermissionStatus.Granted) {
            status = await permissions.RequestAsync<Permissions.LocationWhenInUse>();
         }
      }

      private async Task Settings() {
         await NavigateAsync(
            Strings.Settings,
            "/MainPage/NavigationPage/SettingsPage");
      }

      private async Task Reload(bool force) {
         var status = await permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
         if (status != PermissionStatus.Granted) {
            status = await permissions.RequestAsync<Permissions.LocationWhenInUse>();
         }

         IsLoading = true;
         AreaInfos.Clear();

         IEnumerable<AreaInfo> areas;
         if (connectivity.NetworkAccess != NetworkAccess.Internet) {
            areas = await dataService.GetOfflineAreas();
         } else {
            areas = await dataService.GetAreas(force);
         }

         foreach (var area in areas ?? Enumerable.Empty<AreaInfo>()) {
            AreaInfos.Add(area);
         }
         IsLoading = false;
      }

      public void OnSelectectedBreadcrumbsItemChanged() {
         for (int i = Breadcrumbs.Items.Count - 1; i >= 0; i--) {
            var current = Breadcrumbs.Items[i];
            Breadcrumbs.Items.RemoveAt(i);
            if (current == SelectectedBreadcrumbsItem) {
               NavigateAsync(current.Name, current.Path, current.Parameters);
               break;
            }
         }
      }
   }
}