﻿using BoulderGuide.DTOs;
using BoulderGuide.Mobile.Forms.Domain;
using BoulderGuide.Mobile.Forms.Services.Data;
using BoulderGuide.Mobile.Forms.Services.UI;
using BoulderGuide.Mobile.Forms.Views;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials.Interfaces;

namespace BoulderGuide.Mobile.Forms.ViewModels {
   public class AreaPageViewModel : ViewModelBase {

      private readonly IDataService dataService;
      private readonly IConnectivity connectivity;
      private readonly IActivityIndicationService activityIndicationService;
      private readonly Services.Preferences.IPreferences preferences;

      public ICommand DownloadCommand { get; }
      public ICommand MapCommand { get; }
      public ICommand FilterCommand { get; }
      public ICommand OrderCommand { get; }
      public AreaInfo Info { get; set; }
      public object SelectedChild { get; set; }
      public void OnSelectedChildChanged() {
         RunAsync(NavigateAsync);
      }
      public ObservableCollection<object> Children { get; set; } = new ObservableCollection<object>();

      public AreaPageViewModel(
         IDataService dataService,
         IConnectivity connectivity,
         IActivityIndicationService activityIndicationService,
         Services.Preferences.IPreferences preferences) {
         this.dataService = dataService;
         this.connectivity = connectivity;
         this.activityIndicationService = activityIndicationService;
         this.preferences = preferences;

         DownloadCommand = new AsyncCommand(Download);
         FilterCommand = new AsyncCommand(Filter);
         OrderCommand = new AsyncCommand(Order);
         MapCommand = new AsyncCommand(Map, CanShowMap);
      }

      private async Task Order() {
         try {
            await ShowDialogAsync(nameof(OrderDialogPage)).
               ConfigureAwait(false);
            await InitializeAsync().
               ConfigureAwait(false);
         } catch (Exception ex) {
            await HandleOperationExceptionAsync(ex, Strings.UnableToSort);
         }
      }

      private async Task Filter() {
         try {
            await ShowDialogAsync(nameof(FilterDialogPage)).
               ConfigureAwait(false);
            await InitializeAsync().
               ConfigureAwait(false);
         } catch (Exception ex) {
            await HandleOperationExceptionAsync (ex, Strings.UnableToFilter);
         }
      }

      private async Task Map() {
         await NavigateAsync(
            Info.Name,
            $"/MainPage/NavigationPage/{nameof(MapPage)}",
            MapPageViewModel.InitializeParameters(Info),
            Icons.MaterialIconFont.Map).ConfigureAwait(false);
      }

      private bool CanShowMap() {
         return null != Info?.Area?.IsInitialized;
      }

      public override void OnNavigatedTo(INavigationParameters parameters) {
         base.OnNavigatedTo(parameters);

         if (parameters.TryGetValue(nameof(AreaInfo), out AreaInfo info)) {
            Info = info;
            RunAsync(InitializeAsync);
         }
      }

      public override bool CanNavigate(INavigationParameters parameters) {
         return base.CanNavigate(parameters) &&
            parameters.TryGetValue(nameof(AreaInfo), out AreaInfo _);
      }

      private async Task NavigateAsync() {
         if (SelectedChild is AreaInfo areaInfo) {
            await NavigateAsync(
               areaInfo.Name,
               $"/MainPage/NavigationPage/{nameof(AreaPage)}",
               InitializeParameters(areaInfo),
               Icons.MaterialIconFont.Terrain).ConfigureAwait(false);
         } else if (SelectedChild is RouteInfo routeInfo) {
            await NavigateAsync(
               $"{routeInfo.Name} ({new Grade(routeInfo.Difficulty)})",
               $"/MainPage/NavigationPage/{nameof(RoutePage)}",
               RoutePageViewModel.InitializeParameters(routeInfo),
               Icons.MaterialIconFont.Moving).ConfigureAwait(false);
         }
      }

      public static NavigationParameters InitializeParameters(AreaInfo info) {
         return InitializeParameters(nameof(AreaInfo), info);
      }

      private async Task InitializeAsync() {
         var handle =
            await activityIndicationService.
               StartLoadingAsync().
               ConfigureAwait(false);
         try {
            await Info.LoadAreaAsync().ConfigureAwait(false);

            var searchTerm = preferences.FilterSearchTerm.ToLowerInvariant();
            var minDifficulty = preferences.FilterMinDifficulty;
            var maxDifficulty = preferences.FilterMaxDifficulty;
            var setting = preferences.RouteOrderByProperty;

            var areas = OrderAreas(FilterAreas(searchTerm), setting).ToArray();
            var routes = OrderRoutes(FitlerRoutes(searchTerm, minDifficulty, maxDifficulty), setting).ToArray();

            await RunOnMainThreadAsync(() => {
               Children.Clear();

               foreach (var area in areas) {
                  Children.Add(area);
               }

               foreach (var route in routes) {
                  Children.Add(route);
               }

               Children.Add(new object());

               (MapCommand as AsyncCommand)?.ChangeCanExecute();
            }).ConfigureAwait(false);
         } catch (Exception ex) {
            await HandleOperationExceptionAsync(ex, string.Format(Strings.UnableToInitializeAreaFormat, Info?.Name));
         }
         finally {
            handle.Dispose();
         }
      }

      private IEnumerable<object> OrderRoutes(IEnumerable<RouteInfo> enumerable, Services.Preferences.RouteOrderBy setting) {
         if (setting == Services.Preferences.RouteOrderBy.Difficulty) {
            enumerable = enumerable.OrderBy(r => r.Difficulty).ToArray();
         } else if (setting == Services.Preferences.RouteOrderBy.DifficultyDesc) {
            enumerable = enumerable.OrderByDescending(r => r.Difficulty).ToArray();
         } else if (setting == Services.Preferences.RouteOrderBy.NameDesc) {
            enumerable = enumerable.OrderByDescending(r => r.Name).ToArray();
         } else {
            enumerable = enumerable.OrderBy(r => r.Name).ToArray();
         }

         return enumerable;
      }

      private IEnumerable<object> OrderAreas(IEnumerable<AreaInfo> enumerable, Services.Preferences.RouteOrderBy setting) {
         if (setting == Services.Preferences.RouteOrderBy.NameDesc) {
            enumerable = enumerable.OrderByDescending(r => r.Name).ToArray();
         } else {
            enumerable = enumerable.OrderBy(r => r.Name).ToArray();
         }

         return enumerable;
      }

      private IEnumerable<RouteInfo> FitlerRoutes(string searchTerm, int minDifficulty, int maxDifficulty) {
         return Info.Routes?.Where(r =>
            (string.IsNullOrEmpty(searchTerm) || r.Name.ToLowerInvariant().Contains(searchTerm)) &&
            minDifficulty <= r.Difficulty && r.Difficulty <= maxDifficulty || r.Difficulty == -1) ?? Enumerable.Empty<RouteInfo>();
      }

      private IEnumerable<AreaInfo> FilterAreas(string searchTerm) {
         return Info.Areas?.
            Where(a =>
               string.IsNullOrEmpty(searchTerm) ||
               a.Name.ToLowerInvariant().Contains(searchTerm))
               ?? Enumerable.Empty<AreaInfo>();
      }

      private async Task Download() {
         var handle = await activityIndicationService.StartLoadingAsync().ConfigureAwait(false);
         try {
            await Info.DownloadAsync(true).ConfigureAwait(false);
         } catch (Exception ex) {
            await HandleOperationExceptionAsync (ex, string.Format(Strings.UnableToDownloadFormat, Info?.Name));
         } finally {
            handle.Dispose();
         }
      }
   }
}
