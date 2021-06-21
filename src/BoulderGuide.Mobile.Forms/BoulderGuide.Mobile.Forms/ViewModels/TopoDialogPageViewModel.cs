using BoulderGuide.DTOs;
using BoulderGuide.Mobile.Forms.Services.Data.Entities;
using Prism.Services.Dialogs;
using System.Windows.Input;
using Xamarin.Forms;

namespace BoulderGuide.Mobile.Forms.ViewModels {
   public class TopoDialogPageViewModel : DialogViewModelBase {
      public RouteInfo Info { get; set; }
      public Topo Topo { get; set; }

      public double Scale { get; set; } = 1;

      public ICommand ResetZoomCommand { get; }

      public TopoDialogPageViewModel() {
         ResetZoomCommand = new Command(_ => Scale = 1, _ => Scale > 1);
      }

      public override void OnDialogOpened(IDialogParameters parameters) {
         base.OnDialogOpened(parameters);

         if (parameters.TryGetValue(nameof(Info), out RouteInfo info) &&
            parameters.TryGetValue(nameof(Topo), out Topo topo)) {
            Info = info;
            Topo = topo;
         } else {
            Close();
         }
      }

      public void OnScaleChanged() {
         (ResetZoomCommand as Command)?.ChangeCanExecute();
      }

      internal static IDialogParameters InitializeParameters(RouteInfo info, Topo topo) {
         return new DialogParameters() {
            { nameof(Info), info },
            { nameof(Topo), topo }
         };
      }
   }
}
