using BoulderGuide.DTOs;
using BoulderGuide.Mobile.Forms.Services.Data.Entities;
using Prism.Navigation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BoulderGuide.Mobile.Forms.ViewModels {
   public class SchemaPageViewModel : ViewModelBase {
      public RouteInfo Info { get; set; }
      public Schema Schema { get; set; }

      public override void Initialize(INavigationParameters parameters) {
         base.Initialize(parameters);
         if (parameters.TryGetValue(nameof(Info), out RouteInfo info) &&
            parameters.TryGetValue(nameof(Schema), out Schema schema)) {
            Info = info;
            Schema = schema;
         } else {
            Task.Run(async () => await NavigationService.GoBackAsync());
         }
      }

      internal static INavigationParameters InitializeParameters(RouteInfo info, Schema schema) {
         return InitializeParameters(
            new KeyValuePair<string, object>(nameof(Info), info),
            new KeyValuePair<string, object>(nameof(Schema), schema));
      }
   }
}
