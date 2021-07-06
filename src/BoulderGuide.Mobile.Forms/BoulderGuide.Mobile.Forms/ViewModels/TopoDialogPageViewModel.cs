using BoulderGuide.DTOs;
using BoulderGuide.Mobile.Forms.Domain;
using BoulderGuide.Mobile.Forms.Services.Preferences;
using BoulderGuide.Mobile.Forms.Views;
using Prism.Services.Dialogs;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace BoulderGuide.Mobile.Forms.ViewModels {
   public class TopoDialogPageViewModel : DialogViewModelBase {

      private readonly IPreferences preferences;
      public RouteInfo Info { get; set; }
      public Topo Topo { get; set; }
      public Color TopoColor { get; set; }

      public double Scale { get; set; } = 1;
      public bool ShowTopo { get; set; } = true;

      public void OnShowTopoChanged() {
         if (ShowTopo) {
            TopoColor = Color.FromHex(preferences.TopoColorHex);
         } else {
            TopoColor = Color.Transparent;
         }
      }

      public ICommand ResetZoomCommand { get; }
      public ICommand ZoomOutCommand { get; }
      public ICommand ZoomInCommand { get; }
      public ICommand ChangeColorCommand { get; }
      public ICommand ShowHideTopoCommand { get; }

      public TopoDialogPageViewModel(IPreferences preferences) {
         this.preferences = preferences;

         ResetZoomCommand = new Command(_ => Scale = 1, _ => Scale > 1);
         ZoomOutCommand = new Command(_ => Scale = Math.Max(Scale / 1.3, 1), _ => Scale > 1);
         ZoomInCommand = new Command(_ => Scale *=  1.3);
         ChangeColorCommand = new AsyncCommand(ChangeColor);
         ShowHideTopoCommand = new Command(() => ShowTopo = !ShowTopo);
      }

      public override void OnDialogOpened(IDialogParameters parameters) {
         base.OnDialogOpened(parameters);
         TopoColor = Color.FromHex(preferences.TopoColorHex);

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

      private async Task ChangeColor() {
         await ShowDialogAsync(
            nameof(ColorPickerDialogPage));

         TopoColor = Color.FromHex(preferences.TopoColorHex);
      }

      internal static IDialogParameters InitializeParameters(RouteInfo info, Topo topo) {
         return new DialogParameters() {
            { nameof(Info), info },
            { nameof(Topo), topo }
         };
      }
   }
}
