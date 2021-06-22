using BoulderGuide.Mobile.Forms.Domain;
using Prism.Services.Dialogs;

namespace BoulderGuide.Mobile.Forms.ViewModels {
   public class VideosDialogPageViewModel : DialogViewModelBase {
      public RouteInfo Info { get; set; }

      public override void OnDialogOpened(IDialogParameters parameters) {
         base.OnDialogOpened(parameters);
         if (parameters.TryGetValue(nameof(Info), out RouteInfo info)) {
            Info = info;
         }
      }

      internal static IDialogParameters InitializeParameters(RouteInfo info) {
         return new DialogParameters() {
            { nameof(Info), info }
         };
      }
   }
}
