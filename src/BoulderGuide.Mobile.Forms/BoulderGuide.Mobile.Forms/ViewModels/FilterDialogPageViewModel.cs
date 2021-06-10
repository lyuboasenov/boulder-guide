using BoulderGuide.Domain.Entities;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace BoulderGuide.Mobile.Forms.ViewModels {
   public class FilterDialogPageViewModel : BindableBase, IDialogAware {
      private readonly Services.Preferences.IPreferences preferences;

      public ICommand DoneCommand { get; }
      public ICommand CloseCommand { get; }

      public string SearchTerm { get; set; }
      public int MinDifficulty { get; set; }
      public int MaxDifficulty { get; set; }


      public FilterDialogPageViewModel(Services.Preferences.IPreferences preferences) {
         this.preferences = preferences;

         DoneCommand = new Command(async () => await Done());
         CloseCommand = new Command(async () => await Close());

         SearchTerm = preferences.FilterSearchTerm;
         MinDifficulty = preferences.FilterMinDifficulty;
         MaxDifficulty = preferences.FilterMaxDifficulty;
      }

      private Task Close() {
         preferences.FilterSearchTerm = string.Empty;
         preferences.FilterMinDifficulty = 0;
         preferences.FilterMaxDifficulty = 150;

         RequestClose?.Invoke(null);

         return Task.CompletedTask;
      }

      private Task Done() {

         preferences.FilterSearchTerm = SearchTerm;
         preferences.FilterMinDifficulty = MinDifficulty;
         preferences.FilterMaxDifficulty = MaxDifficulty;

         RequestClose?.Invoke(null);

         return Task.CompletedTask;
      }

      public bool CanCloseDialog() => true;

      public void OnDialogClosed() { }

      public void OnDialogOpened(IDialogParameters parameters) {

      }

      public event Action<IDialogParameters> RequestClose;

   }
}
