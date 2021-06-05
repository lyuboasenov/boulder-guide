using ClimbingMap.Mobile.Forms.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ClimbingMap.Mobile.Forms.ViewModels {
   public class ViewModelBase : BindableBase, IInitialize, INavigationAware, IDestructible {
      protected INavigationService NavigationService { get; }
      protected IDialogService DialogService { get; }

      private string _title;
      public string Title {
         get { return _title; }
         set { SetProperty(ref _title, value); }
      }


      public ViewModelBase(INavigationService navigationService, IDialogService dialogService) {
         NavigationService = navigationService;
         DialogService = dialogService;
      }

      public virtual void Initialize(INavigationParameters parameters) {

      }

      public virtual void OnNavigatedFrom(INavigationParameters parameters) {

      }

      public virtual void OnNavigatedTo(INavigationParameters parameters) {

      }

      public virtual void Destroy() {

      }

      public async Task HandleExceptionAsync(Exception ex) {
         var dialogParams = new DialogParameters();
         dialogParams.Add(
            DialogPageViewModel.ParameterKeys.Message, ex.Message);
         dialogParams.Add(DialogPageViewModel.ParameterKeys.Severity, DialogPageViewModel.Severity.Error);

         await DialogService.ShowDialogAsync(nameof(DialogPage), dialogParams);
      }
   }
}
