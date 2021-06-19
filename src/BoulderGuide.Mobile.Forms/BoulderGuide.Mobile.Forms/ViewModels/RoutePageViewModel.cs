using BoulderGuide.DTOs;
using BoulderGuide.Mobile.Forms.Services.Data;
using BoulderGuide.Mobile.Forms.Services.Data.Entities;
using BoulderGuide.Mobile.Forms.Views;
using Prism.Navigation;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;

namespace BoulderGuide.Mobile.Forms.ViewModels {
   public class RoutePageViewModel : ViewModelBase {

      public RouteInfo Info { get; set; }
      public ICommand MapCommand { get; }
      public ICommand ViewSchemaCommand { get; }

      public RoutePageViewModel() {
         MapCommand = new Command(async () => await Map(), CanShowMap);
         ViewSchemaCommand = new Command(async (obj) => await ViewSchema(obj as Schema));
      }

      public override void Initialize(INavigationParameters parameters) {
         base.Initialize(parameters);
         if (parameters.TryGetValue(nameof(RouteInfo), out RouteInfo info)) {
            Task.Run(async () => await InitializeAsync(info));
         } else {
            Task.Run(async () => await NavigationService.GoBackAsync());
         }
      }

      private bool CanShowMap() {
         return Info.Route != null;
      }

      private async Task Map() {
         await NavigateAsync(
            $"{Info.Name} ({new Grade(Info.Difficulty)})",
            $"/MainPage/NavigationPage/{nameof(MapPage)}",
            MapPageViewModel.InitializeParameters(Info),
            Icons.MaterialIconFont.Place);
      }

      private async Task ViewSchema(Schema schema) {
         if (null != schema) {
            await NavigateAsync(
               $"{Info.Name} ({new Grade(Info.Difficulty)})",
               $"/MainPage/NavigationPage/{nameof(SchemaPage)}",
               SchemaPageViewModel.InitializeParameters(Info, schema),
               Icons.MaterialIconFont.Moving);
         }
      }

      private async Task InitializeAsync(RouteInfo info) {
         Info = info;

         try {
            await info.LoadRouteAsync();

            Device.BeginInvokeOnMainThread(() => {
               (MapCommand as Command)?.ChangeCanExecute();
            });
         } catch (Exception ex) {
            await HandleExceptionAsync(ex);
         }
      }

      public static NavigationParameters InitializeParameters(RouteInfo routeInfo) {
         return InitializeParameters(nameof(RouteInfo), routeInfo);
      }
   }
}
