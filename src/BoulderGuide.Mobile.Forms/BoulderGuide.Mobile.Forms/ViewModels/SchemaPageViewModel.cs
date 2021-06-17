using BoulderGuide.Domain.Entities;
using BoulderGuide.Domain.Schema;
using BoulderGuide.Mobile.Forms.Services.Data;
using Prism.Navigation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BoulderGuide.Mobile.Forms.ViewModels {
   public class SchemaPageViewModel : ViewModelBase {


      public Route Route { get; set; }
      public RouteInfo Info { get; set; }
      public Schema Schema { get; set; }

      public SchemaPageViewModel() {

      }

      public override void Initialize(INavigationParameters parameters) {
         base.Initialize(parameters);
         if (parameters.TryGetValue(nameof(Route), out Route route) &&
            parameters.TryGetValue(nameof(Info), out RouteInfo info) &&
            parameters.TryGetValue(nameof(Schema), out Schema schema)) {
            Route = route;
            Info = info;
            Schema = schema;
         } else {
            Task.Run(async () => await NavigationService.GoBackAsync());
         }
      }

      internal static INavigationParameters InitializeParameters(Route route, RouteInfo info, Schema schema) {
         return InitializeParameters(
            new KeyValuePair<string, object>(nameof(Route), route),
            new KeyValuePair<string, object>(nameof(Info), info),
            new KeyValuePair<string, object>(nameof(Schema), schema));
      }
   }
}
