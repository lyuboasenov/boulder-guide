using BoulderGuide.DTOs;
using BoulderGuide.Mobile.Forms.Services.Data.Entities;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace BoulderGuide.Mobile.Forms.ViewModels {
   public class TopoDialogPageViewModel : BindableBase, IDialogAware {
      public RouteInfo Info { get; set; }
      public Schema Schema { get; set; }

      public double Scale { get; set; } = 1;

      public ICommand ResetZoomCommand { get; }
      public ICommand CloseCommand { get; }

      public TopoDialogPageViewModel() {
         CloseCommand = new Command(() => RequestClose?.Invoke(null));
         ResetZoomCommand = new Command(_ => Scale = 1, _ => Scale > 1);
      }

      public bool CanCloseDialog() => true;

      public void OnDialogClosed() { }

      public void OnDialogOpened(IDialogParameters parameters) {
         if (parameters.TryGetValue(nameof(Info), out RouteInfo info) &&
            parameters.TryGetValue(nameof(Schema), out Schema schema)) {
            Info = info;
            Schema = schema;
         } else {
            RequestClose?.Invoke(null);
         }
      }

      public event Action<IDialogParameters> RequestClose;

      public void OnScaleChanged() {
         (ResetZoomCommand as Command)?.ChangeCanExecute();
      }

      internal static IDialogParameters InitializeParameters(RouteInfo info, Schema schema) {
         return new DialogParameters() {
            { nameof(Info), info },
            { nameof(Schema), schema }
         };
      }
   }
}
