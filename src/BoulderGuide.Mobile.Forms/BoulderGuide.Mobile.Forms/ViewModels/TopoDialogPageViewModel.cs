using BoulderGuide.DTOs;
using BoulderGuide.Mobile.Forms.Domain;
using Prism.Services.Dialogs;
using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace BoulderGuide.Mobile.Forms.ViewModels {
   public class TopoDialogPageViewModel : DialogViewModelBase {
      public RouteInfo Info { get; set; }
      public Topo Topo { get; set; }

      public double Scale { get; set; } = 1;

      public ICommand ResetZoomCommand { get; }
      public ICommand ZoomOutCommand { get; }
      public ICommand ZoomInCommand { get; }

      public TopoDialogPageViewModel() {
         ResetZoomCommand = new Command(_ => Scale = 1, _ => Scale > 1);
         ZoomOutCommand = new Command(_ => Scale = Math.Max(Scale / 1.3, 1), _ => Scale > 1);
         ZoomInCommand = new Command(_ => Scale *=  1.3);
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
         (ZoomOutCommand as Command)?.ChangeCanExecute();
      }

      internal static IDialogParameters InitializeParameters(RouteInfo info, Topo topo) {
         return new DialogParameters() {
            { nameof(Info), info },
            { nameof(Topo), topo }
         };
      }
   }
}
